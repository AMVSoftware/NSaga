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
    }
}
