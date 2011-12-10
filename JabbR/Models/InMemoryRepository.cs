﻿using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading;
using JabbR.ViewModels;

namespace JabbR.Models
{
    public class InMemoryRepository : IJabbrRepository
    {
        private readonly HashSet<ChatUser> _users;
        private readonly HashSet<ChatRoom> _rooms;

        public InMemoryRepository()
        {
            _users = new HashSet<ChatUser>();
            _rooms = new HashSet<ChatRoom>();
        }

        public IQueryable<ChatRoom> Rooms { get { return _rooms.AsQueryable(); } }

        public IQueryable<ChatUser> Users { get { return _users.AsQueryable(); } }

        public void Add(ChatRoom room)
        {
            _rooms.Add(room);
        }

        public void Add(ChatUser user)
        {
            _users.Add(user);
        }

        public void Add(ChatMessage message)
        {
            // There's no need to keep a collection of messages outside of a room
            var room = _rooms.First(r => r == message.Room);
            room.Messages.Add(message);
        }

        public void Add(ChatClient client)
        {
            var user = _users.FirstOrDefault(u => client.User == u);
            user.ConnectedClients.Add(client);
        }

        public void Remove(ChatClient client)
        {
            var user = _users.FirstOrDefault(u => client.User == u);
            user.ConnectedClients.Remove(client);
        }

        public void Remove(ChatRoom room)
        {
            _rooms.Remove(room);
        }

        public void Remove(ChatUser user)
        {
            _users.Remove(user);
        }

        public void CommitChanges()
        {
            // no-op since this is an in-memory impl' of the repo
        }

        public void Dispose()
        {
        }

        public ChatUser GetUserById(string userId)
        {
            return _users.Where(x => x.Id != null).FirstOrDefault(u => u.Id.Equals(userId, StringComparison.OrdinalIgnoreCase));
        }

        public ChatUser GetUserByName(string userName)
        {
            return _users.Where(x => x.Name != null).FirstOrDefault(u => u.Name.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public ChatRoom GetRoomByName(string roomName)
        {
            return _rooms.Where(x => x.Name != null).FirstOrDefault(r => r.Name.Equals(roomName, StringComparison.OrdinalIgnoreCase));
        }

        public IQueryable<ChatRoom> GetAllowedRooms(ChatUser user)
        {
            return _rooms.Where(r => !r.Private || r.Private && r.AllowedUsers.Contains(user)).AsQueryable();
        }

        public IQueryable<ChatMessage> GetMessagesByRoom(string roomName)
        {
            var room = GetRoomByName(roomName);
            if (room == null)
            {
                return Enumerable.Empty<ChatMessage>().AsQueryable();
            }
            return room.Messages.AsQueryable();
        }

        public IQueryable<ChatUser> SearchUsers(string name)
        {
            return _users.Online()
                         .Where(u => u.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1)
                         .AsQueryable();
        }

        public ChatUser GetUserByClientId(string clientId)
        {
            return _users.FirstOrDefault(u => u.ConnectedClients.Any(c => c.Id == clientId));
        }

        public ChatClient GetClientById(string clientId)
        {
            return _users.SelectMany(u => u.ConnectedClients).FirstOrDefault(c => c.Id == clientId);
        }

        public IQueryable<ChatMessage> GetPreviousMessages(string messageId)
        {
            // Ineffcient since we don't have a messages collection

            return (from r in _rooms
                    let message = r.Messages.FirstOrDefault(m => m.Id == messageId)
                    where message != null
                    from m in r.Messages
                    where m.When < message.When
                    select m).AsQueryable();
        }
    }
}