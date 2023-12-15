using System.Net.Sockets;
using NetCoreServer;

namespace MSyncBot.Server;

class HttpCacheSession : HttpSession
{
    public HttpCacheSession(NetCoreServer.HttpServer server) : base(server) {}

    protected override void OnReceivedRequest(HttpRequest request)
    {
        Console.WriteLine(request);

        switch (request.Method)
        {
            case "HEAD":
                SendResponseAsync(Response.MakeHeadResponse());
                break;
            case "GET":
            {
                var key = request.Url;
                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);

                if (string.IsNullOrEmpty(key))
                {
                    SendResponseAsync(Response.MakeGetResponse(CommonCache.GetInstance().GetAllCache(),
                        "application/json; charset=UTF-8"));
                }
                else if (CommonCache.GetInstance().GetCacheValue(key, out var value))
                {
                    SendResponseAsync(Response.MakeGetResponse(value));
                }
                else
                    SendResponseAsync(Response.MakeErrorResponse(404,
                        "Required cache value was not found for the key: " + key));

                break;
            }
            case "POST":
            case "PUT":
            {
                var key = request.Url;
                var value = request.Body;
                    
                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);
                    
                CommonCache.GetInstance().PutCacheValue(key, value);
                    
                SendResponseAsync(Response.MakeOkResponse());
                break;
            }
            case "DELETE":
            {
                var key = request.Url;
                    
                key = Uri.UnescapeDataString(key);
                key = key.Replace("/api/cache", "", StringComparison.InvariantCultureIgnoreCase);
                key = key.Replace("?key=", "", StringComparison.InvariantCultureIgnoreCase);
                    
                SendResponseAsync(CommonCache.GetInstance().DeleteCacheValue(key, out var value)
                    ? Response.MakeGetResponse(value)
                    : Response.MakeErrorResponse(404, "Deleted cache value was not found for the key: " + key));

                break;
            }
            case "OPTIONS":
                SendResponseAsync(Response.MakeOptionsResponse());
                break;
            case "TRACE":
                SendResponseAsync(Response.MakeTraceResponse(request.Cache.Data));
                break;
            default:
                SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
                break;
        }
    }

    protected override void OnReceivedRequestError(HttpRequest request, string error)
    {
        Console.WriteLine($"Request error: {error}");
    }

    protected override void OnError(SocketError error)
    {
        Console.WriteLine($"HTTP session caught an error: {error}");
    }
}