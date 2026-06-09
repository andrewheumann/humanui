using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xunit;

namespace HumanUI.Tests
{
    /// <summary>
    /// Live WPF tests that catch what static analysis and constructor-only smoke tests miss:
    /// XAML pack-URI resource resolution and string-keyed Style/Brush lookups, both of which
    /// fail at runtime when MahApps.Metro (or any other styled library) renames its resource
    /// dictionaries between major versions.
    ///
    /// All tests need an STA thread plus a live WPF Application -- [StaFact]/[StaTheory] from
    /// the Xunit.StaFact package gives us the apartment state, and EnsureApplication() spins
    /// up the singleton on first use.
    /// </summary>
    public class UiIntegrationTests
    {
        private static Application EnsureApplication()
            => Application.Current ?? new Application();

        [StaFact]
        public void MainWindow_ConstructsWithoutException()
        {
            EnsureApplication();
            var win = new HumanUIBaseApp.MainWindow();
            Assert.NotNull(win);
            Assert.Equal(3, win.Resources.MergedDictionaries.Count);
        }

        [StaFact]
        public void MainWindow_DefaultsToCenterScreen()
        {
            // Guards against a regression of the "appears off-screen on multi-monitor"
            // failure mode: when WindowStartupLocation defaults to Manual the window
            // opens at (0, 0) on the primary monitor regardless of where Rhino lives.
            EnsureApplication();
            var win = new HumanUIBaseApp.MainWindow();
            Assert.Equal(WindowStartupLocation.CenterScreen, win.WindowStartupLocation);
        }

        [Fact]
        public void ValueListener_EventedElementsIsInstanceLevel()
        {
            // The original code made this static, so two ValueListener components on
            // the same canvas trampled each other's wired-element bookkeeping. Pin the
            // instance-level layout to prevent regression.
            var field = typeof(ValueListener_Component).GetField(
                "eventedElements",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(field);
            Assert.False(field.IsStatic, "eventedElements must not be static");
        }

        // Every MahApps pack-URI the codebase loads at runtime. If a future MahApps upgrade or
        // an Eto migration moves any of these, the test fails with the specific URI that broke.
        public static IEnumerable<object[]> MahAppsResourceUris()
        {
            yield return new object[] { "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml" };
            yield return new object[] { "pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml" };
            yield return new object[] { "pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml" };
        }

        [StaTheory]
        [MemberData(nameof(MahAppsResourceUris))]
        public void MahAppsResourceUri_LoadsAsResourceDictionary(string uri)
        {
            EnsureApplication();
            var rd = new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) };
            Assert.NotEmpty(rd.Keys.Cast<object>().Concat(rd.MergedDictionaries.SelectMany(d => d.Keys.Cast<object>())));
        }

        // Every string-keyed lookup against a MahApps resource dictionary in source. Each row
        // is (resourceUri, key, expectedRuntimeType). expectedRuntimeType is checked loosely
        // (subtype/interface match) so MahApps can still re-skin its internals.
        public static IEnumerable<object[]> MahAppsKeyedResources()
        {
            const string controls = "pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml";
            const string lightBlue = "pack://application:,,,/MahApps.Metro;component/Styles/Themes/Light.Blue.xaml";

            // Used by CreateButton_Component.cs
            yield return new object[] { controls, "MahApps.Styles.Button", typeof(Style) };
            yield return new object[] { controls, "MahApps.Styles.Button.Square", typeof(Style) };
            yield return new object[] { controls, "MahApps.Styles.Button.Circle", typeof(Style) };

            // Used by TabContainer_Component.cs
            yield return new object[] { controls, "MahApps.Styles.TabItem", typeof(Style) };
            yield return new object[] { lightBlue, "MahApps.Brushes.Accent", typeof(Brush) };
        }

        [StaTheory]
        [MemberData(nameof(MahAppsKeyedResources))]
        public void MahAppsResourceKey_ResolvesToExpectedType(string uri, string key, Type expectedType)
        {
            EnsureApplication();
            var rd = new ResourceDictionary { Source = new Uri(uri, UriKind.RelativeOrAbsolute) };
            object value = rd[key];
            Assert.NotNull(value);
            Assert.True(expectedType.IsAssignableFrom(value.GetType()),
                $"{key} resolved to {value.GetType().FullName}, expected assignable to {expectedType.FullName}");
        }
    }
}
