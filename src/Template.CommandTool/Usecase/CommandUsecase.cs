namespace Template.CommandTool.Usecase;

using System.Security.Cryptography;

using Template.CommandTool.Components;

public sealed class CommandUsecase
{
    private readonly CommandClientFactory clientFactory;

    public CommandUsecase(CommandClientFactory clientFactory)
    {
        this.clientFactory = clientFactory;
    }

    public async ValueTask<CommandClient?> CreateClientWithAuthorizeAsync(string host, int port, string key, string password)
    {
        if (!File.Exists(key))
        {
            return null;
        }

        var client = await clientFactory.CreateClientAsync(new DnsEndPoint(host, port));

        var authorized = false;
        try
        {
            var data = await client.ChallengeAsync();
            if (data.Length == 0)
            {
                return null;
            }

            using var rsa = RSA.Create();
            try
            {
                rsa.ImportFromEncryptedPem(await File.ReadAllTextAsync(key), password);
            }
            catch (CryptographicException)
            {
                return null;
            }

            var token = Convert.FromHexString(Encoding.ASCII.GetString(data));
            var signData = rsa.SignData(token, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            var signature = Encoding.ASCII.GetBytes(Convert.ToHexString(signData));

            authorized = await client.AuthorizeAsync(signature);
            return authorized ? client : null;
        }
        finally
        {
            if (!authorized)
            {
                await client.DisposeAsync();
            }
        }
    }
}
