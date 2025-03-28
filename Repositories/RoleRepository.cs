using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleDAO _roleDao;

        public RoleRepository(RoleDAO roleDao)
        {
            _roleDao = roleDao;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleDao.GetAllRolesAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _roleDao.GetRoleByIdAsync(id);
        }
    }
}
