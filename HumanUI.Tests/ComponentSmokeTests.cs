using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel;
using Xunit;

namespace HumanUI.Tests
{
    /// <summary>
    /// Smoke tests over every GH_Component subclass in the HumanUI assembly. These sit above
    /// the UI toolkit, so they survive a WPF -> Eto migration: they exercise the Grasshopper
    /// metadata layer (Name, Nickname, Category, ComponentGuid uniqueness) and instantiability.
    /// </summary>
    public class ComponentSmokeTests
    {
        private static readonly Type[] ComponentTypes = LoadComponentTypes();

        private static Type[] LoadComponentTypes()
        {
            Assembly asm = typeof(HumanUIInfo).Assembly;
            return asm.GetTypes()
                .Where(t => !t.IsAbstract
                            && typeof(GH_Component).IsAssignableFrom(t)
                            && t.GetConstructor(Type.EmptyTypes) != null)
                .ToArray();
        }

        public static IEnumerable<object[]> AllComponentTypes()
            => ComponentTypes.Select(t => new object[] { t });

        [Fact]
        public void Assembly_ExposesAtLeastOneComponent()
        {
            Assert.NotEmpty(ComponentTypes);
        }

        [Fact]
        public void AssemblyInfo_IsWellFormed()
        {
            var info = new HumanUIInfo();
            Assert.Equal("Human UI", info.Name);
            Assert.False(string.IsNullOrWhiteSpace(info.Version), "Version must be set");
            Assert.NotEqual(Guid.Empty, info.Id);
            Assert.False(string.IsNullOrWhiteSpace(info.AuthorName));
        }

        [Theory]
        [MemberData(nameof(AllComponentTypes))]
        public void Component_InstantiatesViaParameterlessCtor(Type componentType)
        {
            object instance = Activator.CreateInstance(componentType);
            Assert.NotNull(instance);
            Assert.IsAssignableFrom<GH_Component>(instance);
        }

        [Theory]
        [MemberData(nameof(AllComponentTypes))]
        public void Component_HasNonEmptyName(Type componentType)
        {
            var component = (GH_Component)Activator.CreateInstance(componentType);
            Assert.False(string.IsNullOrWhiteSpace(component.Name),
                $"{componentType.FullName} has an empty Name");
            Assert.False(string.IsNullOrWhiteSpace(component.NickName),
                $"{componentType.FullName} has an empty NickName");
        }

        [Theory]
        [MemberData(nameof(AllComponentTypes))]
        public void Component_LivesInHumanUiCategory(Type componentType)
        {
            var component = (GH_Component)Activator.CreateInstance(componentType);
            Assert.Equal("Human UI", component.Category);
        }

        [Theory]
        [MemberData(nameof(AllComponentTypes))]
        public void Component_HasNonEmptyComponentGuid(Type componentType)
        {
            var component = (GH_Component)Activator.CreateInstance(componentType);
            Assert.NotEqual(Guid.Empty, component.ComponentGuid);
        }

        [Fact]
        public void ComponentGuids_AreUniqueAcrossAssembly()
        {
            var guidsByType = ComponentTypes
                .Select(t => new { Type = t, Component = (GH_Component)Activator.CreateInstance(t) })
                .Select(x => new { x.Type, x.Component.ComponentGuid })
                .ToList();

            var duplicates = guidsByType
                .GroupBy(x => x.ComponentGuid)
                .Where(g => g.Count() > 1)
                .Select(g => $"{g.Key}: {string.Join(", ", g.Select(x => x.Type.FullName))}")
                .ToList();

            Assert.True(duplicates.Count == 0,
                "Duplicate ComponentGuids:\n" + string.Join("\n", duplicates));
        }
    }
}
