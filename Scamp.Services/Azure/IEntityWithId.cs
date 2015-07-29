using Microsoft.WindowsAzure.Storage.Table;

namespace SCAMP.Contracts
{
    public interface IEntityWithId : ITableEntity
    {
        void IncrementId();
    }
}
