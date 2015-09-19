using System;
using System.Runtime.Serialization;
using Uno.Model;

namespace WebClient.Exceptions
{
    public class WrongCardException : Exception
    {
        public WrongCardException() : this("This card cannot be discarded")
        {
            
        }

        public WrongCardException(string message) : base(message)
        {
        }

        public WrongCardException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WrongCardException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Card Card { get; internal set; }
    }
}