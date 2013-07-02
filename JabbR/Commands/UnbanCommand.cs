using System;
using System.Linq;
using JabbR.Models;

namespace JabbR.Commands
{
    [Command("unban", "Ban_UnbanCommandInfo", "user", "admin")]
    public class UnbanCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.Ban_UserRequired);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (targetUser.BanStatus == UserBanStatus.NotBanned)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.Ban_UserNotBanned, targetUser.Name));
            }

            context.Service.UnbanUser(callingUser, targetUser);
            context.NotificationService.UnbanUser(targetUser);
            context.Repository.CommitChanges();
        }
    }
}