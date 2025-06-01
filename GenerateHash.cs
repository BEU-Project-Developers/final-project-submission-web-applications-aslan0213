using System;
using System.Security.Cryptography;
using System.Text;

public class HashGenerator
{
    public static void Main()
    {
        string password = "admin123";
        
        // Generate SHA256 hash (legacy format)
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
            string sha256 = Convert.ToBase64String(bytes);
            Console.WriteLine($"SHA256 Hash: {sha256}");
        }
        
        // Generate PBKDF2 hash (new format)
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
            
            string pbkdf2Hash = Convert.ToBase64String(hashBytes);
            Console.WriteLine($"PBKDF2 Hash: {pbkdf2Hash}");
        }
    }
}
