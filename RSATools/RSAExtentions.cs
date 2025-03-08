using System.Security.Cryptography;

namespace WebApi04.RSATools
{
    public class RSAExtensions
    {
        public static RSA GeneratePrivateKey()  //добавил static после RSA
        {
            var key = File.ReadAllText("rsa/private_key.pem");
            var rsa = RSA.Create();
            rsa.ImportFromPem(key);

            return rsa;
        }
    }
}
