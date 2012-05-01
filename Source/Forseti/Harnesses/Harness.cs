using System.Collections.Generic;
using System.Text.RegularExpressions;
using Forseti.Files;
using Forseti.Suites;
using Forseti.Frameworks;

namespace Forseti.Harnesses
{
    public class Harness
    {
        const   string SystemComponentName = "{system}";

		string _systemsSearchPath;
		string _descriptionsSearchPath;
        Regex _systemsSearchPathRegex;
        Regex _descriptionsSearchPathRegex;
        Dictionary<string,int> _systemComponents;
        Dictionary<string,int> _descriptionComponents;

        List<Suite> _suites = new List<Suite>();
		List<IFile> _dependencies = new List<IFile>();
		
		public Harness(IFramework framework)
		{
			Framework = framework;
		}
		
		
		public IFramework Framework { get; private set; }
		
		public string Name { get; set; }
		public string SystemsSearchPath 
		{ 
			get { return _systemsSearchPath; }
			set
			{
				_systemsSearchPath = value;
                _systemsSearchPathRegex = BuildSearchRegex(value, out _systemComponents);
			}
		}
		
		public string DescriptionsSearchPath 
		{ 
			get { return _descriptionsSearchPath; }
			set
			{
				_descriptionsSearchPath = value;
                _descriptionsSearchPathRegex = BuildSearchRegex(value, out _descriptionComponents);
			}
		}
		
		public IEnumerable<IFile> Dependencies {get { return _dependencies; }}
        public IEnumerable<Suite> Suites { get { return _suites; } }
        public IEnumerable<Case> Cases { get; set; }

        public void RemoveSuites(IEnumerable<Suite> suites)
        {
            foreach (var suite in suites)
                _suites.Remove(suite);
        }
		
		public bool IsSystem(IFile file)
		{
			var path = file.FullPath;
			if( _systemsSearchPathRegex == null )
				return false;
			
			var isDescription = false;
			if( _descriptionsSearchPathRegex != null )
				isDescription = _descriptionsSearchPathRegex.IsMatch(path);
			
            return _systemsSearchPathRegex.IsMatch(path) && !isDescription;
		}
		
		public bool IsDescription(IFile file)
		{
			var path = file.FullPath;
			if( _descriptionsSearchPathRegex == null )
				return false;
			
			var isSystem = false;
			if( _systemsSearchPathRegex != null )
				isSystem = _systemsSearchPathRegex.IsMatch(path);
			
            return _descriptionsSearchPathRegex.IsMatch(path) && !isSystem; 
		}


        public IEnumerable<Suite> HandleFiles(IEnumerable<IFile> files)
        {
			var affectedSuites = new List<Suite>();
			
            foreach (var file in files)
            {
                var isSystem = IsSystem(file);
                if (isSystem)
                {
                    var suite = new Suite(file);
                    _suites.Add(suite);
                }

                var isDescription = IsDescription(file);
                if (isDescription && _descriptionComponents.ContainsKey(SystemComponentName))
                {
                    var systemComponentIndex = _descriptionComponents[SystemComponentName];
                    foreach (var suite in _suites)
                    {
                        var match = _descriptionsSearchPathRegex.Match(file.FullPath);
                        if (match.Groups[systemComponentIndex+1].Value == suite.System)
                        {
                            var description = new Description(file);

                            // Todo: Hack for now
                            description.AddCase(new Case());
                            suite.AddDescription(description);
							
							if( !affectedSuites.Contains(suite) ) {
								affectedSuites.Add (suite);
							}
                        }
                    }
                }
            }
			
			return affectedSuites;
        }

        Regex BuildSearchRegex(string path, out Dictionary<string, int> extractedComponents)
        {
            var replacePattern = @"\{[a-zA-Z]*\}";
            var componentMatches = Regex.Match(path, replacePattern);

            extractedComponents = new Dictionary<string, int>();
            for (var matchIndex = 0; matchIndex < componentMatches.Length; matchIndex++, componentMatches = componentMatches.NextMatch())
                extractedComponents[componentMatches.Value] = matchIndex;

            var pattern = Regex.Replace(path, replacePattern, "([\\w.]*)");
            return new Regex(pattern);
        }

        public void AddDependency(IFile dependency)
        {
            _dependencies.Add(dependency);
        }

        public bool HasDependencies()
        {
            return _dependencies.Count > 0;
        }
    }
}
