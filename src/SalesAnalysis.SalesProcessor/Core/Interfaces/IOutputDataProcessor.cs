using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Application.DTO;

namespace SalesAnalysis.SalesProcessor.Core.Interfaces
{
    public interface IOutputDataProcessor
    {
        Task BuildOutputData(FileContentDto fileContent);
    }
}