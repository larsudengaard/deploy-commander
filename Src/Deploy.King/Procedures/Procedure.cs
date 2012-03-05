using System;
using Deploy.King.Builds;
using Deploy.King.Messaging;
using Deploy.King.Procedures.Arguments;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.King.Procedures
{
    public abstract class Procedure<TArguments> : IProcedure<TArguments> where TArguments : IProcedureArguments
    {
        readonly IMessenger messenger;

        protected Procedure(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public abstract bool Perform(Build build, TArguments arguments);

        public bool Perform(Build build, IProcedureArguments arguments)
        {
            try
            {
                Messenger.Publish("Start procedure " + GetType().Name);
                if (Perform(build, (TArguments) arguments))
                {
                    Messenger.Publish("Procedure completed successfully " + GetType().Name);
                    return true;
                }
            }
            catch (TaskFailedException)
            {
            }
            catch(Exception e)
            {
                Messenger.Publish("Procedure encountered an unhandled error: " + e.Message + "\n" + e.StackTrace);
            }

            Messenger.Publish("Procedure failed " + GetType().Name);
            return false;
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        protected IMessenger Messenger
        {
            get { return messenger; }
        }

        protected TResult ExecuteTask<TResult>(PawnClient client, ITask<TResult> task) where TResult : IResult
        {
            Messenger.Publish(String.Format("{0}: Task {1}", client.ClientUrl, task.GetType().Name));
            var response = client.ExecuteTask(task);
            if (!response.Success)
            {
                Messenger.Publish(String.Format("Task not successfull: {0}\n{1}", response.ErrorMessage, response.Stacktrace));
                throw new TaskFailedException();
            }

            return response.Result;
        }

        protected Package GetPackage(Build build, string packageName)
        {
            Messenger.Publish("Retrieving package " + packageName);
            return build.GetPackage(packageName);
        }

        internal class TaskFailedException : Exception
        {
        }
    }
}