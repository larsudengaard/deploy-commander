using System;

namespace Deploy.Procedures.Messaging
{
    public class Message
    {
        readonly string data;
        readonly DateTime time;

        public Message(string data)
        {
            this.data = data;
            time = DateTime.Now;
        }

        public DateTime Time
        {
            get { return time; }
        }

        public string Data
        {
            get { return data; }
        }
    }
}