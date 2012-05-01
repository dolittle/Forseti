using System.Collections.Generic;
using System.Linq;
using System.Yaml;
using Forseti.Files;
using Forseti.Harnesses;
using File = Forseti.Files.File;
using Forseti.Frameworks;

namespace Forseti.Configuration
{
    public class ConfigurationFileReader : IConfigurationFileReader
    {
        IFileSystemWatcher _fileSystemWatcher;
		IYamlParser _yamlParser;
		IHarnessManager _harnessManager;
		IFile _appliedConfigFile;
		IFrameworkManager _frameworkManager;

        public ConfigurationFileReader(
			IConfigure configure, 
			IFileSystemWatcher fileSystemWatcher, 
			IYamlParser yamlParser,
			IHarnessManager harnessManager,
			IFrameworkManager frameworkManager)
        {
            _fileSystemWatcher = fileSystemWatcher;
			_yamlParser = yamlParser;
			_harnessManager = harnessManager;
			_frameworkManager = frameworkManager;
			
			fileSystemWatcher.SubscribeToChanges(FileChanged);
        }
		
		void FileChanged(FileChange change, IFile file)
		{
			if(_appliedConfigFile == null || 
			   _appliedConfigFile != file)
				return;
			
			Apply (file);
		}
		

        public void Apply(IFile file)
        {
			_appliedConfigFile = file;
			var fileContent = file.ReadAllText();
			
			var nodes = _yamlParser.Parse (fileContent);
			var root = nodes.First() as YamlMapping;
            var globalDependencies = new List<File>();

            if(root != null && root.ContainsKey("Dependencies"))
            {
                var globalDependenciesConfig = root["Dependencies"] as YamlSequence;
                globalDependencies = globalDependenciesConfig
                                    .Select((node) => (File)((YamlScalar)node).Value)
                                    .ToList();
                
            }
			
			if( root != null && root.ContainsKey("Harnesses") ) 
			{
				var harnesses = root["Harnesses"] as YamlSequence;
				if( harnesses != null )
				{
					foreach( YamlMapping harnessConfig in harnesses ) 
					{
						if( harnessConfig.ContainsKey("Harness") )
						{
							var values = harnessConfig["Harness"] as YamlMapping;
							
							var frameworkName = ((YamlScalar)values["Framework"]).Value;
							var framework = _frameworkManager.GetByName(frameworkName);
							
							var harness = new Harness(framework);
							harness.Name = ((YamlScalar)values["Name"]).Value;
							harness.SystemsSearchPath = ((YamlScalar)values["SystemsSearchPath"]).Value;
							harness.DescriptionsSearchPath = ((YamlScalar)values["DescriptionsSearchPath"]).Value;
                            if (values.ContainsKey("Dependencies"))
                            {
                                var harnessDependencies =
                                    ((YamlSequence) values["Dependencies"])
                                    .Select((node) => (File)((YamlScalar) node).Value)
                                    .ToList();

                                globalDependencies.AddRange(harnessDependencies);

                                globalDependencies.ForEach(harness.AddDependency);
                            }

						    _harnessManager.Add (harness);
						}
					}
				}
			}
        }
    }
}
