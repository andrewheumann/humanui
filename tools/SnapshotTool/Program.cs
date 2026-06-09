using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Grasshopper.Kernel;

namespace SnapshotTool
{
    /// <summary>
    /// Standalone snapshot generator that walks the same data the branch's ContractSnapshotTests
    /// captures, but in a net48 process so it can load master's HumanUI.dll (which references
    /// Rhino 5 / Grasshopper 0.9.76 assemblies that won't load under net7). Emits JSON in the
    /// same field shape as the branch baseline so the two can be compared semantically.
    /// </summary>
    internal static class Program
    {
        private static int Main(string[] args)
        {
            string outputPath = args.Length > 0 ? args[0] : "master-snapshot.json";

            Assembly asm = typeof(HumanUI.HumanUIInfo).Assembly;
            var snapshot = asm.GetTypes()
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

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            };
            File.WriteAllText(outputPath, JsonSerializer.Serialize(snapshot, options));
            Console.WriteLine($"Wrote {snapshot.Length} components to {Path.GetFullPath(outputPath)}");
            return 0;
        }

        private static ParamSnapshot ToParamSnapshot(IGH_Param param) => new ParamSnapshot
        {
            Type = param.GetType().FullName,
            Name = param.Name,
            NickName = param.NickName,
            Description = param.Description,
            Access = param.Access.ToString(),
            Optional = param.Optional,
        };

        public sealed class ComponentSnapshot
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

        public sealed class ParamSnapshot
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
