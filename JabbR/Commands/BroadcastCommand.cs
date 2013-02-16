using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("broadcast", "Sends a message to all users in all rooms. Only administrators can use this command.", "message", "global")]
    public class BroadcastCommand : AdminCommand
    {
        public override void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, Models.ChatUser callingUser, string[] args)
        {
            string messageText = String.Join(" ", args).Trim();

            if (String.IsNullOrEmpty(messageText))
            {
                throw new InvalidOperationException(LanguageResources.WhatDidYouWantToBroadcast);
            }

            context.NotificationService.BroadcastMessage(callingUser, messageText);
        }
    }
}