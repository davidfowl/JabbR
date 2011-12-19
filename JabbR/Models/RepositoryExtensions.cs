﻿using System;
using System.Collections.Generic;
using System.Linq;
using JabbR.Services;
using System.Diagnostics;

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

        public static IEnumerable<ChatRoom> OrderByActivity(this IEnumerable<ChatRoom> rooms)
        {
            var roomsRet = rooms.Select(r => new
            {
                Room = r,
                TimeDistance = r.Messages.ToArray()
                    .OrderByDescending(m => m.When)
                    .Take(10)
                    .AggregateDistanceBetweenMessages(),
                TimeSinceLast = r.Messages.Max(m => m.When)
            }).OrderBy(n => n.TimeDistance).Select(n => n.Room);
            return roomsRet;

        }
        private static TimeSpan AggregateDistanceBetweenMessages(this IEnumerable<ChatMessage> input)
        {
            if (input.Count() > 0)
            {
                TimeSpan ret = default(TimeSpan);
                ChatMessage previous = null;
                foreach(var current in input)
                {
                    if (previous == null)
                    {
                        ret = ret.Add(DateTime.Now - current.When);
                    }
                    else
                    {
                        ret = ret.Add(previous.When - current.When);
                    }
                    previous = current;
                }
                return ret;
            }
            else
            {
                return TimeSpan.MaxValue;
            }
        }

        public static ChatRoom VerifyUserRoom(this IJabbrRepository repository, ChatUser user, string roomName)
        {
            if (String.IsNullOrEmpty(roomName))
            {
                throw new InvalidOperationException("Use '/join room' to join a room.");
            }

            roomName = ChatService.NormalizeRoomName(roomName);

            ChatRoom room = repository.GetRoomByName(roomName);

            if (room == null)
            {
                throw new InvalidOperationException(String.Format("You're in '{0}' but it doesn't exist.", roomName));
            }

            if (!room.Users.Any(u => u.Name.Equals(user.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException(String.Format("You're not in '{0}'. Use '/join {0}' to join it.", roomName));
            }

            return room;
        }

        public static ChatUser VerifyUserId(this IJabbrRepository repository, string userId)
        {
            ChatUser user = repository.GetUserById(userId);

            if (user == null)
            {
                throw new InvalidOperationException("You don't have a name. Pick a name using '/nick nickname'.");
            }

            return user;
        }

        public static ChatRoom VerifyRoom(this IJabbrRepository repository, string roomName)
        {
            if (String.IsNullOrWhiteSpace(roomName))
            {
                throw new InvalidOperationException("Room name cannot be blank!");
            }

            roomName = ChatService.NormalizeRoomName(roomName);

            var room = repository.GetRoomByName(roomName);

            if (room == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find room '{0}'.", roomName));
            }

            return room;
        }

        public static ChatUser VerifyUser(this IJabbrRepository repository, string userName)
        {
            userName = ChatService.NormalizeUserName(userName);

            ChatUser user = repository.GetUserByName(userName);

            if (user == null)
            {
                throw new InvalidOperationException(String.Format("Unable to find user '{0}'.", userName));
            }

            return user;
        }
    }
}