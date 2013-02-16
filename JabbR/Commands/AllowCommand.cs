﻿using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("allow", "Give a user permission to a private room. Only works if you're an owner of that room.", "user room", "room")]
    public class AllowCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhoDoYouWantToAllow);
            }

            string targetUserName = args[0];

            ChatUser targetUser = context.Repository.VerifyUser(targetUserName);

            if (args.Length == 1)
            {
                throw new InvalidOperationException(LanguageResources.WhichRoom);
            }

            string roomName = args[1];
            ChatRoom targetRoom = context.Repository.VerifyRoom(roomName);

            context.Service.AllowUser(callingUser, targetUser, targetRoom);

            context.NotificationService.AllowUser(targetUser, targetRoom);

            context.Repository.CommitChanges();
        }
    }
}