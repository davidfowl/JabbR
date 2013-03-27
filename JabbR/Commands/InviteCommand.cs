using System;
using System.Collections.Generic;
using System.Linq;
using JabbR.Models;

namespace JabbR.Commands
{
    [Command("invite", "Invite a user to join a room.", "user [more users] room", "room")]
    public class InviteCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidOperationException("Who do you want to invite, and into what room?");
            }

            if (args.Length == 1)
            {
                throw new InvalidOperationException("Invite them to which room?");
            }

            String roomName = null;
            var targetUsers = new List<ChatUser>();

            for (int i = 0; i < args.Length; i++)
            {
                var targetUserName = args[i];
                var isLastArg = i == (args.Length - 1);

                if (String.Equals (targetUserName, callingUser.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new InvalidOperationException("You can't invite yourself!");
                }

                try
                {
                    targetUsers.Add (context.Repository.VerifyUser(targetUserName));
                }
                catch (InvalidOperationException)
                {
                    if (isLastArg)
                    {
                        roomName = targetUserName;
                        break;
                    }
                    throw;
                }
            }

            if (roomName == null)
            {
                throw new InvalidOperationException("Invite them to which room?");
            }

            ChatRoom targetRoom = context.Repository.VerifyRoom(roomName);

            foreach (var targetUser in targetUsers)
            {
                context.NotificationService.Invite(callingUser, targetUser, targetRoom);
            }
        }
    }
}