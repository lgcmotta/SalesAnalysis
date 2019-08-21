using System.Threading.Tasks;

namespace SalesAnalysis.FileWatcher.Core.Scanner
{
    public interface IFolderScanner
    {
        Task StartFolderScanAsync();

    }
}