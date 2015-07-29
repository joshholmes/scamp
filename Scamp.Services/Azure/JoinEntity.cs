using System.Runtime.Serialization;
using Microsoft.WindowsAzure.Storage.Table;

namespace SCAMP.Azure
{
    [DataContract]
    public class JoinEntity : TableEntity
    {
        public JoinEntity()
        {
        }

        public JoinEntity(string keyA, string keyB)
        {
            PartitionKey = keyA;
            RowKey = keyB;
        }
    }
}
