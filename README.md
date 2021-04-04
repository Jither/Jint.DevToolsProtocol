# Chrome devtools protocol implementation for Jint

Still at the *very* experimental/non-functional stage. Also built against a still private branch of Jint (see https://github.com/Jither/jint/tree/devtools-protocol).

The goal is to implement the relevant parts of the devtools `Debugger` and `Runtime` domains - possibly the other `v8-only` parts.

Includes a Minimal HTTP+WS server based on `HttpListener`, with a simple interface for using a different server implementation (e.g. ASP.NET Core/Kestrel) as frontend.

## Quick overview

__Jint.DevToolsProtocol__

* `Server` - HTTP+WS server (`MinimalServer`) and interface (`IDTPServer`) required for Handlers to communicate with different server implementations.
* `Handlers` - Handlers for HTTP and WebSocket communication - these do conversion to/from JSON of devtools protocol methods and events (`WebSocketHandler`) as well as serving the HTTP API for devtools target discovery (`HttpHandler`).
* `Logging` - very basic logging - for this experimental stage.
* `Helpers` - various helpers, duh.
* `Protocol` - this is where the bulk of the protocol resides.

__DevToolsExample__

Minimal example of running the server.

## Usage

* Run the server - currently it only takes the path of a single Javascript file to execute, as well as a port.
* Open `devtools://devtools/bundled/js_app.html?ws=127.0.0.1:9222&v8only=true`

`v8only=true` disables the tabs (and WS communication) that aren't relevant for debugging Javascript only.

Note that while the server *will* probably show up in `chrome://inspect/#devices` (if selecting a different port than 9222), if you start the debugging session using this method, Chrome will likely randomly disconnect after a short while. It also won't react to my early attempts at getting it to set `v8only` from the HTTP discovery service.