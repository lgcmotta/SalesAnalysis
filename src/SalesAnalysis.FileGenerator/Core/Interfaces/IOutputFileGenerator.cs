using SalesAnalysis.FileGenerator.Application.DTO;

namespace SalesAnalysis.FileGenerator.Core.Interfaces
{

    public interface IOutputFileGenerator
    {
        void GenerateFIle(OutputFileContentDto outputDto);
    }
}