using BusinessObjects;
using DataAccessObjects;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly AccountDAO _accountDAO;

        public AccountRepository(AccountDAO accountDAO)
        {
            _accountDAO = accountDAO;
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _accountDAO.GetAllAccountsAsync();
        }

        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _accountDAO.GetAccountByIdAsync(accountId);
        }

        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return await _accountDAO.GetAccountByUsernameAsync(username);
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _accountDAO.GetAccountByEmailAsync(email);
        }

        public async Task AddAccountAsync(Account account)
        {
            await _accountDAO.AddAccountAsync(account);
        }

        public async Task UpdateAccountAsync(Account account)
        {
            await _accountDAO.UpdateAccountAsync(account);
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            await _accountDAO.DeleteAccountAsync(accountId);
        }

        public async Task<Account?> AuthenticateAsync(string username, string password)
        {
            var account = await _accountDAO.AuthenticateUserAsync(username, password);
            return account;
        }

        public async Task UpdatePasswordAsync(int accountId, string newPassword)
        {
            var account = await _accountDAO.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                throw new Exception($"Account with ID {accountId} not found.");
            }
            await _accountDAO.UpdateAccountAsync(account);
        }   
    }
}
