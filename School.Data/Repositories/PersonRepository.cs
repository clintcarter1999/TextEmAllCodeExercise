using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using School.Data.Context;
using School.Data.Models;

namespace School.Data.Repositories
{
    /// <summary>
    /// Note that this implementation is really way more than needed for this Code Challenge.
    /// I am only using the Add from this Repository at the moment.
    /// </summary>
    public class PersonRepository : Repository<Person>, IPersonRepository
    {
        public PersonRepository(SchoolContext context) : base(context)
        {

        }

        // Add specific non-generic methods needed for this repository in this class
    }
}
