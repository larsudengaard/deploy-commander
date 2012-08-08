using System;
using System.Reflection;
using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;

namespace Deploy.Procedures
{
    public abstract class Procedure<TArguments> : IProcedure where TArguments : new()
    {
        readonly IMessenger messenger;
        readonly ISoldierFactory soldierFactory;

        protected Procedure(IMessenger messenger, ISoldierFactory soldierFactory)
        {
            this.messenger = messenger;
            this.soldierFactory = soldierFactory;
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        protected ISoldierFactory SoldierFactory
        {
            get { return soldierFactory; }
        }

        protected IMessenger Messenger
        {
            get { return messenger; }
        }

        public abstract bool Perform(Build build, TArguments arguments);

        public bool Perform(IProject project, Build build)
        {
            try
            {
                var arguments = new TArguments();
                foreach (var property in arguments.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (!property.CanWrite)
                        continue;

                    var argument = project.GetArgument(property.Name);
                    
                    if (property.PropertyType == typeof(Path))
                        property.SetValue(arguments, new Path(argument), null);
                    else
                        property.SetValue(arguments, Convert.ChangeType(argument, property.PropertyType), null);
                }

                Messenger.Publish("Start procedure " + GetType().Name);
                if (Perform(build, arguments))
                {
                    Messenger.Publish("Procedure completed successfully " + GetType().Name);
                    return true;
                }
            }
            catch (TaskFailedException e)
            {
                Messenger.Publish(string.Format("Procedure failed {0}, Task {1}", GetType().Name, e.Task.GetType().Name));
            }
            catch(Exception e)
            {
                Messenger.Publish("Procedure encountered an unhandled error: " + e.Message + "\n" + e.StackTrace);
            }

            return false;
        }

        protected Package GetPackage(Build build, string packageName)
        {
            Messenger.Publish("Retrieving package " + packageName);
            return build.GetPackage(packageName);
        }
    }
}