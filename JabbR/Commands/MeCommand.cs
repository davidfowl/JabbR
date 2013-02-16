using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("me", "Type /me 'does anything'", "note", "user")]
    public class MeCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            if (args.Length  == 0)
            {
                throw new InvalidOperationException(LanguageResources.YouWhat);
            }

            var content = String.Join(" ", args);

            context.NotificationService.OnSelfMessage(room, callingUser, content);
        }
    }
}