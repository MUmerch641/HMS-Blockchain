using System;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainPatientData.Services
{
    /// <summary>
    /// Blockchain Service - Handles SHA256 hashing for blockchain simulation
    /// Provides cryptographic functions for ensuring data immutability
    /// </summary>
    public class BlockchainService
    {
        /// <summary>
        /// Generates SHA256 hash for given input string
        /// </summary>
        /// <param name="input">String to be hashed</param>
        /// <returns>64-character hexadecimal hash</returns>
        public string GenerateHash(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            using (var sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);
                
                // Convert bytes to hexadecimal string
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                
                return sb.ToString().ToUpper();
            }
        }

        /// <summary>
        /// Generates hash for bill data (Patient ID + Description + Amount + Timestamp)
        /// This is used for individual bill verification
        /// </summary>
        public string GenerateBillHash(int patientId, string description, decimal amount, DateTime timestamp)
        {
            string data = $"{patientId}-{description}-{amount}-{timestamp:yyyy-MM-dd HH:mm:ss}";
            return GenerateHash(data);
        }

        /// <summary>
        /// Generates BLOCKCHAIN hash with previous block hash (TRUE BLOCKCHAIN CHAINING)
        /// This creates the immutable chain where each block depends on the previous block
        /// </summary>
        public string GenerateBlockchainHash(int patientId, string description, decimal amount, DateTime timestamp, string previousHash)
        {
            // Combine bill data WITH previous block's hash
            // This creates the blockchain chain - changing any previous block breaks all subsequent blocks
            string data = $"{previousHash}-{patientId}-{description}-{amount}-{timestamp:yyyy-MM-dd HH:mm:ss}";
            return GenerateHash(data);
        }

        /// <summary>
        /// Generates hash with previous block hash (blockchain chaining) - Generic version
        /// </summary>
        public string GenerateBlockHash(string currentData, string previousHash)
        {
            string combinedData = $"{previousHash}{currentData}";
            return GenerateHash(combinedData);
        }

        /// <summary>
        /// Validates if a hash matches the original data
        /// </summary>
        public bool ValidateHash(string data, string hash)
        {
            string computedHash = GenerateHash(data);
            return computedHash.Equals(hash, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Simulates mining (for demonstration purposes)
        /// Finds a hash with specified number of leading zeros
        /// </summary>
        public (string hash, int nonce) MineBlock(string data, int difficulty = 2)
        {
            int nonce = 0;
            string targetPrefix = new string('0', difficulty);
            string hash;

            do
            {
                string dataWithNonce = $"{data}{nonce}";
                hash = GenerateHash(dataWithNonce);
                nonce++;
            }
            while (!hash.StartsWith(targetPrefix) && nonce < 100000);

            return (hash, nonce - 1);
        }

        /// <summary>
        /// Genesis block hash (first block in the blockchain)
        /// </summary>
        public string GetGenesisHash()
        {
            return "0000000000000000000000000000000000000000000000000000000000000000";
        }
    }
}
