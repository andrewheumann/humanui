using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Grasshopper.Kernel;
using Xunit;

namespace HumanUI.Tests
{
    /// <summary>
    /// Snapshot-style regression test over the GH-facing public contract of every component
    /// in the HumanUI assembly. The shape of inputs/outputs and the component GUID are what
    /// user .gh files encode against -- any change here can silently break saved files. This
    /// captures the contract into a checked-in JSON baseline and fails on any drift.
    ///
    /// To intentionally update the baseline (e.g. after adding a component), set the env var
    /// HUMANUI_REGENERATE_SNAPSHOT=1 and run the test. It will overwrite the baseline and
    /// report the path; review the diff in source control before committing.
    /// </summary>
    public class ContractSnapshotTests
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        };

        [Fact]
        public void Snapshot_MatchesBaseline()
        {
            ComponentSnapshot[] current = BuildSnapshot();
            string actual = JsonSerializer.Serialize(current, JsonOptions);

            string baselinePath = ResolveBaselinePath();

            bool regenerate = Environment.GetEnvironmentVariable("HUMANUI_REGENERATE_SNAPSHOT") == "1";
            if (regenerate || !File.Exists(baselinePath))
            {
                File.WriteAllText(baselinePath, actual);
                Assert.Fail(File.Exists(baselinePath) && !regenerate
                    ? $"Baseline did not exist; wrote a fresh one at {baselinePath}. Review and commit it, then re-run."
                    : $"Baseline rewritten at {baselinePath} because HUMANUI_REGENERATE_SNAPSHOT=1. Review the diff and commit.");
            }

            string expected = File.ReadAllText(baselinePath);
            Assert.Equal(NormalizeNewlines(expected), NormalizeNewlines(actual));
        }

        private static ComponentSnapshot[] BuildSnapshot()
        {
            Assembly asm = typeof(HumanUIInfo).Assembly;
            return asm.GetTypes()
                .Where(t => !t.IsAbstract
                            && typeof(GH_Component).IsAssignableFrom(t)
                            && t.GetConstructor(Type.EmptyTypes) != null)
                .Select(t => (GH_Component)Activator.CreateInstance(t))
                .Select(c => new ComponentSnapshot
                {
                    Type = c.GetType().FullName,
                    Guid = c.ComponentGuid.ToString("D"),
                    Name = c.Name,
                    NickName = c.NickName,
                    Category = c.Category,
                    SubCategory = c.SubCategory,
                    Exposure = c.Exposure.ToString(),
                    Inputs = c.Params.Input.Select(ToParamSnapshot).ToArray(),
                    Outputs = c.Params.Output.Select(ToParamSnapshot).ToArray(),
                })
                .OrderBy(s => s.Guid, StringComparer.Ordinal)
                .ToArray();
        }

        private static ParamSnapshot ToParamSnapshot(IGH_Param param) => new()
        {
            Type = param.GetType().FullName,
            Name = param.Name,
            NickName = param.NickName,
            Description = param.Description,
            Access = param.Access.ToString(),
            Optional = param.Optional,
        };

        private static string ResolveBaselinePath([CallerFilePath] string thisFile = null)
            => Path.Combine(Path.GetDirectoryName(thisFile)!, "contract-baseline.json");

        private static string NormalizeNewlines(string s) => s.Replace("\r\n", "\n");

        private sealed class ComponentSnapshot
        {
            public string Type { get; set; }
            public string Guid { get; set; }
            public string Name { get; set; }
            public string NickName { get; set; }
            public string Category { get; set; }
            public string SubCategory { get; set; }
            public string Exposure { get; set; }
            public ParamSnapshot[] Inputs { get; set; }
            public ParamSnapshot[] Outputs { get; set; }
        }

        private sealed class ParamSnapshot
        {
            public string Type { get; set; }
            public string Name { get; set; }
            public string NickName { get; set; }
            public string Description { get; set; }
            public string Access { get; set; }
            public bool Optional { get; set; }
        }
    }
}
