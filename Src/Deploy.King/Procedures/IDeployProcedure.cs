using System.Collections.Generic;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Procedures
{
    public abstract class DeployProcedure<TArguments> : IDeployProcedure<TArguments> where TArguments : IProcedureArguments
    {
        public abstract bool Perform(DeployPackage package, TArguments arguments);

        public bool Perform(DeployPackage package, IProcedureArguments arguments)
        {
            return Perform(package, (TArguments) arguments);
        }
    }

    public interface IDeployProcedure<TArguments> : IDeployProcedure where TArguments : IProcedureArguments
    {
    }

    public interface IDeployProcedure
    {
        bool Perform(DeployPackage package, IProcedureArguments arguments);
    }

    public class DeployPackage
    {
        public ChildPackage GetChildPackage(string packageName)
        {
            return new ChildPackage();
        }
    }

    public class ChildPackage
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public string Fullname
        {
            get { return Name + Version; }
        }
    }
}