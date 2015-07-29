using Microsoft.WindowsAzure.Storage.Table;
using SCAMP.Contracts;

namespace SCAMP.Azure
{
    public class StudentEntity : TableEntity, IEntityWithId
    {
        internal const string EntityKey = "Student";

        public StudentEntity()
            : base(EntityKey, "0")
        {
        }

        public StudentEntity(IStudent student)
            : this()
        {
            FirstName = student.FirstName;
            LastName = student.LastName;
            RowKey = student.MicrosoftId;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public void IncrementId()
        {
        }
    }
}
