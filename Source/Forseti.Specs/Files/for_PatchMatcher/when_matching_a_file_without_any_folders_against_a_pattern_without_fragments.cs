using Machine.Specifications;
using Forseti.Files;

namespace Forseti.Specs
{
	public class when_matching_a_file_without_any_folders_against_an_empty_pattern
	{
		static PathMatcher matcher = new PathMatcher("Something");
		static bool result;
		
		Because of = () => result = matcher.Matches((File)"myFile.js");
		
		It should_not_match = () => result.ShouldBeFalse ();
	}
}

