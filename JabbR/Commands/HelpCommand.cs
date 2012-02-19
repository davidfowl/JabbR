﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using JabbR.Services;
using JabbR.Models;
using Ninject;

namespace JabbR.Commands
{
    [CommandInfo(Name="help", Usage="Type /help to show the list of commands", Weight=1.0f)]
    public class HelpCommand : ICommand
    {
        private readonly INotificationService _notificationService;

        public HelpCommand(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void Handle(string[] parts, string userId, string roomName, string clientId, string userAgent)
        {
            _notificationService.ShowHelp();            
        }
    }
}