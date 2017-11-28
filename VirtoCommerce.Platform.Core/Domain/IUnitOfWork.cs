using System.Threading.Tasks;

namespace VirtoCommerce.Platform.Core.Common
{
    public interface IUnitOfWork
	{
		int Commit();
        Task<int> CommitAsync();
	}
}
