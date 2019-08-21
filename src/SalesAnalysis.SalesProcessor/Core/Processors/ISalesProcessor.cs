using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Core.Processors
{
    public interface ISalesProcessor
    {
        Task ProcessInputFile(InputFile inputFile);
    }
}