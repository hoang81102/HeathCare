using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceDAO _serviceDAO;

        public ServiceRepository()
        {
            _serviceDAO = new ServiceDAO();
        }

        public List<Service> GetAllServices()
        {
            return _serviceDAO.GetAllServices();
        }
    }
}
