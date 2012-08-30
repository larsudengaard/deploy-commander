using System;
using Deploy.Tasks;

namespace Deploy.Procedures
{
    public class TaskFailedException : Exception
    {
        readonly ITask task;

        public ITask Task
        {
            get { return task; }
        }

        public TaskFailedException(ITask task)
        {
            this.task = task;
        }
    }
}