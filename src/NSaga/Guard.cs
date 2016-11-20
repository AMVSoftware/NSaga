using System;
using System.Diagnostics;

namespace NSaga
{
    internal static class Guard
    {
        [DebuggerHidden]
        internal static void ArgumentIsNotNull(object value, string argument)
        {
            if (value == null)
            {
                throw new ArgumentNullException(argument);
            }
        }


        [DebuggerHidden]
        internal static void CheckSagaMessage(ISagaMessage sagaMessage, string argumentName)
        {
            if (sagaMessage == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (sagaMessage.CorrelationId == default(Guid))
            {
                throw new ArgumentException("CorrelationId was not provided in the message. Please make sure you assign CorrelationId before issuing it to your Saga");
            }
        }
    }
}
