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