# Chrome devtools protocol implementation for Jint

Still at the experimental/non-functional stage (does absolutely nothing at the moment). Also built against a still private branch of Jint.

The goal is to implement the parts of the devtools `Debugger` and `Runtime` domains - possibly the other `v8-only` parts.

Includes a Minimal HTTP+WS server based on `HttpListener`, with a simple interface for using a different server implementation (e.g. ASP.NET Core) as frontend.
