using Microsoft.AspNet.SignalR.Hubs;

namespace JabbR.Infrastructure
{
    public static class HubCallerContextExtensions
    {
        public static string GetRemoteIP(this HubCallerContext context)
        {
            return (string)context.Request.Environment["server.RemoteIpAddress"];
        }
    }
}