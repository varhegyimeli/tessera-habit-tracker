namespace Tessera.Tests.Integration.RestApi;

using RestSharp;

/// <summary>
/// Base class for REST API tests using RestSharp.
/// These tests require Tessera.Api to be running locally on port 7184.
/// </summary>
public abstract class RestSharpApiTests : IDisposable
{
    protected readonly RestClient Client;

    protected RestSharpApiTests()
    {
        var options = new RestClientOptions("https://localhost:7184")
        {
            RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
        };
        Client = new RestClient(options);
    }

    public virtual void Dispose()
    {
        Client?.Dispose();
    }
}