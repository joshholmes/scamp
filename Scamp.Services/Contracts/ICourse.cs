using System;
using SCAMP.Azure;

namespace SCAMP.Contracts
{
    public interface ICourse
    {
        int Id { get; }
        string Name { get; set; }
        CourseState State { get; set; }
        DateTimeOffset ExpirationTime { get; set; }
        IProfessor Professor { get; set; }
        Decimal Cost { get; set; }
    }
}
