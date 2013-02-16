using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("removeadmin", "", "", "")]
    public class RemoveAdminCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhichAdminDoYouWantToRemove);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            context.Service.RemoveAdmin(callingUser, targetUser);

            context.NotificationService.RemoveAdmin(targetUser);

            context.Repository.CommitChanges();
        }
    }
}