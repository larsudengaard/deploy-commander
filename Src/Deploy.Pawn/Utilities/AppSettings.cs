using System;
using System.Configuration;

namespace Deploy.Pawn.Utilities
{
    public class AppSettings
    {
        public static bool GetBoolean(string key, bool isRequired = true, bool defaultsTo = default(bool))
        {
            switch (GetString(key, false, "").ToLower())
            {
                case "true":
                    return true;
                case "false":
                    return false;
            }
            if (isRequired)
                throw new AppSettingsException(key, "Must be an boolean value (true/false)");

            return defaultsTo;
        }

        public static string GetString(string key, bool isRequired = true, string defaultsTo = "")
        {
            var value = ConfigurationManager.AppSettings[key];
            if (value == null && isRequired)
                throw new AppSettingsException(key, "Must be set");

            return value ?? defaultsTo;
        }

        public static int GetInteger(string key, bool isRequired = true, int defaultsTo = default(int))
        {
            var value = ConfigurationManager.AppSettings[key];

            int intValue;
            if (value != null && int.TryParse(value, out intValue))
                return intValue;

            if (isRequired)
                throw new AppSettingsException(key, "Must be an integer value");

            return defaultsTo;
        }

        public class AppSettingsException : Exception
        {
            readonly string message;

            protected string Key { get; private set; }

            public AppSettingsException(string key, string message)
            {
                this.message = message;
                Key = key;
            }

            public override string Message
            {
                get { return string.Format("Error in AppSettings.{0}: {1}", Key, message); }
            }
        }
    }
}