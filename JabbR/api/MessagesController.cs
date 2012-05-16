using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using JabbR.Api.Model;
using System.Net;
using JabbR.Models;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace JabbR.api
{
    public class MessagesController : ApiController
    {
        const string FilenameDateFormat = "yyyy-MM-dd.HHmmsszz";
        private IJabbrRepository _repository;

        public MessagesController(IJabbrRepository repository)
        {
            _repository = repository;
        }

        public HttpResponseMessage<IEnumerable<MessageApiModel>> GetAllMessages(string room, string format, string range, bool? download)
        {
            if (String.IsNullOrWhiteSpace(range))
            {
                range = "last-hour";
            }

            var end = DateTime.Now;
            DateTime start;

            switch (range)
            {
                case "last-hour":
                    start = end.AddHours(-1);
                    break;
                case "last-day":
                    start = end.AddDays(-1);
                    break;
                case "last-week":
                    start = end.AddDays(-7);
                    break;
                case "last-month":
                    start = end.AddDays(-30);
                    break;
                case "all":
                    start = DateTime.MinValue;
                    break;
                default:
                    throw new HttpResponseException("range value not recognized", HttpStatusCode.BadRequest);
                    
            }

            ChatRoom chatRoom = null;

            try
            {
                chatRoom = _repository.VerifyRoom(room, mustBeOpen: false);
            }
            catch (Exception ex)
            {
                throw new HttpResponseException(ex.Message, HttpStatusCode.NotFound);
            }

            if (chatRoom.Private)
            {
                // TODO: Allow viewing messages using auth token
                throw new HttpResponseException(String.Format("Unable to locate room {0}.", chatRoom.Name), HttpStatusCode.NotFound);
            }

            var messages = _repository.GetMessagesByRoom(chatRoom)
                .Where(msg => msg.When <= end && msg.When >= start)
                .OrderBy(msg => msg.When)
                .Select(msg => new MessageApiModel
                {
                    Content = msg.Content,
                    Username = msg.User.Name,
                    When = msg.When
                });

            var filenamePrefix = room + ".";

            if (start != DateTime.MinValue)
            {
                filenamePrefix += start.ToString(FilenameDateFormat, CultureInfo.InvariantCulture) + ".";
            }

            filenamePrefix += end.ToString(FilenameDateFormat, CultureInfo.InvariantCulture);

            //todo - figure out attachment
            HttpResponseMessage<IEnumerable<MessageApiModel>> response = null;
            if (format != null)
            {
                response = new HttpResponseMessage<IEnumerable<MessageApiModel>>(messages, new MediaTypeHeaderValue("application/" + format));
            }
            else
            {
                response = new HttpResponseMessage<IEnumerable<MessageApiModel>>(messages);
            }

            if (download.HasValue && download.Value)
            {
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = filenamePrefix + ".json"};
            }

            return response;
        }
    }
}