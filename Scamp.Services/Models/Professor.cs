using SCAMP.Contracts;

namespace SCAMP.Models
{
    public class Professor : Person, IProfessor
    {
        public Professor()
        {
            FirstName = "First";
            LastName = "Last";
        }

        public int Id { get; set; }
    }
}
