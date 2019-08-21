using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Core.Interfaces
{
    public interface ISalesFileAnalyser
    {
        Task ProcessInputFile(InputFile inputFile);
    }
}