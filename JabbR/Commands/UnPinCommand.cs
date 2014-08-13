using JabbR.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Commands
{
    [Command("unpin", "UnPin_CommandInfo", "room", "room")]
    public class UnPinCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            string roomName = args.Length > 0 ? args[0] : callerContext.RoomName;

            if (String.IsNullOrEmpty(roomName))
            {
                throw new HubException(LanguageResources.UnPinRoom_RoomRequired);
            }

            ChatRoom room = context.Repository.VerifyRoom(roomName, mustBeOpen: false);

            context.Service.UnPinRoom(callingUser, room);

            context.NotificationService.UnPinRoom(room);
        }
    }
}