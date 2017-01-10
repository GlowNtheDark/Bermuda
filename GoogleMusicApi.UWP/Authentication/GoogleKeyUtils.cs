using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using System.Text;
using Windows.Security.Cryptography;
using System.Runtime.InteropServices.WindowsRuntime;

namespace GoogleMusicApi.UWP.Authentication
{
    // gpsoauth:google.py
    // URL: https://github.com/simon-weber/gpsoauth/blob/master/gpsoauth/google.py
    internal class GoogleKeyUtils
    {
        private static readonly string _b64KeyBlob =
            "BgIAAACkAABSU0ExAAQAAAEAAQD5Z676H2sHuPzPHSUMid9z1UQQjUWg1SzKBk0EH7GzMkI5hKh5bHhpitEBqddIpKR7Tptj0FDZoqkgYXJGRWjbPQR14VsUd0UreyfKmadWs0KZEsod2mVIIt4iCK+pjsLEYnmvlckceEGFMZbS5XKgndB6u26U7ZRbSb+/Vv8myg==";
        // key_from_b64
        // BitConverter has different endianness, hence the Reverse()
        public static RSAParameters KeyFromB64(string b64Key)
        {
            var decoded = Convert.FromBase64String(b64Key);
            var modLength = BitConverter.ToInt32(decoded.Take(4).Reverse().ToArray(), 0);
            var mod = decoded.Skip(4).Take(modLength).ToArray();
            var expLength = BitConverter.ToInt32(decoded.Skip(modLength + 4).Take(4).Reverse().ToArray(), 0);
            var exponent = decoded.Skip(modLength + 8).Take(expLength).ToArray();
            var rsaKeyInfo = new RSAParameters
            {
                Modulus = mod,
                Exponent = exponent
            };
            return rsaKeyInfo;
        }

        // key_to_struct
        // Python version returns a string, but we use byte[] to get the same results
        public static byte[] KeyToStruct(RSAParameters key)
        {
            byte[] modLength = { 0x00, 0x00, 0x00, 0x80 };
            var mod = key.Modulus;
            byte[] expLength = { 0x00, 0x00, 0x00, 0x03 };
            var exponent = key.Exponent;
            return DataTypeUtils.CombineBytes(modLength, mod, expLength, exponent);
        }

        // parse_auth_response
        public static Dictionary<string, string> ParseAuthResponse(string text)
        {
            return
                text.Split(new[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(line => line.Split('='))
                    .ToDictionary(parts => parts[0], parts => parts[1]);
        }

        // signature
        public static string CreateSignature(string email, string password, RSAParameters key)
        {
            /*var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            var sha1 = SHA1.Create();
            byte[] prefix = { 0x00 };
            var hash = sha1.ComputeHash(KeyToStruct(key)).Take(4).ToArray();
            var encrypted = rsa.Encrypt(Encoding.UTF8.GetBytes(email + "\x00" + password), true);
            return DataTypeUtils.CombineBytes(prefix, hash, encrypted).ToUrlSafeBase64(); */

            var rsa = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaOaepSha1);
            var keyBlob = CryptographicBuffer.DecodeFromBase64String(_b64KeyBlob);
            var publicKey = rsa.ImportPublicKey(keyBlob, CryptographicPublicKeyBlobType.Capi1PublicKey);

            byte[] prefix = { 0x00 };
            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha1);
            var hash = hashAlgorithm.HashData(KeyToStruct(key).AsBuffer()).ToArray().Take(4).ToArray();

            var encrypted =
                CryptographicEngine.Encrypt(publicKey, Encoding.UTF8.GetBytes(email + "\x00" + password).AsBuffer(),
                    null).ToArray();
            return DataTypeUtils.ToUrlSafeBase64(DataTypeUtils.CombineBytes(prefix, hash, encrypted));
        }
    }
}