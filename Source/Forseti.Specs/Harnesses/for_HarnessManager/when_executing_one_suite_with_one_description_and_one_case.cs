﻿using Forseti.Harnesses;
using Forseti.Pages;
using Forseti.Suites;
using Machine.Specifications;
using It = Machine.Specifications.It;
using Forseti.Files;

namespace Forseti.Specs.Harnesses.for_HarnessManager
{
    [Subject(typeof(HarnessManager))]
    public class when_executing_one_suite_with_one_description_and_one_case : given.a_harness_manager_and_a_framework
    {
        static Suite suite;
        static Description description;
		static Harness harness;
        static Harness harness_result;
        static Page expected_page;
        static string system_search_path;
        static string description_search_path;

        Establish context = () =>
        {

            suite = new Suite((File)"Script/something.js");
            description = new Description((File)"Specs/for_something/when_doing_stuff.js");
            suite.AddDescription(description);

            expected_page = new Page();
            page_generator_mock.Setup(p=>p.GenerateFrom(Moq.It.IsAny<Harness>())).Callback((Harness h) => harness_result = h).Returns(expected_page);
			
			harness = new Harness(framework_mock.Object);
        };

        Because of = () => harness_manager.Execute(harness, new[] {suite});

        It should_generate_a_page = () => page_generator_mock.Verify(p => p.GenerateFrom(Moq.It.IsAny<Harness>()));
        It should_generate_a_page_from_cases = () => harness_result.Cases.ShouldContain(c => c.Name == Case.DummyCase.Name && c.Description == description);
        It should_execute_script_for_page = () => script_engine_mock.Verify(s => s.Execute(expected_page));
    }
}
