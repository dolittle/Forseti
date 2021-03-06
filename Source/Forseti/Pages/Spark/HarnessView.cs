﻿using System;
using Spark;
using System.Collections.Generic;
using Forseti.Harnesses;
using System.Linq;

namespace Forseti.Pages.Spark
{
    public class HarnessView : AbstractSparkView
    {
        Harness _harness;


        public HarnessView()
        {
            FrameworkScript = string.Empty;
            FrameworkExecutionScript = string.Empty;
            FrameworkExecutionScript = string.Empty;
        }

        public Harness Harness 
        {
            get { return _harness; }
            set
            {
                _harness = value;
                Prepare();
            }
        }

        public string[] RunnerScripts { get; set; }

        public string FrameworkScript { get; set; }
        public string FrameworkExecutionScript { get; set; }
        public string FrameworkReportingScript { get; set; }
		
        public string[] SystemScripts { get; set; }
        public CaseScriptDescriptor[] CaseScripts { get; set; }

        public string[] Dependencies { get; set; }

        public override Guid GeneratedViewId { get { return Guid.NewGuid(); } }
        public override void Render()
        {
        }


        void Prepare()
        {
            var caseScripts = new List<string>();
            var systemScripts = new List<string>();

            foreach (var @case in _harness.Cases)
            {
                var caseScript = @case.Description.File.RelativePath;
                if( !caseScripts.Contains(caseScript) )
                    caseScripts.Add(caseScript);

                var systemScript = @case.Description.Suite.SystemFile.RelativePath;
                if (!systemScripts.Contains(systemScript))
                    systemScripts.Add(systemScript);
            }

            CaseScripts = caseScripts.Select(c => { return (CaseScriptDescriptor)c; }).ToArray();
            SystemScripts = systemScripts.ToArray();
        }

    }
}
