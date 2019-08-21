using System.Threading.Tasks;

namespace SalesAnalysis.UnitOfWork.Abstractions
{
    public interface IUnitOfWork
    {
        int Save();

        Task<int> SaveAsync();
    }
}