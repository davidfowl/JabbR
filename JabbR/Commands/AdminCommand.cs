using JabbR.Models;
using JabbR.Resources;
using System;

namespace JabbR.Commands
{
    public abstract class AdminCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (!callingUser.IsAdmin)
            {
                throw new InvalidOperationException(LanguageResources.YouAreNotAnAdmin);
            }

            ExecuteAdminOperation(context, callerContext, callingUser, args);
        }

        public abstract void ExecuteAdminOperation(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args);
    }
}