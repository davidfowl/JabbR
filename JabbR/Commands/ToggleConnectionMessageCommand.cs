using JabbR.Models;

namespace JabbR.Commands
{
    [Command("toggleconnectionmessage", "Toggles if connection messages are displayed or muted.", "", "room")]
    public class ToggleConnectionMessageCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            ChatRoom room = context.Repository.VerifyUserRoom(context.Cache, callingUser, callerContext.RoomName);

            context.Service.ToggleConnectionMessage(callingUser, room);

            context.NotificationService.ToggleConnectionMessage(callingUser, room);
        }
    }
}