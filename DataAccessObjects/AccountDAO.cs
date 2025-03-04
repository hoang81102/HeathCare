using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class AccountDAO : SingletonBase<Account>
    {
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .AsNoTracking()
                .Include(a => a.Role) 
                .ToListAsync();
        }


        // 🔹 Lấy tài khoản theo ID
        public async Task<Account?> GetAccountByIdAsync(int accountId)
        {
            return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.AccountId == accountId);
        }

        // 🔹 Lấy tài khoản theo Username
        public async Task<Account?> GetAccountByUsernameAsync(string username)
        {
            return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Username == username);
        }

        public async Task<Account?> GetAccountByEmailAsync(string email)
        {
            return await _context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Email == email);
        }

        // 🔹 Xác thực tài khoản khi đăng nhập
        public async Task<Account?> AuthenticateUserAsync(string loginInput, string password)
        {
            string encryptedPassword = EncryptPassword(password);
            return await _context.Accounts
                                 .AsNoTracking()
                                 .SingleOrDefaultAsync(a =>
                                     (a.Username == loginInput || a.Email == loginInput) && a.Password == encryptedPassword);
        }


        // 🔹 Thêm tài khoản mới
        public async Task AddAccountAsync(Account account)
        {
            account.AccountId = 0;
            account.Password = EncryptPassword(account.Password);

            await _context.Accounts.AddAsync(account);
            await _context.SaveChangesAsync();

            _context.Entry(account).State = EntityState.Detached;
        }


        // 🔹 Cập nhật tài khoản
        public async Task UpdateAccountAsync(Account account)
        {
            var existingAccount = await _context.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

            if (existingAccount == null)
            {
                throw new Exception("Account not found!");
            }

            if (string.IsNullOrEmpty(account.Password))
            {
                account.Password = existingAccount.Password;
            }
            else
            {
                account.Password = EncryptPassword(account.Password);
            }

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            _context.Entry(account).State = EntityState.Detached;
        }


        // 🔹 Xóa tài khoản
        public async Task DeleteAccountAsync(int accountId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var account = await _context.Accounts
                    .Include(a => a.Bookings)
                    .Include(a => a.Caregivers)
                    .Include(a => a.Elders)
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account != null)
                {
                    _context.Bookings.RemoveRange(account.Bookings);
                    _context.Caregivers.RemoveRange(account.Caregivers);
                    _context.Elders.RemoveRange(account.Elders);
                    _context.Accounts.Remove(account);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                throw new Exception($"❌ Lỗi khi xóa tài khoản {accountId}: {ex.Message}", ex);
            }
        }

        // 🔹 Cập nhật mật khẩu tài khoản
        public async Task UpdatePasswordAsync(int accountId, string newPassword)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                throw new Exception($"Tài khoản với ID {accountId} không tồn tại.");
            }

            account.Password = EncryptPassword(newPassword);

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            _context.Entry(account).State = EntityState.Detached;
        }

        // 🔹 Lấy danh sách tài khoản theo RoleId
        public async Task<List<Account>> GetAccountsByRoleAsync(int roleId)
        {
            return await _context.Accounts.AsNoTracking()
                                          .Where(a => a.RoleId == roleId)
                                          .ToListAsync();
        }

        // 🔹 Hàm mã hóa mật khẩu (MD5)
        private string EncryptPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = md5.ComputeHash(bytes);
                StringBuilder sb = new StringBuilder();
                foreach (var b in hash)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
