using JabbR.Models;
using JabbR.Resources;
using System;
using System.Linq;

namespace JabbR.Commands
{
    [Command("list", "Show a list of users in the room.", "room", "room")]
    public class ListCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length  == 0)
            {
                throw new InvalidOperationException(LanguageResources.ListUsersInWhichRoom);
            }

            string roomName = args[0];
            ChatRoom room = context.Repository.VerifyRoom(roomName);

            var names = context.Repository.GetOnlineUsers(room).Select(s => s.Name);

            context.NotificationService.ListUsers(room, names);
        }
    }
}