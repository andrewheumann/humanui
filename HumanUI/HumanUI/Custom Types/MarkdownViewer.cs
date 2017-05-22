using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace HumanUI
{
    public class MarkdownViewer : FlowDocumentScrollViewer
    {
        private Markdown.Xaml.Markdown md;
        private string _markdownText;
        public string MarkdownText
        {
            get => _markdownText;
            set
            {
                _markdownText = value;
                Document = md.Transform(_markdownText);
            }
        }

        public MarkdownViewer(string text, string styleFile = "", string assetDir = "") : base()
        {

            CommandBindings.Add(new CommandBinding(NavigationCommands.GoToPage, (sender, e) =>
            {
                try
                {

                    Process.Start((string) e.Parameter);
                }
                catch
                {
                    MessageBox.Show("Invalid URL.");
                }
            }));
            var styles = styleFile == "" ? LoadDefaultStyles() : LoadStyles(styleFile);

    

            assetDir = assetDir == "" ? Environment.CurrentDirectory : assetDir;
            md = new Markdown.Xaml.Markdown()
            {
                DocumentStyle = styles["DocumentStyle"] as Style,
                Heading1Style = styles["H1Style"] as Style,
                Heading2Style = styles["H2Style"] as Style,
                Heading3Style = styles["H3Style"] as Style,
                Heading4Style = styles["H4Style"] as Style,
                LinkStyle = styles["LinkStyle"] as Style,
                ImageStyle = styles["ImageStyle"] as Style,
                SeparatorStyle = styles["SeparatorStyle"] as Style,
                AssetPathRoot = assetDir
            };
            MarkdownText = text;
        }


        private static ResourceDictionary LoadStyles(string styleFile)
        {
            try
            {
                using (Stream stream = new FileStream(styleFile, FileMode.Open, FileAccess.Read))
                {
                    ResourceDictionary resources = (ResourceDictionary)XamlReader.Load(stream);
                    return resources;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to load styles file '{styleFile}'.\n{ex.Message}");
            }
        }
        private static ResourceDictionary LoadDefaultStyles()
        {
            // Load a custom styles file if it exists
           
           
            string stylesFile = Path.Combine(Grasshopper.Folders.DefaultAssemblyFolder, "HumanUI/Styles.Default.xaml");


            return LoadStyles(stylesFile);
        }
    }
}