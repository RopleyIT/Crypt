using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Crypt
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length != 3)
                {
                    Console.WriteLine("Usage: crypt -d|e password path");
                    return;
                }

                string pass = args[1];
                string path = args[2];
                bool encrypt = args[0] == "-e";
                bool decrypt = args[0] == "-d";

                if (!encrypt && !decrypt)
                {
                    Console.WriteLine("Neither '-e' nor '-d' option specified");
                    return;
                }

                if (pass.Length < 4)
                {
                    Console.WriteLine("Password must be four or more characters");
                    return;
                }

                if (!File.Exists(path))
                {
                    Console.WriteLine($"File {path} does not exist");
                    return;
                }

                string tmpPath = Path.GetTempFileName();
                if (encrypt)
                {
                    byte[] hash = GetHashSha256(path);
                    using FileStream outFile = new FileStream
                        (tmpPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    using FileStream inFile = File.OpenRead(path);
                    AesEncrypt(pass, hash, inFile, outFile);
                    inFile.Dispose();
                    outFile.Dispose();

                    // Now copy the encrypted file back into its original path

                    File.Copy(tmpPath, path, true);
                    File.Delete(tmpPath);
                }
                else if(decrypt)
                {
                    using FileStream outFile = new FileStream
                        (tmpPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    using FileStream inFile = File.OpenRead(path);
                    var hash = AesDecrypt(pass, inFile, outFile);
                    inFile.Dispose();
                    outFile.Dispose();
                    var fileHash = GetHashSha256(tmpPath);
                    if (!CompareByteArrays(hash, fileHash))
                        throw new ArgumentException("Password incorrect");

                    // Now copy the decrypted file back into its original path

                    File.Copy(tmpPath, path, true);
                    File.Delete(tmpPath);
                }
            }
            catch(Exception x)
            {
                Console.WriteLine("Unable to encrypt/decrypt file: " + x.Message);
            }
        }

        private static bool CompareByteArrays(byte[] h1, byte[] h2)
        {
            return
                h1 != null &&
                h2 != null &&
                h1.Length == h2.Length &&
                h1.SequenceEqual(h2);
        }

        /// <summary>
        /// Generate the SHA256 hash value for the file
        /// with the specified path. The path is assumed
        /// to have already been checked for readability
        /// and for existence.
        /// </summary>
        /// <param name="filename">The path to the file</param>
        /// <returns>The 32 bytes in the 256 bit hash value
        /// </returns>
        
        private static byte[] GetHashSha256(string filename)
        {
            SHA256 Sha256 = SHA256.Create();
            using FileStream stream = File.OpenRead(filename);
            return Sha256.ComputeHash(stream);
        }

        /// <summary>
        /// Create a new initialisation vector of a given length
        /// </summary>
        /// <param name="length">The desired vector length</param>
        /// <returns>The byte array with the new IV</returns>
        
        private static byte[] GenerateUniqueIV(int length)
        {
            var seed = DateTimeOffset.UtcNow.Ticks;
            Random rnd = new Random((int)seed);
            var iv = new byte[length];
            rnd.NextBytes(iv);
            return iv;
        }

        /// <summary>
        /// Apply the Aes encryption to data taken from the
        /// input stream and sent to the output stream
        /// </summary>
        /// <param name="pass">Password for encryption</param>
        /// <param name="hash">Hash value for file</param>
        /// <param name="inFile">Input unencrypted stream</param>
        /// <param name="outFile">Output encrypted stream</param>
        
        private static void AesEncrypt
            (string pass, byte[] hash, Stream inFile, Stream outFile)
        {
            // Set the AES key using a default byte
            // array and the characters from the password

            byte[] key = 
            { 
                0x31, 0x52, 0x74, 0x96, 0xB8, 0xDA, 0xFC, 0x0E, 
                0x42, 0x61, 0x83, 0xA5, 0xC7, 0xE9, 0x0B, 0xFD 
            };

            for (int i = 0; i < pass.Length && i < key.Length; i++)
                key[i] ^= (byte)(pass[i]);

            byte[] iv = GenerateUniqueIV(16);
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            // Store Initialisation Vector at beginning of file.
            // Needed for decryption.

            outFile.Write(iv, 0, iv.Length);

            // Encrypt the remainder of the file
                
            using CryptoStream cryptStream = new CryptoStream
            (
                outFile,
                aes.CreateEncryptor(),
                CryptoStreamMode.Write
            );

            // First write the hash value

            cryptStream.Write(hash, 0, 32);

            // Now transfer the input file
            // data to the output stream

            int bufferLength;
            byte[] buffer = new byte[16 * 1024];
            while ((bufferLength = inFile.Read(buffer, 0, buffer.Length)) > 0)
                cryptStream.Write(buffer, 0, bufferLength);
        }

        private static byte[] AesDecrypt
           (string pass, Stream inFile, Stream outFile)
        {
            byte[] hash = new byte[32];

            // Set the AES key using a default byte
            // array and the characters from the password

            byte[] key =
            {
                0x31, 0x52, 0x74, 0x96, 0xB8, 0xDA, 0xFC, 0x0E,
                0x42, 0x61, 0x83, 0xA5, 0xC7, 0xE9, 0x0B, 0xFD
            };
            for (int i = 0; i < pass.Length && i < key.Length; i++)
                key[i] ^= (byte)(pass[i]);

            // Read the initialisation vector bytes
            // from the beginning of the file
            
            byte[] iv = new byte[16];
            inFile.Read(iv, 0, iv.Length);

            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            // Encrypt the remainder of the file

            using CryptoStream cryptStream = new CryptoStream
            (
                inFile,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read
            );

            // First obtain the hash value

            cryptStream.Read(hash, 0, 32);

            // Now transfer the input file
            // data to the output stream

            int bufferLength;
            byte[] buffer = new byte[16 * 1024];
            while ((bufferLength = cryptStream.Read(buffer, 0, buffer.Length)) > 0)
                outFile.Write(buffer, 0, bufferLength);
            return hash;
        }
    }
}
