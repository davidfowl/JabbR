using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    [Command("lock", "Make a room private. Only works if you're the creator of that room.", "room", "room")]
    public class LockCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length  == 0)
            {
                throw new InvalidOperationException(LanguageResources.WhichRoomDoYouWantToLock);
            }

            string roomName = args[0];
            ChatRoom room = context.Repository.VerifyRoom(roomName);

            context.Service.LockRoom(callingUser, room);

            context.NotificationService.LockRoom(callingUser, room);
        }
    }
}