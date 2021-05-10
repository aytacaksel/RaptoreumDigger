using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;


namespace RaptoreumDigger
{
    public static class Utilities
    {
        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format("The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] HexAsBytes = new byte[hexString.Length / 2];

            for (int index = 0; index < HexAsBytes.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                HexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return HexAsBytes;
        }

        public static string ByteArrayToHexString(byte[] byteArray)
        {
            string result = "";

            foreach (byte b in byteArray)
                result += string.Format("{0:x2}", b);

            return result;
        }

        public static byte[] ReverseByteArrayByFours(byte[] byteArray)
        {
            byte temp;

            if (byteArray.Length % 4 != 0)
            {
                throw new ArgumentException(String.Format("The byte array length must be a multiple of 4"));
            }

            for (int index = 0; index < byteArray.Length; index += 4)
            {
                temp = byteArray[index];
                byteArray[index] = byteArray[index + 3];
                byteArray[index + 3] = byteArray[index + 2];
                byteArray[index + 2] = byteArray[index + 1];
                byteArray[index + 1] = byteArray[index + 3];
                byteArray[index + 3] = temp;
            }

            return byteArray;
        }

        public static string GenerateMerkleRoot(string Coinb1, string Coinb2, string ExtraNonce1, string ExtraNonce2, string[] MerkleNumbers)
        {
            string Coinbase = Coinb1 + ExtraNonce1 + ExtraNonce2 + Coinb2;

            byte[] Coinbasebytes = Utilities.HexStringToByteArray(Coinbase);

            SHA256 mySHA256 = SHA256.Create();
            mySHA256.Initialize();
            byte[] hashValue, hashValue2;

            
            hashValue = mySHA256.ComputeHash(Coinbasebytes);
            hashValue2 = mySHA256.ComputeHash(hashValue);

            
            foreach (string s in MerkleNumbers)
            {
                hashValue = mySHA256.ComputeHash(Utilities.HexStringToByteArray(Utilities.ByteArrayToHexString(hashValue2) + s));
                hashValue2 = mySHA256.ComputeHash(hashValue);
            }

            string MerkleRoot = Utilities.ByteArrayToHexString(Utilities.ReverseByteArrayByFours(hashValue2));

            mySHA256.Dispose();

            return MerkleRoot;
        }
     
        public static string JsonSerialize(object obj)
        {
                     
            MemoryStream ms = new MemoryStream();

                  
            DataContractJsonSerializer s = new DataContractJsonSerializer(obj.GetType());
            s.WriteObject(ms, obj);
            byte[] json = ms.ToArray();
            ms.Close();

                    
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static T JsonDeserialize<T>(string json)
        {
            T result = default(T);

                
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));

            DataContractJsonSerializer s = new DataContractJsonSerializer(typeof(T));
            result = (T)s.ReadObject(ms);
            ms.Close();
            return result;
        }
    }
}

