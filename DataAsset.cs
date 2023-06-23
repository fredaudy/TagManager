using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static TagManager.MainWindow;

namespace TagManager
{
    public class DataAsset
    {
        public string DungeonDraftPackageConfigPath { get; set; } = "";
        public AssetsManager AssetsManager { get; set; }
        public SelectedFolder SelectedFolder { get; set; }
        public string DataFolderPath { get; set; } = "";

        private string allTagsFilePath = "allTagsAvailable.json";

        /// <summary>
        /// to store the values of all the tags available in the app
        /// </summary>
        public ObservableCollection<SelectableTag> SelectableTags { get; set; }

        public DataAsset() 
        {
            this.SelectableTags = new ObservableCollection<SelectableTag>();
            this.SelectableTags.CollectionChanged += AllTagsAvailable_CollectionChanged;

            this.AssetsManager = new AssetsManager();
            this.SelectedFolder = new SelectedFolder();

            this.SetAllTagsAvailable();
        }

        private void AllTagsAvailable_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                SelectableTag theNewSelectableTag = e.NewItems[0] as SelectableTag;
                //if the tag already exist
                if (this.SelectableTags.Where(t => t.Tag.ToLower() == theNewSelectableTag.Tag.ToLower()).Count() > 1)
                {
                    this.SelectableTags.RemoveAt(this.SelectableTags.Count - 1);
                    return;
                }
                //check if the new tag is well formated
                if (this.SelectableTags[this.SelectableTags.Count - 1].Tag != FormatedTag(theNewSelectableTag.Tag))
                {
                    this.SelectableTags[this.SelectableTags.Count - 1].Tag = FormatedTag(theNewSelectableTag.Tag);
                }

                this.SerializeAllTagsAvailable();
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                this.SerializeAllTagsAvailable();
            }
        }

        private void SetAllTagsAvailable()
        {
            if (System.IO.File.Exists(allTagsFilePath))
            {
                string allTagsFileContent = "";
                using (var reader = new StreamReader(allTagsFilePath))
                {
                    allTagsFileContent = reader.ReadToEnd();
                }
                if (!string.IsNullOrEmpty(allTagsFileContent))
                {
                    var allTagsFromFiles = JsonConvert.DeserializeObject<List<string>>(allTagsFileContent);

                    //var allTagsList = allTagsFromFiles.Children().ToList();

                    foreach (var item in allTagsFromFiles)
                    {
                        this.AddNewTagToAllTagsList(new SelectableTag(FormatedTag(item.ToString())));
                    }                    
                }
            }
            else
            {
                SerializeAllTagsAvailable();
            }
        }

        private void SerializeAllTagsAvailable()
        {
            //get a list of all tags name availabe
            List<string> allTagsAvailable = this.SelectableTags.Select(t => t.Tag).ToList();
            //create allTagsAvailable.json with default values
            string json = JsonConvert.SerializeObject(allTagsAvailable);
            using (var writer = new StreamWriter(allTagsFilePath))
            {
                writer.WriteLine(json);
            };
        }

        private string FormatedTag(string tagToFormat)
        {
            var tag = tagToFormat.Trim();
            var firstTagLetter = tag.Substring(0, 1);
            var endOfTag = tag.Substring(1);

            return String.Format("{0}{1}", firstTagLetter.ToUpper(), endOfTag);
        }

        public void SetPathes(string filePath)
        {
            this.DungeonDraftPackageConfigPath = filePath;

            this.AssetsManager.Init(filePath);
        }

        internal void AddNewTagToAllTagsList(SelectableTag selectableTag2Add)
        {
            if (selectableTag2Add.Tag != null)
            {
                this.SelectableTags.Add(selectableTag2Add);
                selectableTag2Add.PropertyChanged += SelectableTag2Add_PropertyChanged;
            }
        }

        /// <summary>
        /// the tag property of a SelectableTag from the SelectableTags collection have change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SelectableTag2Add_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SerializeAllTagsAvailable();
        }

        internal void DeleteTagFromAllTagsList(SelectableTag? tag2delete = null)
        {
            if (tag2delete != null)
            {
                SelectableTag? tag2del = this.SelectableTags.Select(t => t)
                                                            .Where(t => t.Tag == tag2delete.Tag).FirstOrDefault();
                if (tag2del != null)
                {
                    this.SelectableTags.Remove(tag2del);
                }
            }
        }
    }

    public class SelectedFolder
    {
        private FolderItem folderItem;

        public FolderItem FolderItem
        {
            get { return folderItem; }
            set 
            { 
                folderItem = value;
                InitContentToDisplay();
            }
        }

        public ObservableCollection<ContentToDisplay> ContentToDisplay { get; set; }

        private void InitContentToDisplay()
        {
            if (this.folderItem != null)
            {
                this.ContentToDisplay.Clear();

                if (this.folderItem.Content.Count > 0)
                {
                    foreach (var item in this.folderItem.Content)
                    {
                        this.ContentToDisplay.Add(new ContentToDisplay()
                        {
                            PictureName = Path.GetFileName(item),
                            PicturePath = item
                        });
                    }
                }
            }
        }

        public SelectedFolder()
        {
            this.ContentToDisplay = new ObservableCollection<ContentToDisplay>();
            this.folderItem = new FolderItem();
        }

        internal void UpdateSelectedFolder(FolderItem? folderItem)
        {
            if (folderItem != null)
            {
                if (folderItem.Content != null)
                {
                    this.FolderItem = folderItem;
                }
            }
        }
    }

    public class ContentToDisplay
    {
        private string picturePath;
        private string pictureName;

        public string PicturePath
        {
            get { return picturePath; }
            set { picturePath = value; }
        }
        public string PictureName
        {
            get { return pictureName; }
            set { pictureName = value; }
        }

        public bool IsSelected { get; set; } = false;
    }

    public class SelectableTag: INotifyPropertyChanged
    {
        public SelectableTag(string tagName)
        {
            this.Tag = tagName;
            this.Selected = false;
        }

        private string tag;

        public string Tag
        {
            get { return tag; }
            set 
            { 
                tag = value;
                NotifyPropertyChanged(nameof(Tag));
            }
        }

        private bool selected;

        public bool Selected
        {
            get { return selected; }
            set 
            { 
                selected = value;
            }
        }

        /// <summary>
        /// the path from the 'textures' folder of the images
        /// </summary>
        public ObservableCollection<string> AssociatedPictures { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}