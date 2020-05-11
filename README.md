# Logfella

Logfella is a small logging library specifically designed to work well with [Google Cloud Logging](https://cloud.google.com/logging/).

If you are running a .NET Core or ASP.NET Core application inside GCP and want to write structured logs into [Google Cloud Logging](https://cloud.google.com/logging/) then this library will offer you a more advanced integration point than .NET Core's out of the box `ILogger` providers.

Logfella writes structured logs directly to stdout in a specific JSON format which GCP's log analyzer can natively understand and process accordingly. Logfella offers support for the full range of Google's severity levels, automatic propagation of exceptions into [Google Error Reporting](https://cloud.google.com/error-reporting), full integration with `Microsoft.Extensions.Logging` libraries and much more.

#### NOTE:

This library is an early version mostly intended for personal use and neither the docs nor the APIs have been fully fledged out for wider exposure.

That being said, it has been written with flexibility and extensibility 100% in mind and hopefully will be able to suit more than just my immediate and personal use case in the future. It's been open sourced under Apache 2.0 and feedback and/or PRs are more than welcome.

## Benefits of Logfella

- Write structured logs into [Google Cloud Logging](https://cloud.google.com/logging/) when hosted in GCP
- Automatic error propagation into [Google Error Reporting](https://cloud.google.com/error-reporting) when exceptions are being logged
- Support of custom log labels for advanced log filtering
- Support of the full range of severity levels of GCP (9 in total)
- Automatically sets the correct timestamp and service context information for all Google Cloud Logs
- Seamlessly integrates with .NET Core's logging providers (`ILogger` and `ILoggerProvider`)
- Optimised for C# and F# applications so that logging feels natural from an object-oriented as well as from a functional architecture
- `ILogWriter` can be injected either via DI or called directly from the static `Log` instance
- 100% customisable and extensible
- Uses .NET Core's new `System.Text.Json` library for faster JSON serialisation
- Supports log aggregation via an optional correlation ID which can be set on a `LogWriter` instance. The additional `PerRequestLogWriterMiddleware` makes it extremely easy to compute (or inherit) a correlation ID for all logs of a given HTTP request pipeline 
    
 ## How it works
 
 In its simplest form one has to create a logger object of type `ILogWriter` in order to start writing logs. Because this library is mainly designed for writing logs to Google Cloud Logging there are only four default loggers which come out of the box:
 
 - `GoogleCloudLogWriter`
 - `ConsoleLogWriter`
 - `MultiLogWriter`
 - `NullLogWriter`
 
 The `GoogleCloudLogWriter` is the main implementation and why you're probably looking at using Logfella. The `ConsoleLogWriter` is a simple console writer which can be swapped for the `GoogleCloudLogWriter` during development. The `MultiLogWriter` is a convenience class to configure multiple log writers simultaneously. Lastly the `NullLogWriter` is the default implementation which the static `Log` instance gets configured with in order to avoid a `NullReferenceException` when the static log methods are being used (mainly from a functional F# architecture, since constructor DI is not idiomatic in functional programming).
 
 ### GoogleCloudLogWriter
 
 The `GoogleCloudLogWriter` is a relatively unexciting console log writer, which outputs log entries in a certain JSON format into `stdout` which Google's log analyzer knows how to parse and interpret correctly.
 
 The benefit of this architecture is that writing logs into GCP has almost no impact on latency, doesn't require any additional network calls and doesn't require batching like most other logging sinks. Logs arrive in the Google Cloud Logging console almost in near real-time with only fractions of a second to a couple seconds of delay in most cases.
 
 The `GoogleCloudLogWriter` can be configured as following:
 
 ```csharp
// Create a new instance of ILogWriter:

var appName = "SampleApp";
var appVersion = "v1.0.0";

var logger =
    new GoogleCloudLogWriter(
        Severity.Debug,
        appName,
        appVersion,
        false,
        new Dictionary<string, string>
        {
            { "appName", appName },
            { "appVersion", appVersion },
            { "loggerType", "global" },
            { "foo", "bar" }
        });

// Write logs:

logger.Info("This is my first log entry.");
```

Currently the `GoogleCloudLogWriter` can be configured with the following settings:

| Parameter | Default Value | Description |
| :-------- | :------------ | :---------- |
| `severity` | -- | The minimum severity level for writing logs. |
| `serviceName` | `null` | The `serviceContext.service` property which is required by Google Cloud Error Reporting to capture error entries. |
| `serviceVersion` | `null` | The `serviceContext.version` property used for Google Error Reporting. |
| `useGoogleCloudTimestamp` | `false` | If `true` then log entries will not be decorated with the `timestamp` property and GCP will set its own `timestamp` when the log entry arrives. |
| `labels` | `null` | Additional labels to be set on every log entry. |
| `correlationIdKey` | `correlationId` | A string value setting the property name of an optional correlation ID. |
| `correlationId` | `string.Empty` | An optional correlation ID by which logs can be aggregated. |

The `serviceName` and `serviceVersion` are only used for error reporting and will not appear in the structured logs of non error messages. For more information please check out the [Google Cloud Error Reporting formatting docs](https://cloud.google.com/error-reporting/docs/formatting-error-messages).
 
 Use `labels` to decorate all log entries with useful data which can help to meaningfully filter and aggregate multiple log entries (e.g. set a label for the application version in order to accurately attribute logs to a certain deployment).
 
 The `severity` parameter must be any value of Google Cloud's severity levels:
 
 | Severity Level | Value | Description |
 | :------------- | :---- | :---------- |
 | `Default` | 0 | The log entry has no assigned severity level. |
 | `Debug` | 100 | Debug or trace information. |
 | `Info` | 200 | Routine information, such as ongoing status or performance. |
 | `Notice` | 300 | Normal but significant events, such as start up, shut down, or a configuration change. |
 | `Warning` | 400 | Warning events might cause problems. |
 | `Error` | 500 | Error events are likely to cause problems. |
 | `Critical` | 600 | Critical events cause more severe problems or outages. |
 | `Alert` | 700 | A person must take an action immediately. |
 | `Emergency` | 800 | One or more systems are unusable. |
 
 For further information on `correlationIdKey` and `correlationId` please read the [ASP.NET Core](#using-with-aspnet-core) section of this document.
 
 ## Using with Microsoft.Extensions.Logging
 
 All it takes to configure a .NET Core application to use Logfella when creating new `ILogger` or `ILogger<T>` instances from the `Microsoft.Extensions.Logging` namespace is to register the Logfella adapter during application startup:
 
 ```csharp
Host.CreateDefaultBuilder(args)
    .UseLogfella()
    .ConfigureWebHost(
        webBuilder =>
        {
            webBuilder
                .UseKestrel()
                .UseStartup<Startup>();
        })
    .Build()
    .Run(); 
```
 
 ## Using with ASP.NET Core
 
 More documentation coming soon.
 
 #### C# Example
 
 More documentation coming soon, but for now check out the sample application inside `/samples`.
 
 #### F# Example
 
More documentation coming soon, but for now check out the sample application inside `/samples`.
 
 ## Custom LogWriters
 
 Coming soon.
 
 ## License

 [Apache 2.0](https://github.com/dustinmoris/Logfella/master/LICENSE)