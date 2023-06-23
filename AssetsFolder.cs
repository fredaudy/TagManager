using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TagManager
{
    public class AssetsFolder
    {
        private DirectoryInfo rootFolderPath;

        public ObservableCollection<FolderItem> FoldersStructure { get; set; }

        public AssetsFolder()
        {
            this.FoldersStructure = new ObservableCollection<FolderItem>();
        }

        private void BuildFoldersStructure(DirectoryInfo rootFolderPath, ObservableCollection<FolderItem> foldersStructure)
        {
            var assetsFolder = rootFolderPath.GetDirectories();
            foreach (DirectoryInfo folder in assetsFolder)
            {
                ObservableCollection<FolderItem> childFolders = new ObservableCollection<FolderItem>();
                BuildFoldersStructure(folder, childFolders);
                foldersStructure.Add(new FolderItem()
                {
                    Name = folder.Name,
                    Path = folder.FullName,
                    Child = childFolders
                });
            }
        }


        internal void Init(string filePath)
        {
            this.rootFolderPath = System.IO.Directory.GetParent(System.IO.Directory.GetParent(filePath).ToString());

            this.BuildFoldersStructure(this.rootFolderPath, this.FoldersStructure);
        }
    }

    public class FolderItem
    {
        public string Name { get; set; }

        private string path;

        public string Path
        {
            get { return path; }
            set 
            { 
                path = value; 
                GetFileContent(path);
            }
        }

        public FolderItem()
        {
            this.Content = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Content { get; set; }
        public ObservableCollection<FolderItem> Child { get; set; }

        public void GetFileContent(string path)
        {
            List<string> allPathes = new List<string>();
            if (!string.IsNullOrEmpty(path))
            {
                var fileContent = Directory.GetFiles(path);
                allPathes = fileContent.Select(f => System.IO.Path.GetFullPath(f))
                                       .Where(f => System.IO.Path.GetExtension(f) == ".png")
                                       .ToList();
                this.Content.Clear();
                foreach (var file in allPathes) 
                { 
                    this.Content.Add(file.ToString());
                }
            }
        }
    }
}