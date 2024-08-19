using DataAccess.Repository.IRepository;
using Models;
using Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly MainDbContext db;

        public ShoppingCartRepository(MainDbContext db):base(db) 
        {
            this.db = db;
        }
        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(ShoppingCart shoppingCart)
        {
            db.ShoppingCarts.Update(shoppingCart);
        }
    }
}
