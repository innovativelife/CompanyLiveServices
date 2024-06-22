
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InnovativeLife.CloudFunctionHandler;
public class SimpleOptions : AuthenticationSchemeOptions
{
    public string DisplayMessage { get; set; } = "";
}

public class SimpleAuthHandler : AuthenticationHandler<SimpleOptions>
{
    private readonly ILogger<SimpleAuthHandler> _logger;
    public SimpleAuthHandler(IOptionsMonitor<SimpleOptions> options, ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    { 
        _logger = logger.CreateLogger<SimpleAuthHandler>();
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }
}