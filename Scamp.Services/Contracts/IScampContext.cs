using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCAMP.Contracts
{
    public interface IScampContext
    {
        IEnumerable<ICourse> Courses { get; }
        ICourse AddCourse(ICourse course);
        ICourse GetCourse(int id);
        // Remove
        ICourse UpdateCourse(ICourse c);
        IStudent AddStudent(IStudent student);
        IEnumerable<IStudent> GetStudents();
        IStudent GetStudent(string id);
        Task<string> GetToken();
        void AddStudentToCourse(IStudent student, ICourse c);
        IEnumerable<IStudent> GetStudentsInCourse(ICourse c);
        IEnumerable<IResource> GetResoucesForStudentInCourse(ICourse c, IStudent s);
        void AddWebSite(ICourse course, IStudent student, IWebSite webSite);
    }
}
