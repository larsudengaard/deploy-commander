using System;
using System.Diagnostics;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;
using System.Linq;

namespace Deploy.Pawn
{
    public class PawnService : IPawnService
    {
        private readonly ITaskExecuterFactory taskExecuterFactory;

        public PawnService(ITaskExecuterFactory taskExecuterFactory)
        {
            this.taskExecuterFactory = taskExecuterFactory;
        }

        public IResponse Execute(ITask task)
        {
            Debug.WriteLine(string.Format("{0}: Received task", DateTime.Now));

            bool successful = true;
            Exception exception = null;
            IResult result = null;
            try
            {
                ITaskExecutor taskExecutor = taskExecuterFactory.CreateExecuterFor(task);
                Debug.WriteLine(string.Format("{0}: Executing {1}", DateTime.Now, task.GetType().Name));
                result = taskExecutor.Execute(task);
            }
            catch(Exception e)
            {
                Debug.WriteLine(string.Format("{0}: Error in {1}, {2} ({3})", DateTime.Now, task.GetType().Name, e.GetType().Name, e.Message));
                successful = false;
                exception = e;
            }

            var resultType = task.GetType().GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (ITask<>)).GetGenericArguments()[0];

            return CreateResponse(resultType, result, successful, exception);
        }

        IResponse CreateResponse(Type resultType, IResult result, bool successful, Exception e)
        {
            Type responseType = typeof(Response<>).MakeGenericType(resultType);
            return (IResponse) Activator.CreateInstance(responseType, result, successful, e);
        }
    }
}