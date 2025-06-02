using HotelManagementSystem.Data;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HotelManagementSystem.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<User?> RegisterAsync(RegisterViewModel model);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }

    public class UserService : IUserService
    {
        private readonly HotelDbContext _context;

        public UserService(HotelDbContext context)
        {
            _context = context;
        }        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            if (!VerifyPassword(password, user.Password))
                return null;

            if (IsLegacyPassword(user.Password))
            {
                user.Password = HashPassword(password);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }public async Task<User?> RegisterAsync(RegisterViewModel model)
        {
            if (await EmailExistsAsync(model.Email))
                return null;

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = HashPassword(model.Password),
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                Role = "Guest",
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }        public string HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(32);
                
                byte[] hashBytes = new byte[48];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 32);
                
                return Convert.ToBase64String(hashBytes);
            }
        }        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                if (TryVerifyPBKDF2(password, hashedPassword))
                    return true;
                
                return VerifyLegacySHA256(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        private bool TryVerifyPBKDF2(string password, string hashedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);
                
                
                if (hashBytes.Length != 48)
                    return false;
                
                
                byte[] salt = new byte[16];
                Array.Copy(hashBytes, 0, salt, 0, 16);
                
                
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
                {
                    byte[] hash = pbkdf2.GetBytes(32);
                    
                    
                    for (int i = 0; i < 32; i++)
                    {
                        if (hashBytes[i + 16] != hash[i])
                            return false;
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }        private bool VerifyLegacySHA256(string password, string hashedPassword)
        {
            try
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                    string computedHash = Convert.ToBase64String(bytes);
                    return computedHash == hashedPassword;
                }
            }
            catch
            {
                return false;
            }
        }

        private bool IsLegacyPassword(string hashedPassword)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(hashedPassword);
                
                return hashBytes.Length == 32;
            }
            catch
            {
                return false;
            }
        }
    }
}
