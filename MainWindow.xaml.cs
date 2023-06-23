using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TagManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataAsset dataAsset;

        public DataAsset DataAsset
        {
            get { return dataAsset; }
            set { dataAsset = value; }
        }

        public MainWindow()
        {
            this.DataAsset = new DataAsset();

            this.DataContext = this.DataAsset;

            InitializeComponent();
        }

        private void folderDialog_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            //dlg.DefaultExt = ".txt"; // Default file extension
            //dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                this.dataAsset.SetPathes(dlg.FileName);

                //Console.WriteLine(this.dataAsset.DataFolderPath);
            }
        }

        private void addNewTag_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tb_tag2add.Text))
            {
                this.DataAsset.AddNewTagToAllTagsList(new SelectableTag(tb_tag2add.Text));
            }
        }

        private void deleteTag_Click(object sender, RoutedEventArgs e)
        {
            this.dataAsset.DeleteTagFromAllTagsList((SelectableTag)allTagsListBox.SelectedItem);
        }

        private void tb_tag2add_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(tb_tag2add.Text))
                {
                    this.DataAsset.AddNewTagToAllTagsList(new SelectableTag(tb_tag2add.Text));
                }
            }
        }

        private void tv_folder_explorer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(e);
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var myTextBlockItem = sender as System.Windows.Controls.TextBlock;
            this.dataAsset.SelectedFolder.UpdateSelectedFolder(myTextBlockItem.DataContext as FolderItem);

        }

        private void TextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var myTextBlockItem = sender as System.Windows.Controls.TextBlock;
            //this.dataAsset.SelectedFolder.UpdateSelectedFolder(myTextBlockItem.DataContext as FolderItem);
        }
    }
}
