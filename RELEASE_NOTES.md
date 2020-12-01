# 7.1.0

- Upgraded to .NET 5

Logfella `7.1.0` and `7.0.0` are identical in functionality, but `7.1.0` is targetting .NET 5 only now.

# 7.0.0

- Renamed `RequestBasedLogWriterMiddleware` to `RequestScopedLogWriterMiddleware`
- Changed the `RequestLoggingMiddleware` to use an object of `RequestLoggingOptions` for its configuration.

The extension method `app.UseRequestLogging(...)` has changed its signature from multiple optional parameters to accepting a single `Action<RequestLoggingOptions>` method to configure the middleware.

# 6.2.0

The request logging middleware accepts two new parameters to filter HTTP headers from the logs:

- `includeHttpHeaders`
- `excludeHttpHeaders`

Example usage:

```
app.UseRequestLogging(
        excludeHttpHeaders: HashSet.New("Authorization", "Cookie", "X-ApiKey"))
    .UseRouting()
    .UseEndpoints(
        endpoints => { endpoints.MapControllers(); });
```

The `HashSet.New` helper method is just a nicer way of initialising a `HashSet<string>` of http headers in the example above. It's particularly helpful in F# projects where creating a new hash set is not as straight forward as `new HashSet<string> { "..." }` due to the awkward interop between F# and C# type classes.

# 6.1.0

Always creating a safe copy of the data dictionary to prevent a `NotSupportedException` when adding additional items in F# where the `dict` type only returns an immutable version of `IDictionary<TKey, TValue>`

# 6.0.1

Fixed `UseRequestLogging` extension method.

# 6.0.0

- Naming is hard, therefore renamed `LogWriterPerRequestMiddleware` to `RequestBasedLogWriterMiddleware`
- Extended the `RequestLoggingMiddleware` to have an option for logging before and after other middleware has been invoked (helpful to debug X-Forwarded-For settings and middleware)
- Added a new `MuteLogWriter` and the `Mute` builder class to filter/mute log entries based on a predicate (experimental)

# 5.0.1

Fixed private `ctor` of `GoogleCloudLogWriter`

# 5.0.0

- Renamed `IncludeHttpRequest` to `AddHttpContext`
- Renamed `Log.SetGlobalLogWriter` to `Log.SetDefaultLogWriter`
- Added `AsLogWriter` convenience method for F# to cast any log writer to the common interface
- Made all log writers immutable
- Added immutable builder methods to the `GoogleCloudLogWriter` to allow a more dynamic, flexible and safe composition API
- Fixed double `/` bug in the request path of the `httpRequest` object
- Improved error logging

# 4.0.0

- Removed duplicate error logging
- Setting the `@type` property for exception logs
- Added ability to log request data when using the per-request logging middleware
- Minor breaking changes through re-ordering and renaming some method parameters

# 3.0.0

- Changed the `SetLogWriter` and `GetLogWriter` methods to a single `LogWriter` property with setter and getter.
- Added an internal `AsyncLocal<ILogWriter>` instance for per-request log writer instantiation (can be used to set a correlation ID per HTTP request). Use the new `PerRequestLogWriterMiddleware` to set up a new `ILogWriter` instance for each request pipeline.
- The `UseLogfella` extension method will automatically register a transient `ILogWriter` dependency.
- Each log writer accepts an optional `correlationIdKey` and `correlationId` parameter in their constructors.
- The coloring of console logs can be disabled for the `ConsoleLogWriter` now.
- Sealed all `ILogWriter` implementations.

# 2.0.0

Extremely minor, but breaking API change of making `IHostBuilder` and `ILoggingBuilder` extension methods return a non `void` type to support configuration chaining.

# 1.0.0

First version. Hello Logfella!