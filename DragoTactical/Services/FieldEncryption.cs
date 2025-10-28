using System;
using System.Security.Cryptography;
using System.Text;

namespace DragoTactical.Services
{
    public static class EncryptionConfig
    {
        private static byte[]? _keyBytes;

        public static void SetKeyFromString(string? key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _keyBytes = null;
                return;
            }

            if (TryFromBase64(key, out var b64))
            {
                _keyBytes = EnsureKeySize(b64);
                return;
            }

            if (TryFromHex(key, out var hex))
            {
                _keyBytes = EnsureKeySize(hex);
                return;
            }

            using var sha = SHA256.Create();
            _keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(key));
        }

        public static bool IsEnabled => _keyBytes != null && _keyBytes.Length == 32;

        public static byte[] GetKeyOrThrow()
        {
            if (!IsEnabled)
            {
                throw new InvalidOperationException("Field encryption key is not configured.");
            }
            return _keyBytes!;
        }

        private static bool TryFromBase64(string input, out byte[] bytes)
        {
            try
            {
                bytes = Convert.FromBase64String(input);
                return true;
            }
            catch
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }

        private static bool TryFromHex(string input, out byte[] bytes)
        {
            try
            {
                input = input.Replace(" ", string.Empty).Replace("-", string.Empty);
                if (input.Length % 2 != 0)
                {
                    bytes = Array.Empty<byte>();
                    return false;
                }
                bytes = new byte[input.Length / 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(input.Substring(i * 2, 2), 16);
                }
                return true;
            }
            catch
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }

        private static byte[] EnsureKeySize(byte[] key)
        {
            if (key.Length == 32) return key;
            using var sha = SHA256.Create();
            return sha.ComputeHash(key);
        }
    }

    public static class FieldEncryption
    {
        private const string Prefix = "enc:v1:";

        public static string? EncryptString(string? plaintext)
        {
            if (!EncryptionConfig.IsEnabled) return plaintext;
            if (string.IsNullOrEmpty(plaintext)) return plaintext;

            var key = EncryptionConfig.GetKeyOrThrow();
            var nonce = RandomNumberGenerator.GetBytes(12);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[16];

            using var aes = new AesGcm(key, 16);
            aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

            byte[] combined = new byte[12 + 16 + ciphertext.Length];
            Buffer.BlockCopy(nonce, 0, combined, 0, 12);
            Buffer.BlockCopy(tag, 0, combined, 12, 16);
            Buffer.BlockCopy(ciphertext, 0, combined, 28, ciphertext.Length);

            string b64 = Convert.ToBase64String(combined);
            return Prefix + b64;
        }

        public static string? DecryptString(string? stored)
        {
            if (!EncryptionConfig.IsEnabled) return stored;
            if (string.IsNullOrEmpty(stored)) return stored;

            if (!stored.StartsWith(Prefix, StringComparison.Ordinal))
            {
                return stored;
            }

            var b64 = stored.Substring(Prefix.Length);
            byte[] combined = Convert.FromBase64String(b64);
            if (combined.Length < 28) return stored;

            var nonce = new byte[12];
            var tag = new byte[16];
            var ciphertext = new byte[combined.Length - 28];
            Buffer.BlockCopy(combined, 0, nonce, 0, 12);
            Buffer.BlockCopy(combined, 12, tag, 0, 16);
            Buffer.BlockCopy(combined, 28, ciphertext, 0, ciphertext.Length);

            var key = EncryptionConfig.GetKeyOrThrow();
            byte[] plaintext = new byte[ciphertext.Length];
            try
            {
                using var aes = new AesGcm(key, 16);
                aes.Decrypt(nonce, ciphertext, tag, plaintext);
                return Encoding.UTF8.GetString(plaintext);
            }
            catch
            {
                return stored;
            }
        }

        public static bool LooksEncrypted(string? value)
        {
            return !string.IsNullOrEmpty(value) && value.StartsWith(Prefix, StringComparison.Ordinal);
        }
    }
}


