using JabbR.Infrastructure;
using JabbR.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.IO;
using System.Linq;

namespace JabbR.Commands
{
    [Command("lmgtfy", "Lmgtfy_CommandInfo", "[@user] query", "user")]
    public class LmgtfyCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Models.ChatUser callingUser, string[] args)
        {
            if (String.IsNullOrEmpty(callerContext.RoomName))
            {
                throw new HubException(LanguageResources.InvokeFromRoomRequired);
            }

            if (args == null || args.Length < 1)
            {
                throw new HubException(LanguageResources.Lmstfy_DataRequired);
            }

            string query = null;
            ChatUser alertUser = null;
            if (args[0].StartsWith("@"))
            {
                alertUser = context.Repository.VerifyUser(args[0]);
                query = string.Join(" ", args.Skip(1));
            }
            else
            {
                query = string.Join(" ", args);
            }

            if (string.IsNullOrWhiteSpace(query))
            {
                throw new HubException(LanguageResources.Lmstfy_DataRequired);
            }

            ChatRoom callingRoom = context.Repository.GetRoomByName(callerContext.RoomName);

            string tinyUrlRequest = string.Format("http://tinyurl.com/api-create.php?url=http://lmgtfy.com/?q={0}", Uri.EscapeDataString(query));
            var tinyUrlResponse = Http.GetAsync(tinyUrlRequest).Result;
            string url;

            using (var tinyUrlStream = tinyUrlResponse.GetResponseStream())
            {
                using (var tinyUrlReader = new StreamReader(tinyUrlStream))
                {
                    url = tinyUrlReader.ReadToEnd();
                }
            }

            string msg;
            if (alertUser == null)
                msg = url;
            else
                msg = string.Format("@{0} {1}", alertUser.Name, url);

            context.NotificationService.SendMessage(callingRoom, msg);
        }
    }
}