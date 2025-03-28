using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface IServiceRepository
    {
        List<Service> GetAllServices();
    }
}
