using JabbR.Infrastructure;
using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("gravatar", "Set your gravatar.", "email", "user")]
    public class GravatarCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            string email = String.Join(" ", args);

            if (String.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException(LanguageResources.EmailWasNotSpecified);
            }

            string hash = email.ToLowerInvariant().ToMD5();

            // Set user hash
            callingUser.Hash = hash;

            context.NotificationService.ChangeGravatar(callingUser);

            context.Repository.CommitChanges();
        }
    }
}