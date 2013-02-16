using JabbR.Models;
using JabbR.Resources;
using System;
using System.Linq;

namespace JabbR.Commands
{
    [Command("kick", "Kick a user from the room. Note, this is only valid for owners of the room.", "user", "user")]
    public class KickCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, Models.ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhoAreYouTryingToKick);
            }

            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            if (context.Repository.GetOnlineUsers(room).Count() == 1)
            {
                throw new InvalidOperationException(LanguageResources.YoureTheOnlyPersonInHere);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            context.Service.KickUser(callingUser, targetUser, room);

            context.NotificationService.KickUser(targetUser, room);

            context.Repository.CommitChanges();
        }
    }
}