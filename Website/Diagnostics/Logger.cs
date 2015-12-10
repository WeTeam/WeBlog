using System;
using System.Diagnostics;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.WeBlog.Diagnostics
{
    public sealed class Logger
    {
        public static void Audit(string message)
        {
            Log.Audit(FormatMessage(message), GetCallerType());
        }

        public static void Audit(string message, object owner)
        {
            Log.Audit(FormatMessage(message), owner);
        }

        public static void Audit(string message, Type ownerType)
        {
            Log.Audit(FormatMessage(message), ownerType);
        }

        public static void Debug(string message)
        {
            Log.Debug(FormatMessage(message));
        }

        public static void Debug(string message, object owner)
        {
            Log.Debug(FormatMessage(message), owner);
        }

        public static void Error(string message)
        {
            Log.Error(FormatMessage(message), GetCallerType());
        }

        public static void Error(string message, object owner)
        {
            Log.Error(FormatMessage(message), owner);
        }

        public static void Error(string message, Type ownerType)
        {
            Log.Error(FormatMessage(message), ownerType);
        }

        public static void Error(string message, Exception exception, object owner)
        {
            Log.Error(FormatMessage(message), exception, owner);
        }

        public static void Error(string message, Exception exception, Type ownerType)
        {
            Log.Error(FormatMessage(message), exception, ownerType);
        }

        public static void Info(string message)
        {
            Log.Info(FormatMessage(message), GetCallerType());
        }

        public static void Info(string message, object owner)
        {
            Log.Info(FormatMessage(message), owner);
        }

        public static void Warn(string message)
        {
            Log.Warn(FormatMessage(message), GetCallerType());
        }

        public static void Warn(string message, object owner)
        {
            Log.Warn(FormatMessage(message), owner);
        }

        public static void Warn(string message, Exception exception, object owner)
        {
            Log.Warn(FormatMessage(message), exception, owner);
        }

        private static string FormatMessage(string message)
        {
            var callerName = GetCallerType().Name;
            return String.Format("[WeBlog][{0}] {1}", callerName, message);
        }

        private static Type GetCallerType()
        {
            int i = 2;
            Type type;
            do
            {
                type = new StackTrace().GetFrame(i++).GetMethod().DeclaringType;

            } while (type == typeof(Logger));
            return type;
        }
    }
}