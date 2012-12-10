﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Forseti.TRX.Reporting
{
    public class TrxBuilder 
    {
        //public const string XMLNS = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";
        public static readonly XNamespace XMLNS = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

        public TestRun TestRun { get; private set; }
        public TestSettings RunSettings { get; private set; }
        public ResultSummary Summary { get; private set; }
        public Times Timing { get; private set; }
        public TestLists TestLists { get; private set; }

        public TestDefinitions Definitions { get; private set; }
        public TestEntries TestEntries { get; private set; }
        public Results Results { get; private set; }


        public TrxBuilder()
        {
            TestRun = new TestRun();
            RunSettings = new TestSettings(); //TestSettings.WithDefaults
            Summary = new ResultSummary();
            Timing = new Times();
            TestLists = new TestLists();

            Definitions = new TestDefinitions();
            TestEntries = new TestEntries();
            Results = new Results();
        }


        public TrxBuilder SetRunInformation(Guid id, string name, string runUser)
        {
            TestRun.Id = id;
            TestRun.Name = name;
            TestRun.RunUser = runUser;

            return this;
        }

        public TrxBuilder SetDefaultTestSettingsWithDescription(string description) 
        {
            RunSettings.Description = description;
            RunSettings.ExecutionAgentRule = TestSettings.DefaultExecutionAgentRule;
            RunSettings.RunDeploymentRoot = TestSettings.DefaultDeploymentRoot;
            return this;
        }

        public TrxBuilder SetResultSummary(int passed, int failed) 
        {
            Summary.Passed = passed;
            Summary.Failed = failed;

            return this;
        
        }

        public TrxBuilder SetRunTimes(DateTime start, DateTime end)
        {
            Timing.StartTime = start;
            Timing.EndTime = end;

            return this;
        }



        public TrxBuilder AddTestResult(string name, Guid id, string computerName, UnitTestResult.ResultOutcome outcome ,string testFilePath = @"C:\TestFilePath\SomeTest", string testClassName = @"SomeTest")
        {
            var testId = Guid.NewGuid();
            var executionId = Guid.NewGuid();

            Definitions.AddDefinition(testId, name, executionId, testFilePath, testClassName);

            TestEntries.AddEntry(testId, executionId);

            Results.AddResult(name, testId, executionId, computerName, outcome);
            
            return this;
        }

        public XDocument Build() 
        {
            var testRun = TestRun.GenerateTrxPart();
            testRun.Add( RunSettings.GenerateTrxPart(),
                         Timing.GenerateTrxPart(),
                         Summary.GenerateTrxPart(),
                         Definitions.GenerateTrxPart(),
                         TestLists.GenerateTrxPart(),
                         TestEntries.GenerateTrxPart(),
                         Results.GenerateTrxPart());

            var trx = new XDocument(testRun);

            return trx;
        }
    }
}