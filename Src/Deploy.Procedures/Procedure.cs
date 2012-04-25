using System;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;
using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;

namespace Deploy.Procedures
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

        // TODO: Make DI pawnclient factory, and make pawnclient print for it self, removeing the need for this method:
        protected TResult ExecuteTask<TResult>(PawnClient client, ITask<TResult> task) where TResult : IResult
        {
            Messenger.Publish(String.Format("{0}: Task {1}", client.ClientUrl, task.GetType().Name));
            var response = client.ExecuteTask(task);
            if (!response.Success)
            {
                Messenger.Publish(String.Format("Task error: {0}\n{1}", response.ErrorMessage, response.StackTrace));
                throw new TaskFailedException();
            }

            return response.Result;
        }

        protected Package GetPackage(Build build, string packageName)
        {
            Messenger.Publish("Retrieving package " + packageName);
            return build.GetPackage(packageName);
        }

        public class TaskFailedException : Exception
        {
        }
    }
}