using System;

namespace TagManager
{
    public class AssetsManager
    {
        public AssetsFolder AssetsFolder { get; set; }

        public AssetsManager()
        {
            this.AssetsFolder = new AssetsFolder();
        }

        internal void Init(string filePath)
        {
            this.AssetsFolder.Init(filePath);
        }
    }
}