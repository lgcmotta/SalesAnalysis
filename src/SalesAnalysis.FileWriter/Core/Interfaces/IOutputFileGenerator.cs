using System.Threading.Tasks;
using SalesAnalysis.FileWriter.Application.DTO;

namespace SalesAnalysis.FileWriter.Core.Interfaces
{
    public interface IOutputFileGenerator
    {
        void GenerateFIle(OutputFileContentDto outputDto);
    }
}