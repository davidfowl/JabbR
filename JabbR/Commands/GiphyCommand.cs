using System;
using System.IO;
using System.Net;
using System.Text;
using JabbR.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Http = JabbR.Infrastructure.Http;

namespace JabbR.Commands
{
    [Command("giphy","Giphy_CommandInfo", "keyword1 keyword2 keywordX", "user")]
    public class GiphyCommand : UserCommand
    {
        public override void Execute(CommandContext context, CallerContext callerContext, ChatUser callingUser, string[] args)
        {
            if (String.IsNullOrEmpty(callerContext.RoomName))
            {
                throw new HubException(LanguageResources.InvokeFromRoomRequired);
            }

            ChatRoom callingRoom = context.Repository.GetRoomByName(callerContext.RoomName);

            string url = String.Format("http://api.giphy.com/v1/gifs/random?api_key={0}&tag=", "dc6zaTOxFJmzC");

            //add keywords/tags to the url
            int i = 0;
            foreach (String keyword in args)
            {
                url += keyword;
                if (i < args.Length-1)
                {
                    url += "&";
                }
                i++;
            }

            dynamic data = ((dynamic) JsonConvert.DeserializeObject(Http.GetWebResponse(url))).data;
            string dataContent = data.ToString();

            //Only post message if data actully has content
            if (!dataContent.Equals("[]"))
            {
                string message = data.fixed_height_downsampled_url.ToString();
                context.NotificationService.GenerateMeme(callingUser, callingRoom, data.fixed_height_downsampled_url.ToString());
            }
        }
    }
}