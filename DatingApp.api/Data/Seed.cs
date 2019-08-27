using System.Collections.Generic;
using DatingApp.api.Models;
using Newtonsoft.Json;

namespace DatingApp.api.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }

        private void CreatePasswordHash(string password, out byte[] PasswordHash, out byte[] PasswordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        public void SeedUsers()
        {
            var UserData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(UserData);
            foreach(var user in users)
            {
                byte[] PasswordHash, PasswordSalt;
                CreatePasswordHash("password", out PasswordHash, out PasswordSalt);
                user.Hash = PasswordHash;
                user.Salt = PasswordSalt;
                user.Username = user.Username.ToLower();
                _context.Users.Add(user);
            }

            _context.SaveChanges();
        }
    }
}