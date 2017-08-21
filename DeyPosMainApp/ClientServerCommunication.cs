using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UVCE.ME.IEEE.Apps.DeyPosMainApp.Common;

namespace UVCE.ME.IEEE.Apps.DeyPosMainApp
{

    // https://msdn.microsoft.com/en-us/library/system.security.cryptography.ecdiffiehellmancng(v=vs.110).aspx


    class ClientServerCommunication
    {

    }

    public class AliceUser
    {
        public byte[] alicePublicKey;
        public byte[] aliceKey;

        public AliceUser()
        {
            using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
            {

                alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                alice.HashAlgorithm = CngAlgorithm.Sha256;
                alicePublicKey = alice.PublicKey.ToByteArray();
            }
        }

        public ServerBob Server {get; set;}

        public string EncryptMessage(string message)
        {
            using (ECDiffieHellmanCng alice = new ECDiffieHellmanCng())
            {

                alice.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                alice.HashAlgorithm = CngAlgorithm.Sha256;
                alicePublicKey = alice.PublicKey.ToByteArray();
                CngKey k = CngKey.Import(Server.bobPublicKey, CngKeyBlobFormat.EccPublicBlob);
                aliceKey = alice.DeriveKeyMaterial(CngKey.Import(Server.bobPublicKey, CngKeyBlobFormat.EccPublicBlob));
                byte[] encryptedMessage = null;
                byte[] iv = null;
                Send(aliceKey, message, out encryptedMessage, out iv);
               // Server.Receive(encryptedMessage, iv);
                return Utility.ToHex(encryptedMessage,true);
            }
        }

        private void Send(byte[] key, string secretMessage, out byte[] encryptedMessage, out byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = key;
                iv = aes.IV;

                // Encrypt the message
                using (MemoryStream ciphertext = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ciphertext, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    byte[] plaintextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cs.Write(plaintextMessage, 0, plaintextMessage.Length);
                    cs.Close();
                    encryptedMessage = ciphertext.ToArray();
                }
            }
        }

    }
    public class ServerBob
    {
        public byte[] bobPublicKey;
        private byte[] bobKey;

        public AliceUser User { get; set; }
        public ServerBob()
        {
            using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
            {

                bob.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                bob.HashAlgorithm = CngAlgorithm.Sha256;
                bobPublicKey = bob.PublicKey.ToByteArray();
            }
        }

        public void setKeysWithUser()
        {
            using (ECDiffieHellmanCng bob = new ECDiffieHellmanCng())
            {
                bobKey = bob.DeriveKeyMaterial(CngKey.Import(User.alicePublicKey, CngKeyBlobFormat.EccPublicBlob));
            }
        }

        public void Receive(byte[] encryptedMessage, byte[] iv)
        {
            using (Aes aes = new AesCryptoServiceProvider())
            {
                aes.Key = bobKey;
                aes.IV = iv;
                // Decrypt the message
                using (MemoryStream plaintext = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(plaintext, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                        cs.Close();
                        string message = Encoding.UTF8.GetString(plaintext.ToArray());
                        Console.WriteLine(message);
                    }
                }
            }
        }

    }
}
