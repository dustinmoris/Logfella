# Logfella

![NuGet Badge](http://buildstats.info/nuget/logfella)

Logfella is a small logging library specifically designed to work well with [Google Cloud Logging](https://cloud.google.com/logging/).

If you are running a .NET Core or ASP.NET Core application inside GCP and want to write structured logs into [Google Cloud Logging](https://cloud.google.com/logging/) then this library will offer you a more complete integration point than .NET Core's out of the box `ILogger` providers.

Logfella writes structured logs directly to stdout in a specific JSON format which GCP's log analyzer can natively understand and process accordingly. Logfella offers support for the full range of [Google's severity levels](#severity-levels), automatic propagation of exceptions into [Google Error Reporting](https://cloud.google.com/error-reporting), full integration with `Microsoft.Extensions.Logging` libraries and much more.

#### NOTE:

This library is an early version mostly built for personal use and neither the docs nor the APIs have been fully tested or fine tuned for a more global exposure.

That being said, it has been written with flexibility and extensibility 100% in mind and hopefully will be able to suit more than just my immediate and personal use case. It's been open sourced under Apache 2.0 and feedback and/or PRs are more than welcome.

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
- Supports log aggregation via an optional correlation ID which can be set on a `LogWriter` instance. The additional `LogWriterPerRequestMiddleware` makes it extremely easy to compute (or inherit) a correlation ID for all logs of a given HTTP request pipeline
- HTTP request data can be logged with each log entry as part of the ASP.NET Core request pipeline
    
## Overview
 
In order to start logging with Logfella one has to create a logger object of type `ILogWriter`. By default this library comes with four loggers out of the box:
 
- `GoogleCloudLogWriter`
- `ConsoleLogWriter`
- `MultiLogWriter`
- `NullLogWriter`
 
The `GoogleCloudLogWriter` is the main logger which should be used when an application is running inside the Google Cloud Platform. The `ConsoleLogWriter` is a simple console writer which can be swapped for the `GoogleCloudLogWriter` during development. The `MultiLogWriter` is a convenience class to configure multiple log writers simultaneously. Lastly the `NullLogWriter` is a void implementation which is used to prevent a `NullReferenceException` from occuring when no other log writer has been configured.
 
## GoogleCloudLogWriter
 
The `GoogleCloudLogWriter` is a relatively unexciting console logger which outputs log entries in a certain JSON format into `stdout` which Google Cloud's log analyzer knows how to parse and interpret accordingly.
 
The benefit of writing logs in GCP in this way is that the process of logging itself has almost no impact on latency, doesn't require any additional network calls, doesn't require any obscure dependencies or convoluted infrastructure configuration and it doesn't require log batching like most other log providers do. Logs will get ingested into the Google Cloud Logging console almost in real-time and allow the StackDriver analyzer do its job best.
 
The easiest way to create a new `GoogleCloudLogWriter` is to use the `Create(...)` builder method:
 
###### C#
 
```csharp
var logWriter = GoogleCloudLogWriter.Create(Severity.Info);
```

###### F#

```fsharp
let logWriter = GoogleCloudLogWriter.Create Severity.Info
```
 
This will create a basic GCP log writer with the minimum set of features. The severity parameter supports the full range of GCP's severity levels (see below).
 
### Severity Levels
 
Google Cloud Logging supports the following severity levels:
 
| Severity       | Value | Description |
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
 
### Configuring the GoogleCloudLogWriter
 
The `GoogleCloudLogWriter` offers several builder methods which can be used to add additional features to the logger instance. The `GoogleCloudLogWriter` class itself is immutable and each builder method invocation will create a new instance which makes it safe to use in a multi threaded environment (which is useful when a new log writer has to be configured per HTTP request - more on this topic in the [ASP.NET Core section]()).
 
#### Error Reporting Feature
 
When logging an `Exception` object with the `Error` severity level or above (`Error`, `Critical`, `Alert` and `Emergency`) then the error can get automatically propagated into [Google Error Reporting](https://cloud.google.com/error-reporting). This is a handy feature if a team wants to take additional advantage of error tracking and automatic error notifications in the Google Cloud Platform.
 
In order to enable this feature the `GoogleCloudLogWriter` has to be configured with a service context which the Error Reporting APIs rely on. This is done via the `AddServiceContext("serviceName", "serviceVersion")` builder method:
 
###### C#
 
```csharp
var logWriter = 
    GoogleCloudLogWriter
        .Create(Severity.Info)
        .AddServiceContext(
            "<your-service-name>",
            "<your-service-version>");
```

###### F#
 
```fsharp
let logWriter = 
    GoogleCloudLogWriter
        .Create(Severity.Info)
        .AddServiceContext(
            "<your-service-name>",
            "<your-service-version>")
```

The service name and service version are two properties solely used b the Google Error Reporting APIs and will not appear in the structured logs otherwise. For more information please check out the [Google Cloud Error Reporting formatting docs](https://cloud.google.com/error-reporting/docs/formatting-error-messages).

#### Labels

Labels are a neat feature to "tag" log entries with useful meta data which can be used to sieve through a large number of logs. Common use cases would be to label (= tag) each log entry with the name of the application/service and the version of it. This can be extremely handy if a team is analysing logs and trying to attribute certain log entries to a specific application version (e.g. trying to understand which version introduced a newly occuring error).

The `AddLabels(...)` builder method can be used to set a whole `IDictionary<string, string>` of labels on the `GoogleCloudLogWriter` instance, otherwise the `AddLabel(key, value)` builder method can be used to add an additional label to the existing dictionary of labels:

###### C#
   
```csharp
var logWriter = 
    GoogleCloudLogWriter
        .Create(Severity.Info)
        .AddServiceContext(
            "<your-service-name>",
            "<your-service-version>")
        .AddLabels(
            new Dictionary<string, string>
                {
                    {"appName", "MyApp1234"},
                    {"appVersion", "1.0.0"}
                });

// Add an extra label later:
var newLogWriter = logWriter.AddLabel("foo", "bar");
```
  
###### F#
   
```fsharp
let logWriter =
    GoogleCloudLogWriter
        .Create(Severity.Debug)
        .AddServiceContext(appName, appVersion)
        .AddLabels(
            dict [
                "appName", "MyApp1234"
                "appVersion", "1.0.0"
            ])

// Add an extra label later:
let logWriter' = logWriter.AddLabel("foo", "bar")
```

#### Timestamp Settings

By default the `GoogleCloudLogWriter` will set an ISO 8601 formatted UTC timestamp for each log entry, however if omitted then the Google Cloud log analyzer will set its own timestamp in RFC 3339 UTC "Zulu" format - accurate to nanoseconds and by 3 nanosecond digits more accurate than what .NET Core can achieve natively.

Computing the timestamp in .NET Core will be closer to the actual time when the log entry is being written, whereas letting Google Cloud set its own timestamp will enable a higher accuracy between different log entries. It's a trade-off and if the latter is preferred then the `UseGoogleCloudTimestamp()` builder method can be used to configure the log writer accordingly:

###### C#

```csharp
var logWriter = 
    GoogleCloudLogWriter
        .Create(Severity.Info)
        .AddServiceContext(
            "<your-service-name>",
            "<your-service-version>")
        .AddLabels(
            new Dictionary<string, string>
                {
                    {"appName", "MyApp1234"},
                    {"appVersion", "1.0.0"}
                })
        .UseGoogleCloudTimestamp();
```

###### F#
   
```fsharp
let logWriter =
    GoogleCloudLogWriter
        .Create(Severity.Debug)
        .AddServiceContext(appName, appVersion)
        .AddLabels(
            dict [
                "appName", "MyApp1234"
                "appVersion", "1.0.0"
            ])
        .UseGoogleCloudTimestamp()
```

#### Correlation ID

Sometimes it can be very helpful to tag log entries with a specific ID in order to group correlated log entries together. This can be done by adding the correlation ID feature to the `GoogleCloudLogWriter`:

###### C#
 
```csharp
var logWriter = 
    GoogleCloudLogWriter
        .Create(Severity.Info)
        .AddServiceContext(
            "<your-service-name>",
            "<your-service-version>")
        .AddLabels(
            new Dictionary<string, string>
            {
                {"appName", "MyApp1234"},
                {"appVersion", "1.0.0"}
            })
        .UseGoogleCloudTimestamp()
        .AddCorrelationId("<some id>");
```
 
###### F#
    
```fsharp
let logWriter =
    GoogleCloudLogWriter
        .Create(Severity.Debug)
        .AddServiceContext(appName, appVersion)
        .AddLabels(
            dict [
                "appName", "MyApp1234"
                "appVersion", "1.0.0"
            ])
        .UseGoogleCloudTimestamp()
        .AddCorrelationId("<some id>")
```

The correlation ID will be set as part of the data dictionary of each log entry, which can be queried by the `jsonPayload.data.correlationId` property in the Google Cloud Console. If the name `correlationId` collides with another data piece with the same name, or if one simply wants to specify a different name, then an additional string argument can be passed into the `AddCorrelationId(value, key)` builder method to set a custom key:

```
logWriter.AddCorrelationId("<some id>", "requestId")
```

This feature is most useful in combination with the `LogWriterPerRequestMiddleware` (see [ASP.NET Core](#using-with-aspnet-core)).

#### HTTP Context

Log entries can also include information about the HTTP request if they've been written during the ASP.NET Core request pipeline. In order to do so pass the `HttpContext` object into the `AddHttpContext(ctx)` builder method:

```
logWriter.AddHttpContext(context);
```

This feature makes most sense as part of the `LogWriterPerRequestMiddleware` (see [ASP.NET Core](#using-with-aspnet-core)).
 
## .NET Core 
 
 All it takes to configure a .NET Core application to use Logfella when creating new `ILogger` or `ILogger<T>` instances from the `Microsoft.Extensions.Logging` namespace is to register the Logfella adapter during application startup:
 
## Using with ASP.NET Core

Configuring Logfella as the preferred logging library in a .NET Core application is as easy as registering the library via the `UseLogfella()` extension method of an `IHostBuilder` or alternatively of an `ILoggingBuilder` object:

###### C#

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
 
###### F#

```fsharp
Host.CreateDefaultBuilder()
    .UseLogfella()
    .ConfigureWebHost(
        fun webHostBuilder ->
            webHostBuilder
                .UseKestrel()
                .Configure(configureApp)
                .ConfigureServices(configureServices)
                |> ignore)
    .Build()
    .Run()
```

Additionally the `LogWriterPerRequestMiddleware` can be registered early in the HTTP pipeline to decorate all log entries with HTTP context information and an optional correlation ID.

For a more detailed example of how to use Logfella as part of an ASP.NET core web application please refer to the MVC example in C# or the Giraffe application in F#, which are both found in the `/samples` directory of this repository.
 
## License

 [Apache 2.0](https://github.com/dustinmoris/Logfella/master/LICENSE)