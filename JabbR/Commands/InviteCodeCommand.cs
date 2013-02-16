using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("invitecode", "Show the current invite code.", "", "room")]
    public class InviteCodeCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            if (String.IsNullOrEmpty(room.InviteCode))
            {
                context.Service.SetInviteCode(callingUser, room, RandomUtils.NextInviteCode());
            }

            context.NotificationService.PostNotification(room, callingUser, String.Format(LanguageResources.InviteCodeForThisRoomX, room.InviteCode));
        }
    }
}
