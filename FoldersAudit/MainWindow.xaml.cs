using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FoldersAudit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FoldersControl_Initialized(object sender, EventArgs e)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                TreeViewItem item = new()
                {
                    Header = drive.Name,
                    Tag = drive.RootDirectory,
                    IsEnabled = drive.IsReady
                };

                FoldersControl.Items.Add(item);
            }
        }

        private void FoldersControl_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ExpandFoldersControlItem((TreeViewItem)e.NewValue);
        }

        private void FoldersControlItem_Expanded(object sender, RoutedEventArgs e)
        {
            ExpandFoldersControlItem((TreeViewItem)e.Source);
        }

        private void ExpandFoldersControlItem(TreeViewItem selectedItem)
        {
            Cursor = Cursors.Wait;

            DirectoryInfo selectedDir = (DirectoryInfo)selectedItem.Tag;
            selectedItem.Items.Clear();

            EnumerationOptions options = new();
            var dirs = selectedDir.GetDirectories("*", options);
            int n = dirs.Length;

            if (n > 0)
            {
                ProgressControl.Maximum = dirs.Length;
                int i = 0;

                foreach (DirectoryInfo dir in dirs)
                {
                    TreeViewItem item = new()
                    {
                        Header = dir.Name,
                        Tag = dir
                    };

                    var subdirs = dir.GetDirectories("*", options);
                    int count = subdirs.Length;
                    if (count > 0)
                    {
                        item.Header = $"{dir.Name} +{count}";
                        item.Items.Add(count);
                        item.Expanded += FoldersControlItem_Expanded;
                    }

                    selectedItem.Items.Add(item);
                    ProgressControl.Value = ++i;
                }

                selectedItem.IsExpanded = true;
            }
            selectedItem.BringIntoView();
            

            PathControl.Text = selectedDir.FullName;

            FilesControl.Items.Clear();
            var files = selectedDir.GetFiles("*", options);
            n = files.Length;

            if (n > 0)
            {
                ProgressControl.Maximum = n;
                int i = 0;
                foreach (FileInfo file in files)
                {
                    FilesControl.Items.Add(file.Name);
                    ProgressControl.Value = ++i;
                }
            }

            ProgressControl.Value = 0;
            Cursor = Cursors.Arrow;
        }
    }
}
