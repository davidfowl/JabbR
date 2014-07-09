using JabbR.Models;
using Microsoft.AspNet.SignalR;
using System;

namespace JabbR.Commands
{
    [Command("lmgtfy", "Lmgtfy_CommandInfo", "query", "user")]
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

            ChatRoom callingRoom = context.Repository.GetRoomByName(callerContext.RoomName);

            string query = string.Join(" ", args);
            string queryEscaped = Uri.EscapeDataString(query);
            string url = String.Format("http://lmgtfy.com/?q={0}", queryEscaped);

            context.NotificationService.SendMessage(callingRoom, url);
        }
    }
}