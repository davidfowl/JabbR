using System;
using System.Linq;
using JabbR.Models;

namespace JabbR.Commands
{
    [Command("ban", "Ban_CommandInfo", "user", "admin")]
    public class BanCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.Ban_UserRequired);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (targetUser.BanStatus == UserBanStatus.Banned)
            {
                throw new InvalidOperationException(String.Format("{0} is already banned.", targetUser.Name));
            }

            context.Service.BanUser(callingUser, targetUser, silent: false);
            context.NotificationService.BanUser(targetUser, silent: false);
            context.Repository.CommitChanges();
        }
    }
}