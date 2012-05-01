using System.Collections.Generic;
using Forseti.Configuration;
using Forseti.Files;
using Forseti.Harnesses;
using Forseti.Scripting;
using Machine.Specifications;
using Moq;
using Forseti.Frameworks;

namespace Forseti.Specs.Configuration.for_ConfigurationFileReader.given
{
	public class a_configuration_file_reader
	{
		static List<FileChanged>	file_changed_delegates = new List<FileChanged>();
		
		protected static ConfigurationFileReader reader;
		protected static Mock<IFileSystemWatcher> file_system_watcher_mock;
		protected static Mock<IYamlParser> yaml_parser_mock;
		protected static Mock<IConfigure>	configure_mock;
		protected static Mock<IHarnessManager> harness_manager_mock;
		protected static Mock<IFrameworkManager> framework_manager_mock;
		
		Establish context = () => 
		{ 
			file_system_watcher_mock = new Mock<IFileSystemWatcher>();
			
			file_system_watcher_mock.Setup (f=>f.SubscribeToChanges(Moq.It.IsAny<FileChanged>())).Callback ((FileChanged f)=>file_changed_delegates.Add(f));
			configure_mock = new Mock<IConfigure>();
			harness_manager_mock = new Mock<IHarnessManager>();
			framework_manager_mock = new Mock<IFrameworkManager>();
			
			yaml_parser_mock = new Mock<IYamlParser>();
			reader = new ConfigurationFileReader(
				configure_mock.Object,
				file_system_watcher_mock.Object,
				yaml_parser_mock.Object,
				harness_manager_mock.Object,
				framework_manager_mock.Object
			);
		};
		
		protected static void FireFileChanged(FileChange change, IFile file)
		{
			foreach( var file_changed_delegate in file_changed_delegates ) 
				file_changed_delegate(change, file);
		}
	}
}

