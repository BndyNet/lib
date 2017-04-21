using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Net.Bndy
{
    public class Cryptography
    {
        /// <summary>
        /// Hashes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="salt">The salt.</param>
        /// <returns>System.String.</returns>
        public static string Hash(string text, string salt)
        {
            var saltBytes = Encoding.UTF8.GetBytes(salt + "--" + text);
            var hashBytes = System.Security.Cryptography.SHA256.Create().ComputeHash(saltBytes);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
