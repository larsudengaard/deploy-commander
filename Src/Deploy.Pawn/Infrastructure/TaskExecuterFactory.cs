using System;
using System.Linq;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Executors;

namespace Deploy.Pawn.Infrastructure
{
    internal class TaskExecuterFactory : ITaskExecuterFactory
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