using JabbR.Services;
using JabbR.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JabbR.Models
{
    public static class RepositoryExtensions
    {
        public static IQueryable<ChatUser> Online(this IQueryable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatUser> Online(this IEnumerable<ChatUser> source)
        {
            return source.Where(u => u.Status != (int)UserStatus.Offline);
        }

        public static IEnumerable<ChatRoom> Allowed(this IEnumerable<ChatRoom> rooms, string userId)
        {
            return from r in rooms
                   where !r.Private ||
                         r.Private && r.AllowedUsers.Any(u => u.Id == userId)
                   select r;
        }

        public static ChatRoom VerifyUserRoom(this IJabbrRepository repository, ICache cache, ChatUser user, string roomName)
        {
            if (String.IsNullOrEmpty(roomName))
            {
                throw new InvalidOperationException(LanguageResources.UseJoinRoomToJoinARoom);
            }

            roomName = ChatService.NormalizeRoomName(roomName);

            ChatRoom room = repository.GetRoomByName(roomName);

            if (room == null)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.YoureInXButItDoesntExist, roomName));
            }

            if (!repository.IsUserInRoom(cache, user, room))
            {
                throw new InvalidOperationException(String.Format(LanguageResources.YoureNotInXUseJoinXToJoinIt, roomName));
            }

            return room;
        }

        public static bool IsUserInRoom(this IJabbrRepository repository, ICache cache, ChatUser user, ChatRoom room)
        {
            bool? cached = cache.IsUserInRoom(user, room);

            if (cached == null)
            {
                cached = repository.IsUserInRoom(user, room);
                cache.SetUserInRoom(user, room, cached.Value);
            }

            return cached.Value;
        }

        public static ChatUser VerifyUserId(this IJabbrRepository repository, string userId)
        {
            ChatUser user = repository.GetUserById(userId);

            if (user == null)
            {
                // The user isn't logged in 
                throw new InvalidOperationException(LanguageResources.YoureNotLoggedIn);
            }

            return user;
        }

        public static ChatRoom VerifyRoom(this IJabbrRepository repository, string roomName, bool mustBeOpen = true)
        {
            if (String.IsNullOrWhiteSpace(roomName))
            {
                throw new InvalidOperationException(LanguageResources.RoomNameCannotBeBlank);
            }

            roomName = ChatService.NormalizeRoomName(roomName);

            var room = repository.GetRoomByName(roomName);

            if (room == null)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.UnableToFindRoomX, roomName));
            }

            if (room.Closed && mustBeOpen)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.TheRoomXIsClosed, roomName));
            }

            return room;
        }

        public static ChatUser VerifyUser(this IJabbrRepository repository, string userName)
        {
            userName = MembershipService.NormalizeUserName(userName);

            ChatUser user = repository.GetUserByName(userName);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format(LanguageResources.UnableToFindUserX, userName));
            }

            return user;
        }
    }
}