using Deploy.King.Builds;
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

        public string Name
        {
            get { return GetType().Name; }
        }
    }
}