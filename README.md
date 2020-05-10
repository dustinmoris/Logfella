# Logfella

Logfella is a small logging library specifically designed to work well with [Google Cloud Logging](https://cloud.google.com/logging/).

If you are running a .NET Core or ASP.NET Core application inside GCP and want to write structured logs into [Google Cloud Logging](https://cloud.google.com/logging/) then this library will offer you a more advanced integration point than .NET Core's out of the box `ILogger` providers.

Logfella writes structured logs directly to stdout in a specific JSON format which GCP's log analyzer can natively understand and process accordingly. Logfella offers support for the full range of Google's severity levels, automatic propagation of exceptions into [Google Error Reporting](https://cloud.google.com/error-reporting), full integration with `Microsoft.Extensions.Logging` libraries and much more.

#### NOTE:

This library is an early version mostly intended for personal use and neither the docs nor the APIs have been fully fledged out for wider exposure.

That being said, it has been written with flexibility and extensibility 100% in mind and hopefully to suit more than just my immediate and personal use case. It's been open sourced under Apache 2.0 and feedback and/or PRs are more than welcome.

## Benefits of Logfella

- Write intuitively structured logs into [Google Cloud Logging](https://cloud.google.com/logging/)
- Automatic error propagation into [Google Error Reporting](https://cloud.google.com/error-reporting) when exceptions are being logged
- Support of custom log labels for advanced log filtering
- Support of the full range of severity levels (9 in total)
- Automatically sets the correct timestamp and service context information for all Google Cloud logs
- Seamlessly integrates with .NET Core's logging providers (`ILogger` and `ILoggerProvider`)
- Optimised for C# and F# applications so that logging feels natural from an object-oriented as well as from a functional architecture
- `ILogWriter` can be injected either via DI or called directly from the static `Log` type
- 100% customisable and extensible
- Uses .NET Core's new `System.Text.Json` library for faster JSON serialisation
- Supports log aggregation via an optional correlation ID which can be set on a `LogWriter` instance. The additional `PerRequestLogWriterMiddleware` makes it extremely easy to compute (or inherit) a correlation ID for all logs for a given web request pipeline. 
    
 ## How it works
 
More documentation coming soon, but for now check out the sample application inside `/samples`.
 
 ## Using with Microsoft.Extensions.Logging
 
 More documentation coming soon, but for now check out the sample application inside `/samples`.
 
 ## Using with ASP.NET Core
 
  More documentation coming soon.
 
 #### C# Example
 
 More documentation coming soon, but for now check out the sample application inside `/samples`.
 
 #### F# Example
 
 ```fsharp
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Logfella
open Logfella.Adapters
open Logfella.LogWriters

let private configureLogger() =
    // Get the desired log level from wherever you want
    // (JSON file, environment variable, TOML, YAML, ...)   
    let logLevel = Environment.GetEnvironmentVariable "LOG_LEVEL"

    let isProduction = 
        match Environment.GetEnvironmentVariable "ASPNETCORE_ENVIRONMENT" with
        | "Production" -> true
        |              -> false

    // You can call the ParseSeverity extension method on a string
    // to parse it into a valid Severity enum value:
    let severity = logLevel.ParseSeverity()
    
    // Initialise a simple console logger for local dev:
    let consoleLogger = ConsoleLogWriter(severity) :> ILogWriter

    // Initialise the StackDriver logger for production logging in GCP:
    let googleCloudLogger =
        GoogleCloudLogWriter(
            severity,               // Min severity for this logger
            "Your-App-Name",        // App name for GCP ErrorReporting
            "Your-App-Version",     // App version for GCP ErrorReporting
            dict [                  
                "foo", "bar"        // Dictionary of custom labels
                "custom", "label"   // attached to StackDriver logs
            ]) :> ILogWriter
    
    // Pick the correct logger based on the environment:
    Log.SetLogWriter(
        match isProduction with
        | true  -> googleCloudLogger
        | false -> consoleLogger)

[<EntryPoint>]
let main _ =
    try
        configureLogger()

        Host.CreateDefaultBuilder()
            .UseLogfella()  // Integrate with Micrisoft.Extensions.Logging
            .ConfigureWebHost(
                fun webHostBuilder ->
                    webHostBuilder
                        .UseKestrel()
                        .Configure(configureApp)
                        .ConfigureServices(configureServices)
                        |> ignore)
            .Build()
            .Run()
        0
    with ex ->
        Log.Emergency("Host terminated unexpectedly.", ex)
        1
```
 
 ## Custom LogWriters
 
 Coming soon.
 
 ## License

 [Apache 2.0](https://github.com/dustinmoris/Logfella/master/LICENSE)