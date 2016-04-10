using JabbR.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Commands
{
    [Command("pin", "Pin_CommandInfo", "room [priority]", "room")]
    public class PinCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {   
            string roomName = args.Length > 0 ? args[0] : callerContext.RoomName;             

            if (String.IsNullOrEmpty(roomName))
            {
                throw new HubException(LanguageResources.PinRoom_RoomRequired);
            }                      

            ChatRoom room = context.Repository.VerifyRoom(roomName, mustBeOpen: false);
            
            // set the pin priority
            int pinnedPriority = 0;
            if (args.Length > 1)
            {
                if (!Int32.TryParse(String.Join(" ", args.Skip(1)).Trim(), out pinnedPriority))
                {                   
                    throw new HubException(LanguageResources.PinRoom_PriorityNotInt);
                }                
            }                       

            context.Service.PinRoom(callingUser, room, pinnedPriority);

            context.NotificationService.PinRoom(room);
        }
    }
}