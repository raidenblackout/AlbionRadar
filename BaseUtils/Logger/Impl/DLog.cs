using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseUtils.Logger.Impl
{
    public class ConsoleLogger : ILogLogger
    {
        public void D(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            throw new NotImplementedException();
        }

        public void E(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            throw new NotImplementedException();
        }

        public void I(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            throw new NotImplementedException();
        }
    }

    public static class DLog
    {
        private static readonly ILogLogger logger = new ConsoleLogger();
        public static void D(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            logger.D(message, callerMemberName, callerLineNumber, callerFilePath);
        }

        public static void E(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            logger.E(message, callerMemberName, callerLineNumber, callerFilePath);
        }

        public static void I(string message, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int callerLineNumber = 0, [CallerFilePath] string callerFilePath = "")
        {
            logger.I(message, callerMemberName, callerLineNumber, callerFilePath);
        }
    }
}
