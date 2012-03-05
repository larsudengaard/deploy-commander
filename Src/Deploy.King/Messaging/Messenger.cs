using System;
using System.Collections.Generic;

namespace Deploy.King.Messaging
{
    public class Messenger : IMessenger, IMessageSubscriber
    {
        readonly List<IMessageListener> listeners;

        public Messenger()
        {
            listeners = new List<IMessageListener>();
        }

        public void Publish(string message)
        {
            listeners.ForEach(x => x.HandleMessage(new Message(message)));
        }

        public void AddListener(IMessageListener listener)
        {
            if (listeners.Contains(listener))
                throw new ArgumentException("Listener already registered", "listener");

            listeners.Add(listener);
        }

        public void RemoveListener(IMessageListener listener)
        {
            if (!listeners.Remove(listener))
                throw new ArgumentException("Listener not registered", "listener");
        }
    }

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