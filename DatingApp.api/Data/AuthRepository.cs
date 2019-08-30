using System;
using System.Threading.Tasks;
using DatingApp.api.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.api.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.Hash, user.Salt))
            {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] hash, byte[] salt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(salt))
            {
                var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < ComputedHash.Length; i++)
                {
                    if (ComputedHash[i] != hash[i])
                    {
                        return false;
                    }
                }

            }
            return true;

        }

        public async Task<User> Register(User user, string password)
        {
            byte[] PasswordHash, PasswordSalt;
            CreatePasswordHash(password, out PasswordHash, out PasswordSalt);
            user.Hash = PasswordHash;
            user.Salt = PasswordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return true;
            }

            return false;
        }
    }
}