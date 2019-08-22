using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Application.DTO;

namespace SalesAnalysis.SalesProcessor.Core.Interfaces
{
    public interface IOutputDataProcessor
    {
        void BuildOutputData(FileContentDto fileContent);
    }
}