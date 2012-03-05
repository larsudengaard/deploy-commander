using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Web.Mvc.Async;

namespace Deploy.Infrastructure.Poll
{
    public static class Bus
    {
        static Bus()
        {
            Subscribers = new Dictionary<Subscription, List<Guid>>();
            Connections = new Dictionary<Guid, Connection>();
            MessageQueues = new Dictionary<Guid, MessageQueue>();
        }

        static Dictionary<Guid, MessageQueue> MessageQueues { get; set; }
        static Dictionary<Subscription, List<Guid>> Subscribers { get; set; }
        static Dictionary<Guid, Connection> Connections { get; set; }

        public static void Subscribe(Guid connectionId, string channel, string group)
        {
            lock (((IDictionary) Subscribers).SyncRoot)
            {
                var subscription = new Subscription(channel, @group);
                if (!Subscribers.ContainsKey(subscription))
                {
                    Subscribers[subscription] = new List<Guid>();
                }

                Subscribers[subscription].Add(connectionId);
            }
        }

        public static void Unsubscribe(Guid connectionId, string channel, string group)
        {
            lock (((IDictionary)Subscribers).SyncRoot)
            {
                var subscription = new Subscription(channel, @group);
                if (Subscribers.ContainsKey(subscription))
                {
                    Subscribers[subscription].Remove(connectionId);
                }
            }
        }

        public static void Connect(Guid connectionId, AsyncManager asyncManager)
        {
            Complete(connectionId, null);

            lock (((IDictionary)Subscribers).SyncRoot)
            lock (((IDictionary)Connections).SyncRoot)
            {
                var timer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds) { AutoReset = false };
                timer.Elapsed += (sender, args) => Complete(connectionId, null);
                Connections.Add(connectionId, new Connection(connectionId, asyncManager, timer));
                asyncManager.OutstandingOperations.Increment();
                timer.Start();
            }
        }

        public static void Publish(Message message)
        {
            lock (((IDictionary)Subscribers).SyncRoot)
            {
                List<Guid> subscribingConnections;
                var subscription = new Subscription(message.Channel, message.Group);
                if (Subscribers.TryGetValue(subscription, out subscribingConnections))
                {
                    foreach (var connection in subscribingConnections)
                    {
                        AddToMessageQueue(connection, message);
                    }
                }
            }
        }

        static void AddToMessageQueue(Guid connectionId, Message message)
        {
            lock (((IDictionary) MessageQueues).SyncRoot)
            {
                if (!MessageQueues.ContainsKey(connectionId))
                {
                    var buffer = new MessageQueue(20, queue =>
                    {
                        queue.Timer.Stop();
                        lock (((IDictionary)MessageQueues).SyncRoot)
                        {
                            MessageQueues.Remove(connectionId);
                            Complete(connectionId, queue.Messages);
                        }
                    });

                    MessageQueues.Add(connectionId, buffer);
                }

                MessageQueue messageQueue = MessageQueues[connectionId];
                messageQueue.Timer.Stop();
                messageQueue.Messages.Add(message);
                messageQueue.Timer.Start();
            }
        }

        static void Complete(Guid connectionId, List<Message> messages)
        {
            lock (((IDictionary)Connections).SyncRoot)
            {
                Connection connection;
                if (Connections.TryGetValue(connectionId, out connection))
                {
                    connection.Timer.Stop();
                    connection.Manager.Parameters["messages"] = messages;
                    connection.Manager.OutstandingOperations.Decrement();
                    Connections.Remove(connectionId);
                }
            }
        }

        public class MessageQueue
        {
            public MessageQueue(double queueLifeTime, Action<MessageQueue> callBack)
            {
                Timer = new Timer(queueLifeTime);
                Timer.Elapsed += (sender, args) => callBack(this);
                Messages = new List<Message>();
            }

            public Timer Timer { get; private set; }
            public List<Message> Messages { get; private set; }
        }
    }
}