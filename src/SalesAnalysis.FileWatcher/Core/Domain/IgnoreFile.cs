using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAnalysis.FileWatcher.Core.Domain
{
    public class IgnoreFile
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileExtension { get; set; }
    }
}
