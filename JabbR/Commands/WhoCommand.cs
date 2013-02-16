﻿using JabbR.Models;
using JabbR.Resources;
using JabbR.Services;
using System;

namespace JabbR.Commands
{
    [Command("who", "Show a list of all users, Use [name] to show specific information about that user.", "[nickname]", "global")]
    public class WhoCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                context.NotificationService.ListUsers();
                return;
            }

            var name = MembershipService.NormalizeUserName(args[0]);

            ChatUser user = context.Repository.GetUserByName(name);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.WeDidntFindAnyoneWithTheUsername, name));
            }

            context.NotificationService.ShowUserInfo(user);
        }
    }
}