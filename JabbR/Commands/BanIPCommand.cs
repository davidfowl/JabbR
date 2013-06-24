using System;
using System.Linq;
using JabbR.Models;

namespace JabbR.Commands
{
    [Command("banip", "Ban a user's client IP addresses from JabbR!", "user", "admin")]
    public class BanIPCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Who do you want to ban?");
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            context.Service.BanUserIPs(callingUser, targetUser);
            context.NotificationService.BanUser(targetUser);
            context.Repository.CommitChanges();
        }
    }
}