using System.Threading.Tasks;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Core.Interfaces
{
    public interface ISalesFileAnalyser
    {
        void ProcessInputFile(InputFile inputFile);
    }
}