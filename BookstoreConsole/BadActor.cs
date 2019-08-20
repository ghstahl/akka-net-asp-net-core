using Akka.Actor;
using System;

namespace BookstoreConsole
{
    public class InsanelyBadException : Exception
    {
        public InsanelyBadException(string message) : base(message)
        {
        }
    }
    public partial class Messages
    {
        public class BadActorMessage
        {
            public class DoThrowUnknownExcpetion { }
            public class DoThrownArithmeticException { }
            public class DoThrownInsanelyBadException { }
            public class DoNotSupportedException { }
        }
    }
    public class BadActor : ReceiveActor
    {
        public BadActor()
        {
            ReceiveAsync<Messages.BadActorMessage.DoThrowUnknownExcpetion>(async _ => {
                throw new Exception("Well Hello There!");
            });
            ReceiveAsync<Messages.BadActorMessage.DoThrownArithmeticException>(async _ => {
                throw new ArithmeticException("Well Hello There!");
            });
            ReceiveAsync<Messages.BadActorMessage.DoThrownInsanelyBadException>(async _ => {

                throw new InsanelyBadException("Well Hello There!");
            });
            ReceiveAsync<Messages.BadActorMessage.DoNotSupportedException>(async _ => {
                throw new NotSupportedException("Well Hello There!");
            });
        }
    }
}
