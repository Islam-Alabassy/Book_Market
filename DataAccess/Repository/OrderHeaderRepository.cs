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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly MainDbContext db;

        public OrderHeaderRepository(MainDbContext db):base(db) 
        {
            this.db = db;
        }
        public void Save()
        {
            db.SaveChanges();
        }

        public void Update(OrderHeader orderHeader)
        {
            db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
           var orderHeader = db.OrderHeaders.FirstOrDefault(u=>u.Id == id);
            if (orderHeader != null)
            {
                orderHeader.OrderStatus = orderStatus;
                if(paymentStatus != null)
                {  orderHeader.PaymentStatus = paymentStatus; }
            }
        }

        public void UpdateStripPaymentID(int id, string sessionId, string paymentIntentId)
        {
            var orderHeader = db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if(!string.IsNullOrEmpty(sessionId))
            {
                orderHeader.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderHeader.PaymentIntentId = paymentIntentId;
                orderHeader.PaymentDate = DateTime.Now;
            }
        }
    }
}
