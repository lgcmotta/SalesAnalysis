using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Core.ViewModel;

namespace SalesAnalysis.SalesProcessor.Core.Processors
{
    public interface IDbProcessor
    {
        Task ProcessDatabaseViewModel(FileContentViewModel viewModel);
    }
}