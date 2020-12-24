using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SymmetricAlgorithmAES
{
    public static class AESInECB
    {
        // Alarm encription
        public static byte[] EncryptAlarm(Alarm alarm, string secretKey)
        {
            byte[] AlarmInByteArr = ObjectToByteArray(alarm);
            byte[] encryptedAlarm = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform encryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(AlarmInByteArr, 0, AlarmInByteArr.Length);
                    cryptoStream.FlushFinalBlock();
                    encryptedAlarm = memoryStream.ToArray();
                }
            }

            return encryptedAlarm;
        }

        // Alarm decryption
        public static Alarm DecryptAlarm(byte[] AlarmInByteArr, string secretKey)
        {
            byte[] decryptedAlarmInByteArr = null;
            Alarm decryptedAlarm = null;
            
            AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform decryptTransform = aesCryptoServiceProvider.CreateDecryptor();

            using (MemoryStream memoryStream = new MemoryStream(AlarmInByteArr))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptTransform, CryptoStreamMode.Read))
                {
                    decryptedAlarmInByteArr = new byte[AlarmInByteArr.Length];
                    cryptoStream.Read(decryptedAlarmInByteArr, 0, decryptedAlarmInByteArr.Length);
                }
            }

            decryptedAlarm = (Alarm)ByteArrayToObject(decryptedAlarmInByteArr);

            return decryptedAlarm;
        }

        // Integer encryption
        public static byte[] EncriptInteger(int numToEncrypt, string secretKey)
        {
            byte[] numToEncryptInByteArr = BitConverter.GetBytes(numToEncrypt);
            byte[] encryptedNum = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform encryptTransform = aesCryptoProvider.CreateEncryptor();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(numToEncryptInByteArr, 0, numToEncryptInByteArr.Length);
                    cryptoStream.FlushFinalBlock();
                    encryptedNum = memoryStream.ToArray();
                }
            }

            return encryptedNum;
        }

        // Integer Decryption
        public static int DecryptInteger(byte[] numToDecryptInByteArr, string secretKey)
        {
            byte[] decryptedNumInByteArr = null;
            int decryptedNum = -1;

            AesCryptoServiceProvider aesCryptoServiceProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            ICryptoTransform decryptTransform = aesCryptoServiceProvider.CreateDecryptor();

            using (MemoryStream memoryStream = new MemoryStream(numToDecryptInByteArr))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptTransform, CryptoStreamMode.Read))
                {
                    decryptedNumInByteArr = new byte[numToDecryptInByteArr.Length];
                    cryptoStream.Read(decryptedNumInByteArr, 0, decryptedNumInByteArr.Length);
                }
            }

            return decryptedNum = BitConverter.ToInt32(decryptedNumInByteArr, 0);                                    
        }

        // Convert an object to a byte array
        public static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);

            return ms.ToArray();
        }

        // Convert a byte array to an Object
        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);

            return obj;
        }
    }
}
