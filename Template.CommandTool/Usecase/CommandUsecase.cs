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
        var client = await clientFactory.CreateClientAsync(new IPEndPoint(IPAddress.Parse(host), port));

        var data = await client.ChallengeAsync();
        if (data.Length == 0)
        {
            await client.DisposeAsync();
            return null;
        }

        using var rsa = RSA.Create();
        rsa.ImportFromEncryptedPem(await File.ReadAllTextAsync(key), password);

        var token = Convert.FromHexString(Encoding.ASCII.GetString(data));
        var signData = rsa.SignData(token, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        var signature = Encoding.ASCII.GetBytes(Convert.ToHexString(signData));

        if (!await client.AuthorizeAsync(signature))
        {
            await client.DisposeAsync();
            return null;
        }

        return client;
    }
}
