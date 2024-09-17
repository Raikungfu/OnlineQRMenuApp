using System.IO;
using System.Security.Cryptography;

public static class KeyHelper
{
    public static RSA GetPrivateKey()
    {
        string relativePath = "Key/privateKey.pem";
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, relativePath);

        var rsa = RSA.Create();
        var privateKey = File.ReadAllText(fullPath);
        rsa.ImportFromPem(privateKey.ToCharArray());
        return rsa;
    }

    public static RSA GetPublicKey()
    {

        string relativePath = "Key/publicKey.pem";
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string fullPath = Path.Combine(basePath, relativePath);

        var rsa = RSA.Create();
        var publicKey = File.ReadAllText(fullPath);
        rsa.ImportFromPem(publicKey.ToCharArray());
        return rsa;
    }
}
