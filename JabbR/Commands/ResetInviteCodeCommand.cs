using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("resetinvitecode", "Reset the current invite code. This will render the previous invite code invalid.", "", "room")]
    public class ResetInviteCodeCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Models.ChatUser callingUser, string[] args)
        {
            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            context.Service.SetInviteCode(callingUser, room, RandomUtils.NextInviteCode());

            context.NotificationService.PostNotification(room, callingUser, String.Format(LanguageResources.InviteCodeForThisRoomX, room.InviteCode));
        }
    }
}