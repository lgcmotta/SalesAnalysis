using System;
using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.FileWatcher.Core.Domain
{
    public class InputFile
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FileExtension { get; set; }

        public bool Processed { get; set; }

        public DateTime ProcessDate { get; set; }
    }
}
