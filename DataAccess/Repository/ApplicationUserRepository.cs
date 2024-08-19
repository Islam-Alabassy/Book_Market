using DataAccess.Repository.IRepository;
using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUSerRepository
    {
        private readonly MainDbContext db;

        public ApplicationUserRepository(MainDbContext db):base(db) 
        {
            this.db = db;
        }       
    }
}
