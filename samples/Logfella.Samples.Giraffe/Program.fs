namespace Logfella.Samples.Giraffe

module Program =
    open System
    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Hosting
    open Microsoft.Extensions.DependencyInjection
    open Microsoft.Extensions.Logging
    open Giraffe
    open Logfella
    open Logfella.LogWriters
    open Logfella.Adapters
    open Logfella.AspNetCore

    // ----------------------
    // Global settings
    // ----------------------

    // Load this from a config file, environment variables,
    // the .fsproj file, or whatever else you prefer:
    let appName = "SampleAppFSharp"
    let appVersion = "v1.0.0"

    // ----------------------
    // Configure LogWriter
    // ----------------------

    // Generate a new GoogleCloudLogWriter with the help of
    // the builder methods by dynamically adding more
    // features to the instance. These builder methods allow
    // one to add features or configurations based on conditions
    // in a fluent way:
    let googleCloudLogWriter =
        GoogleCloudLogWriter
            .Create(Severity.Debug)                 // Min severity to write logs
            .AddServiceContext(appName, appVersion) // Configure service context for Google Error Reporting
            .AddLabels(
                dict [                        // Add labels which will be added to all log entries.
                    "appName", appName        // Use labels to add useful information for log filtering
                    "appVersion", appVersion  // and log analysis (e.g. which app version has started throwing errors, etc.)
                ])

    // ----------------------
    // Set up Giraffe web app
    // ----------------------

    let pingHandler : HttpHandler =
        fun ctx next ->
            // In F# the easiest way to write log entries
            // is to use the static `Log` instance:
            Log.Info "Ping!"
            text "ping" ctx next

    let routes : HttpHandler =
        choose [
            GET_HEAD >=>
                choose [
                    route "/ping" >=> pingHandler
                    route "/pong" >=> text "pong"
                    route "/foo"  >=> text "foo"
                ]
            setStatusCode 404 >=> text "Not found" ]

    let errorHandler (ex : Exception) (logger : ILogger) =
        // You can also request a `Microsoft.Extensions.Logging.ILogger` instance
        // and write logs via this interface, however the `ILogger` interface
        // doesn't expose the full set of severity levels which limits it in some ways:
        logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
        clearResponse >=> setStatusCode 500 >=> text ex.Message

    // ----------------------
    // Configure and run web host
    // ----------------------

    let private configureServices (svc : IServiceCollection) =
        svc.AddGiraffe() |> ignore

    let private configureApp (app : IApplicationBuilder) =
        // This is an optional middleware which can be configured to create a new
        // `ILogWriter` instance for each incoming HTTP request. This makes it possible
        // to decorate all log entries with the HTTP context (see `AddHttpContext(ctx)`)
        // and to set a correlation ID for all log entries which belong to the same
        // HTTP request (see `AddCorrelationId(..., ...)`).
        //
        // Because this middleware requires a factory method (HttpContext -> ILogWriter)
        // one has full control over how or which `ILogWriter` to use and to inspect an incoming
        // HttpRequest in order to dynamically configure the instance (e.g. check the HTTP
        // headers for an X-Correlation-Id header and use that value as `requestId`):
        app.UseRequestBasedLogWriter(
            fun ctx ->
                // Note that one can use an already existing and pre-configured GoogleCloudLogWriter
                // to extend it with additional features without having to re-create the entire
                // object again. The GoogleCloudLogWriter is immutable and the builder methods
                // always create a new instance, which makes it safe to use it in this way:
                googleCloudLogWriter
                    .AddLabel("loggerType", "requestLogger")
                    .AddHttpContext(ctx)
                    .AddCorrelationId(Guid.NewGuid().ToString("N"), "requestId")
                    .AsLogWriter()) // <- Convenience method to cast GoogleCloudLogWriter to ILogWriter in F#
           .UseGiraffeErrorHandler(errorHandler)
           .UseGiraffe routes

    [<EntryPoint>]
    let main _ =
        try
            // Configure the default log writer:
            Log.SetDefaultLogWriter(googleCloudLogWriter)

            Host.CreateDefaultBuilder()
                // This is only required to configure an adapter so that ILogger objects
                // will use the above configured ILogWriter for all logs:
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
            0
        with ex ->
            // Note that the above configured "per-request" ILogWriter only exists in the scope
            // of the middleware and any middleware beneath it. Middleware which writes logs before
            // the `RequestBasedLogWriterMiddleware` has been called or log calls which occur outside
            // the entire HTTP pipeline altogether (like this one directly in the main function)
            // will obviously not have a request based `ILogWriter` and use the globally configured
            // default ILogWriter which has been set up via `Log.SetDefaultLogWriter(...)`
            Log.Emergency("Host terminated unexpectedly.", ex)
            1