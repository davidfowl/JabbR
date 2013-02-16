﻿using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("ban", "Ban a user from JabbR!", "user", "admin")]
    public class BanCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhoAreYouTtryingToBan);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            context.Service.BanUser(callingUser, targetUser);
            context.NotificationService.BanUser(targetUser);
            context.Repository.CommitChanges();
        }
    }
}