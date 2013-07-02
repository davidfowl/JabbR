using System;
using JabbR.Models;

namespace JabbR.Commands
{
    [Command("silentban", "Ban_SilentCommandInfo", "user", "admin")]
    public class SilentBanCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.Ban_UserRequired);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (targetUser.BanStatus == UserBanStatus.SilentlyBanned)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.Ban_UserAlreadyBannedSilently, targetUser.Name));
            }

            context.Service.BanUser(callingUser, targetUser, silent: true);
            context.NotificationService.BanUser(targetUser, silent: true);
            context.Repository.CommitChanges();
        }
    }
}