﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JabbR.Api.Model
{
    public class MessageApiModel
    {
        public string Content { get; set; }
        public string Username { get; set; }
        public DateTimeOffset When { get; set; }
    }
}