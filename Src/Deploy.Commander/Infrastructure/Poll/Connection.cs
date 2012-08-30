using System;
using System.Timers;
using System.Web.Mvc.Async;

namespace Deploy.Commander.Infrastructure.Poll
{
    public class Connection
    {
        public Connection(Guid id, AsyncManager manager, Timer timer)
        {
            Id = id;
            Manager = manager;
            Timer = timer;
        }

        public Guid Id { get; set; }
        public AsyncManager Manager { get; set; }
        public Timer Timer { get; set; }
    }
}