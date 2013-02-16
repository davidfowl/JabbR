using JabbR.Models;
using JabbR.Resources;
using System;
using System.Linq;

namespace JabbR.Commands
{
    [Command("msg", "Send a private message to nickname. @ is optional.", "@nickname message", "user")]
    public class PrivateMessageCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (context.Repository.Users.Count() == 1)
            {
                throw new InvalidOperationException(LanguageResources.YoureTheOnlyPersonInHere);
            }

            if (args.Length == 0 || String.IsNullOrWhiteSpace(args[0]))
            {
                throw new InvalidOperationException(LanguageResources.WhoAreYouTryingSendAPrivateMessageTo);
            }
            var toUserName = args[0];
            ChatUser toUser = context.Repository.VerifyUser(toUserName);

            if (toUser == callingUser)
            {
                throw new InvalidOperationException(LanguageResources.YouCantPrivateMessageYourself);
            }

            string messageText = String.Join(" ", args.Skip(1)).Trim();

            if (String.IsNullOrEmpty(messageText))
            {
                throw new InvalidOperationException(String.Format(LanguageResources.WhatDidYouWantToSayToX, toUser.Name));
            }

            context.NotificationService.SendPrivateMessage(callingUser, toUser, messageText);
        }
    }
}