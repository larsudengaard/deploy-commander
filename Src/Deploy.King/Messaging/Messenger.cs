using System;
using System.Collections.Generic;
using Deploy.Procedures.Messaging;

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
}