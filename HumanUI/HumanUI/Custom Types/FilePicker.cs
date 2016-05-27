using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HumanUI
{
    public enum fileDialogType
    {
        SaveFileDialog = 1,
        OpenFileDialog = 0,
        FolderBrowserDialog = 2
    }
    public class FilePicker : DockPanel, INotifyPropertyChanged
    {
        private string _filter;
        public bool fileMustExist { get; set; }
        public string filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (value.Contains("|"))
                {
                    _filter = value;
                } else
                {
                    _filter = value + "|" + value;
                }
            }
        }

        fileDialogType type { get; set; }

        TextBox tb;

        public event PropertyChangedEventHandler PropertyChanged;

        public FilePicker(string buttonLabelTxt = "Browse...", fileDialogType type = fileDialogType.OpenFileDialog, bool fileMustExist = true, string filter = "All Files|*.*") : base()
        {
            _path = "";
            this.fileMustExist = fileMustExist;
            this.filter = filter;
            this.type = type;
            Height = 30;
            Margin = new Thickness(10);
            Button b = new Button();
            TextBlock buttonLabel = new TextBlock();
            buttonLabel.Margin = new Thickness(3);
            buttonLabel.Text = buttonLabelTxt;
            b.Content = buttonLabel;
            SetDock(b, Dock.Right);
            tb = new TextBox();
            tb.MinWidth = 50;
            tb.HorizontalAlignment = HorizontalAlignment.Stretch;
            b.AddHandler(Button.ClickEvent, new RoutedEventHandler(ButtonClick));
            Children.Add(b);
            Children.Add(tb);

        }

        private string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path != value)
                {
                    _path = value;

                    OnPropertyChanged("Path Changed");
                }

            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
            tb.Text = _path;
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            switch (type)
            {
                case fileDialogType.OpenFileDialog:
                    System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
                    ofd.Filter = filter;
                    ofd.CheckFileExists = fileMustExist;
                    ofd.Multiselect = false;
                    var result = ofd.ShowDialog();
                    if (result == System.Windows.Forms.DialogResult.OK)
                    {
                        Path = ofd.FileName;
                    }
                    return;
                case fileDialogType.SaveFileDialog:
                    System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                    sfd.Filter = filter;
                    var saveResult = sfd.ShowDialog();
                    if (saveResult == System.Windows.Forms.DialogResult.OK)
                    {
                        Path = sfd.FileName;
                    }
                    return;
                case fileDialogType.FolderBrowserDialog:
                    System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
                    fbd.ShowNewFolderButton = true;
                    var folderResult = fbd.ShowDialog();
                    if (folderResult == System.Windows.Forms.DialogResult.OK)
                    {
                        Path = fbd.SelectedPath;
                    }
                    return;
            }


        }
    }
}
