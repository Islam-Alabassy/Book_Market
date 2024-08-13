using DataAccess.Repository.IRepository;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly MainDbContext db;

        public CategoryRepository(MainDbContext db):base(db) 
        {
            this.db = db;
        }
        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(Category category)
        {
            db.Categories.Update(category);
        }
    }
}
