using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("invite", "Invite a user to join a room.", "user room", "room")]
    public class InviteCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhoDoYouWantToInvite);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (targetUser == callingUser)
            {
                throw new InvalidOperationException(LanguageResources.YouCantInviteYourself);
            }

            if (args.Length == 1)
            {
                throw new InvalidOperationException(LanguageResources.InviteThemToWhichRoom);
            }

            string roomName = args[1];
            ChatRoom targetRoom = context.Repository.VerifyRoom(roomName);

            context.NotificationService.Invite(callingUser, targetUser, targetRoom);
        }
    }
}