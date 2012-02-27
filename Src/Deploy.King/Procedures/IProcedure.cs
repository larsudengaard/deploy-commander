using System;
using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Procedures
{
    public abstract class Procedure<TArguments> : IProcedure<TArguments> where TArguments : IProcedureArguments
    {
        public abstract bool Perform(Build build, TArguments arguments);

        public bool Perform(Build build, IProcedureArguments arguments)
        {
            return Perform(build, (TArguments) arguments);
        }
    }

    public interface IProcedure<TArguments> : IProcedure where TArguments : IProcedureArguments
    {
    }

    public interface IProcedure
    {
        bool Perform(Build build, IProcedureArguments arguments);
    }

    public class Build
    {
        public string Id { get; set; }
        public DateTime Received { get; set; }

        public Package GetPackage(string packageName)
        {
            return new Package();
        }
    }

    public class Package
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