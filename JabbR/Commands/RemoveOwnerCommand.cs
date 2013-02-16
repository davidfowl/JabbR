using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("removeowner", "Remove an owner from the specified room. Only works if you're the creator of that room.", "user room", "room")]
    public class RemoveOwnerCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhichOwnerDoYouWantToRemove);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (args.Length == 1)
            {
                throw new InvalidOperationException(LanguageResources.WhichRoom);
            }

            string roomName = args[1];
            ChatRoom targetRoom = context.Repository.VerifyRoom(roomName);

            context.Service.RemoveOwner(callingUser, targetUser, targetRoom);

            context.NotificationService.RemoveOwner(targetUser, targetRoom);

            context.Repository.CommitChanges();
        }
    }
}