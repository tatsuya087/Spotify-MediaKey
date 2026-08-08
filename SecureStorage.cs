using System;
using System.Security.Cryptography;
using System.Text;

namespace SpotifyMediaKey
{
    public static class SecureStorage
    {
        public static string? Protect(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return null;

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }

        public static string? Unprotect(string? encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText)) return null;

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plainBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}