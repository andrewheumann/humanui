using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace HumanUI.Tests
{
    /// <summary>
    /// Semantic diff between the branch's contract baseline and an external snapshot file
    /// (typically produced by SnapshotTool against a master build). Compares JSON
    /// structurally so serializer formatting differences are ignored.
    ///
    /// Skipped unless the HUMANUI_MASTER_SNAPSHOT env var points at a snapshot JSON file.
    /// </summary>
    public class SnapshotComparison
    {
        private readonly ITestOutputHelper _output;
        public SnapshotComparison(ITestOutputHelper output) => _output = output;

        // Components removed during the net7/MahApps 2 modernization. Each one used MahApps 1.x
        // APIs (ThemeManager.ChangeAppTheme / ControlsHelper.SetHeaderFontSize) that no longer
        // exist; all three are GH_Exposure.hidden so removing them is invisible in the toolbar.
        // See commit 49dfc4e for the rationale.
        private static readonly HashSet<string> AcceptedRemovals = new()
        {
            "669ed7cd-5b59-4484-b179-4e8934ab39b3", // TabContainer_Component_ALSO_DEPRECATED
            "aa3816cc-918e-4383-9125-8f00922f154a", // SetWindowProperties_Component_DEPRECATED
            "b2ca4d57-1f81-4ce5-aed6-2a39fb285814", // SetWindowProperties_Component_ALSO_DEPRECATED
        };

        // Intentional contract corrections relative to master. Keyed by ComponentGuid; value is
        // the set of property names allowed to differ.
        private static readonly Dictionary<string, HashSet<string>> AcceptedFieldChanges = new()
        {
            // CreateGrid_Component_DEPRECATED -- category typo fix, "Human" -> "Human UI" (commit 7561ff0)
            ["1e68a9a8-c28d-4799-854c-337dc4018917"] = new() { "Category" },
            // CreateSlider_Component_ALSO_DEPRECATED -- same typo fix
            ["e4f276af-46fb-478e-b10a-b95e7b04dff0"] = new() { "Category" },
        };

        [Fact]
        public void Branch_ContractMatchesMasterSnapshot_ExceptForKnownIntentionalDiffs()
        {
            string testsDir = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(typeof(SnapshotComparison).Assembly.Location)!,
                "..", "..", ".."));

            string masterPath = Environment.GetEnvironmentVariable("HUMANUI_MASTER_SNAPSHOT");
            if (string.IsNullOrWhiteSpace(masterPath))
                masterPath = Path.Combine(testsDir, "master-snapshot.json");
            Assert.True(File.Exists(masterPath), $"Master snapshot not found at {masterPath}");

            string branchPath = Path.Combine(testsDir, "contract-baseline.json");
            Assert.True(File.Exists(branchPath), $"Branch baseline not found at {branchPath}");

            Dictionary<string, JsonElement> master = LoadByGuid(masterPath);
            Dictionary<string, JsonElement> branch = LoadByGuid(branchPath);

            var onlyInMaster = master.Keys.Except(branch.Keys).OrderBy(x => x).ToList();
            var onlyInBranch = branch.Keys.Except(master.Keys).OrderBy(x => x).ToList();
            var inBoth = master.Keys.Intersect(branch.Keys).OrderBy(x => x).ToList();

            var unexpectedRemovals = onlyInMaster.Where(g => !AcceptedRemovals.Contains(g)).ToList();
            var unexpectedAdditions = onlyInBranch.ToList();

            _output.WriteLine($"master snapshot:  {master.Count} components ({masterPath})");
            _output.WriteLine($"branch baseline:  {branch.Count} components ({branchPath})");
            _output.WriteLine($"shared:           {inBoth.Count}");
            _output.WriteLine($"only in master:   {onlyInMaster.Count}");
            _output.WriteLine($"only in branch:   {onlyInBranch.Count}");

            if (onlyInMaster.Count > 0)
            {
                _output.WriteLine("\n=== Components in MASTER but missing from BRANCH ===");
                foreach (string g in onlyInMaster)
                    _output.WriteLine($"  {g}  {master[g].GetProperty("Type").GetString()}");
            }
            if (onlyInBranch.Count > 0)
            {
                _output.WriteLine("\n=== Components in BRANCH but missing from MASTER ===");
                foreach (string g in onlyInBranch)
                    _output.WriteLine($"  {g}  {branch[g].GetProperty("Type").GetString()}");
            }

            var unexpectedDiffs = new List<string>();
            var acceptedDiffs = new List<string>();
            foreach (string guid in inBoth)
            {
                var diffs = DiffComponent(master[guid], branch[guid]);
                if (diffs.Count == 0) continue;

                AcceptedFieldChanges.TryGetValue(guid, out var allowed);
                allowed ??= new HashSet<string>();
                var unexpected = diffs.Where(d => !allowed.Contains(FieldOf(d))).ToList();
                string typeName = branch[guid].GetProperty("Type").GetString();

                if (unexpected.Count > 0)
                {
                    unexpectedDiffs.Add($"  {guid}  {typeName}");
                    foreach (string d in unexpected) unexpectedDiffs.Add($"      {d}");
                }
                else
                {
                    acceptedDiffs.Add($"  {guid}  {typeName}");
                    foreach (string d in diffs) acceptedDiffs.Add($"      {d} (allowlisted)");
                }
            }

            if (acceptedDiffs.Count > 0)
            {
                _output.WriteLine("\n=== Allowlisted intentional contract changes ===");
                foreach (string line in acceptedDiffs) _output.WriteLine(line);
            }
            if (unexpectedDiffs.Count > 0)
            {
                _output.WriteLine("\n=== Unexpected contract differences (regressions) ===");
                foreach (string line in unexpectedDiffs) _output.WriteLine(line);
            }

            Assert.True(unexpectedRemovals.Count == 0,
                $"{unexpectedRemovals.Count} component(s) present in master are missing from the branch " +
                "and not on the AcceptedRemovals allowlist. See test output above.");
            Assert.True(unexpectedAdditions.Count == 0,
                $"{unexpectedAdditions.Count} component(s) added to the branch are not in master. " +
                "Confirm the new GUID is intentional, then add it to AcceptedAdditions or extend this assertion.");
            int unexpectedComponents = unexpectedDiffs.Count(l => !l.StartsWith("      "));
            Assert.True(unexpectedDiffs.Count == 0,
                $"{unexpectedComponents} component(s) drifted from master in unexpected ways. " +
                "Either fix the drift or add the field(s) to AcceptedFieldChanges with a justification.");
        }

        private static string FieldOf(string diffLine)
        {
            // Lines look like "Category: master='Human' -> branch='Human UI'" or
            // "Inputs[0].Name: master='X' -> branch='Y'" -- the field is everything before the first colon,
            // and for params we accept on the dotted field too.
            int colon = diffLine.IndexOf(':');
            string head = colon > 0 ? diffLine.Substring(0, colon) : diffLine;
            int dot = head.LastIndexOf('.');
            return dot > 0 ? head.Substring(dot + 1) : head;
        }

        private static Dictionary<string, JsonElement> LoadByGuid(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            var map = new Dictionary<string, JsonElement>();
            foreach (var element in doc.RootElement.EnumerateArray())
                map[element.GetProperty("Guid").GetString()] = element.Clone();
            return map;
        }

        private static List<string> DiffComponent(JsonElement master, JsonElement branch)
        {
            var diffs = new List<string>();
            foreach (string prop in new[] { "Type", "Name", "NickName", "Category", "SubCategory", "Exposure" })
            {
                string m = master.GetProperty(prop).GetString();
                string b = branch.GetProperty(prop).GetString();
                if (m != b) diffs.Add($"{prop}: master='{m}' -> branch='{b}'");
            }
            DiffParams(master.GetProperty("Inputs"), branch.GetProperty("Inputs"), "Inputs", diffs);
            DiffParams(master.GetProperty("Outputs"), branch.GetProperty("Outputs"), "Outputs", diffs);
            return diffs;
        }

        private static void DiffParams(JsonElement master, JsonElement branch, string label, List<string> diffs)
        {
            int mCount = master.GetArrayLength();
            int bCount = branch.GetArrayLength();
            if (mCount != bCount)
            {
                diffs.Add($"{label} count: master={mCount} -> branch={bCount}");
                return;
            }
            for (int i = 0; i < mCount; i++)
            {
                JsonElement mp = master[i];
                JsonElement bp = branch[i];
                foreach (string prop in new[] { "Type", "Name", "NickName", "Description", "Access" })
                {
                    string m = mp.GetProperty(prop).GetString();
                    string b = bp.GetProperty(prop).GetString();
                    if (m != b) diffs.Add($"{label}[{i}].{prop}: master='{m}' -> branch='{b}'");
                }
                bool mo = mp.GetProperty("Optional").GetBoolean();
                bool bo = bp.GetProperty("Optional").GetBoolean();
                if (mo != bo) diffs.Add($"{label}[{i}].Optional: master={mo} -> branch={bo}");
            }
        }
    }
}
