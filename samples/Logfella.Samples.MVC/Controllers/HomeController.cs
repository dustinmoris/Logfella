using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Logfella.Samples.MVC.Controllers
{
    [ApiController]
    public class HomeController : ControllerBase
    {
        // Inject an ILogger<T> object as normal.
        // The ILogger interface is somewhat limited when it comes
        // to logging to GCP, as Microsoft.Extensions.Logging
        // has a smaller set of severity levels and doesn't set
        // certain properties for Error Reporting.
        private readonly ILogger<HomeController> _logger;

        // Inject an ILogWriter to use Logfella's own abstraction
        // which is tailored towards Google Cloud Logging:
        private readonly ILogWriter _logWriter;

        public HomeController(
            ILogger<HomeController> logger,
            ILogWriter logWriter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logWriter = logWriter ?? throw new ArgumentException(nameof(logWriter));
        }

        [HttpGet("/ping")]
        public string Ping()
        {
            _logger.LogInformation("Ping");
            return "ping";
        }

        [HttpGet("/pong")]
        public string Pong()
        {
            _logWriter.Info("Pong");
            return "pong";
        }

        [HttpGet("/foo")]
        public string Foo()
        {
            // Alternatively, if you can't be bothered to inject a logger
            // into every single class across your entire application then
            // you can use the static Log instance instead.
            // Regardless which option you choose to use (ILogger vs. ILogWriter vs. Log)
            // you'll always be using the same global or per request instance under
            // the hood and therefore not miss out on features such as correlating
            // logs or pre-configured labels, severity, and more.
            // The static Log instance is mainly the preferred option for
            // functional applications in F#, where constructor DI is not that common.
            Log.Info("Foo");

            return "foo";
        }
    }
}