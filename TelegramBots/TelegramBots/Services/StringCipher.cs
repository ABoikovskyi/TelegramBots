using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TelegramBots.Services
{
	public static class StringCipher
	{
		private static readonly byte[] InitVectorBytes = Encoding.ASCII.GetBytes("tu89geji340t89u2");
		private const int KeySize = 256;
		private const string EncryptionKey = "45YWeaGACdUnKNsQ";

		public static string EncryptPost(int postId)
		{
			return Encrypt(postId.ToString());
		}

		public static string Encrypt(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			using (var password = new PasswordDeriveBytes(EncryptionKey, null))
			{
				var keyBytes = password.GetBytes(KeySize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, InitVectorBytes))
					{
						using (var memoryStream = new MemoryStream())
						{
							using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
							{
								cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
								cryptoStream.FlushFinalBlock();
								var cipherTextBytes = memoryStream.ToArray();
								return Convert.ToBase64String(cipherTextBytes);
							}
						}
					}
				}
			}
		}

		public static int DecryptPost(string postIdStr)
		{
			return Convert.ToInt32(Decrypt(postIdStr));
		}

		public static string Decrypt(string cipherText)
		{
			var cipherTextBytes = Convert.FromBase64String(cipherText);
			using (var password = new PasswordDeriveBytes(EncryptionKey, null))
			{
				var keyBytes = password.GetBytes(KeySize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.Mode = CipherMode.CBC;
					using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, InitVectorBytes))
					{
						using (var memoryStream = new MemoryStream(cipherTextBytes))
						{
							using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
							{
								var plainTextBytes = new byte[cipherTextBytes.Length];
								var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
								return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
							}
						}
					}
				}
			}
		}
	}
}