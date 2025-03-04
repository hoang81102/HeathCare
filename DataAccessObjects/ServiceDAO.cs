using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class ServiceDAO : SingletonBase<Service>
    {
        public List<Service> GetAllServices()
        {
            return _context.Services
                .Include(s => s.Bookings) 
                .ToList();
        }
    }
}
