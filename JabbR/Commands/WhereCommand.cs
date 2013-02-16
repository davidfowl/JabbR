﻿using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("where", "List the rooms that user is in.", "nickname", "user")]
    public class WhereCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhoAreYouTryingToLocate);
            }

            string targetUserName = args[0];

            ChatUser user = context.Repository.VerifyUser(targetUserName);
            context.NotificationService.ListRooms(user);
        }
    }
}