# Chrome devtools protocol implementation for Jint

Still at the very experimental/non-functional stage. Also built against a still private branch of Jint.

The goal is to implement the relevant parts of the devtools `Debugger` and `Runtime` domains - possibly the other `v8-only` parts.

Includes a Minimal HTTP+WS server based on `HttpListener`, with a simple interface for using a different server implementation (e.g. ASP.NET Core/Kestrel) as frontend.
