using System;
using System.Linq;
using Deploy.Soldier.Application.Executors;
using Deploy.Tasks;

namespace Deploy.Soldier.Application.Infrastructure
{
    public class TaskExecuterFactory : ITaskExecuterFactory
    {
        public ITaskExecutor CreateExecuterFor(ITask task)
        {
            Type taskType = task.GetType();
            Type resultType = taskType.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ITask<>)).GetGenericArguments()[0];
            Type genericCommandHandler = typeof (ITaskExecutor<,>).MakeGenericType(taskType, resultType);
            var type = typeof(StopWebsiteExecutor).Assembly.GetTypes().Single(genericCommandHandler.IsAssignableFrom);

            return (ITaskExecutor)Activator.CreateInstance(type);
        }
    }
}