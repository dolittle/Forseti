using System.IO;
using System.Text;
using Forseti.Resources;
using Spark;
using Spark.FileSystem;
using System;
using System.Collections.Generic;
using Forseti.Harnesses;

namespace Forseti.Pages.Spark
{
    public class PageGenerator : IPageGenerator
    {
        const string TemplateName = "Harness";
        SparkViewDescriptor _descriptor;
        SparkViewEngine _engine;

        public PageGenerator(IResourceManager resourceManager)
        {
            var template = resourceManager.GetStringFromAssemblyOf<PageGenerator>("Forseti.Pages.Spark.Harness.spark");

            var settings = new SparkSettings().SetPageBaseType(typeof(HarnessView));
            var templates = new InMemoryViewFolder();
            _engine = new SparkViewEngine(settings)
            {
                ViewFolder = templates
            };
            templates.Add(TemplateName, template); 
            _descriptor = new SparkViewDescriptor().AddTemplate(TemplateName);
        }


        public Page GenerateFrom(Harness harness)
        {
            var page = new Page();

            var harnessView = (HarnessView)_engine.CreateInstance(_descriptor);
            harnessView.Harness = harness;
            harnessView.FrameworkScript = harness.Framework.ScriptName;
            harnessView.FrameworkExecutionScript = harness.Framework.ExecuteScriptName;
            harnessView.FrameworkReportingScript = harness.Framework.ReportScriptName;

            page.RootPath = Path.GetTempPath() + @"Forseti/";
            page.Filename = string.Format("{0}runner.html", page.RootPath);

            if (!Directory.Exists(page.RootPath))
                Directory.CreateDirectory(page.RootPath);

            if (harness.HasDependencies())
            {
                var actualDependencies = new List<string>();
                foreach (var dependency in harness.Dependencies)
                {
                    CopyScript(page.RootPath, dependency.RelativePath);
                    actualDependencies.Add(dependency.RelativePath);
                }
                harnessView.Dependencies = actualDependencies.ToArray();
            } else {
				harnessView.Dependencies = new string[0];
			}
							

            var writer = new StringWriter();
            harnessView.RenderView(writer);

            var result = writer.ToString();

            File.WriteAllText(page.RootPath + harness.Framework.ScriptName, harness.Framework.Script);
            File.WriteAllText(page.RootPath + harness.Framework.ExecuteScriptName, harness.Framework.ExecuteScript);
            File.WriteAllText(page.RootPath + harness.Framework.ReportScriptName, harness.Framework.ReportScript);

            foreach (var scriptFile in harnessView.SystemScripts)
                CopyScript(page.RootPath, scriptFile);
			
			
			
            foreach (var scriptFile in harnessView.CaseScripts)
                CopyScript(page.RootPath, scriptFile);

            File.WriteAllText(page.Filename, result);

            return page;
        }


        void CopyScript(string rootPath, Files.File scriptFile)
        {
            var target = rootPath + scriptFile;
            var script = scriptFile.ReadAllText();
				//File.ReadAllText(scriptFile);

            var dir = Path.GetDirectoryName(target);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(target, script, Encoding.ASCII);
        }
    }
}
