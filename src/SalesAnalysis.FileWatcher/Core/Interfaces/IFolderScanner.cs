using System.Threading.Tasks;

namespace SalesAnalysis.FileWatcher.Core.Interfaces
{
    public interface IFolderScanner
    {
        Task StartFolderScanAsync();

    }
}