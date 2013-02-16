using System;
using System.Collections.Generic;

namespace JabbR.Commands
{
    public class CommandNotFoundException : Exception
    {
        public CommandNotFoundException() {}

        public CommandNotFoundException(string message) : base(message) {}
    }

    public class CommandAmbiguityException : Exception
    {
        public IEnumerable<string> Ambiguities { get; set; }

        public CommandAmbiguityException(IEnumerable<string> ambiguities)
        {
            Ambiguities = ambiguities;
        }

        public CommandAmbiguityException(string message) : base(message) { }
    }
}