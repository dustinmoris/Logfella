using Microsoft.Extensions.Logging;

namespace Logfella.Adapters
{
    public class LoggerAdapterProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName = null) => new LoggerAdapter(categoryName);

        public void Dispose()
        {
        }
    }
}