using System.Collections.Generic;
using System.Linq;
using Deploy.Procedures.Arguments;

namespace Deploy.King.Projects
{
    public class Arguments : IProcedureArguments
    {
        readonly List<Argument> arguments;
        readonly Project project;

        public Arguments(Project project)
        {
            this.project = project;
            arguments = new List<Argument>();
        }

        public void ConfigureArgument(string argumentName, string value)
        {
            var argument = arguments.SingleOrDefault(x => x.Name == argumentName);
            if (argument == null)
            {
                argument = new Argument(argumentName);
                arguments.Add(argument);
            }
            argument.Value = value;
        }

        public string GetArgument(string name)
        {
            var argument = arguments.SingleOrDefault(x => x.Name == name);
            if (argument == null)
            {
                throw new MissingProcedureArgumentException(string.Format("Project configuration for project '{0}' is missing the argument named '{1}'", project.Name, name));
            }

            return argument.Value;
        }

        public IEnumerable<Argument> All
        {
            get { return arguments; }
        }
    }
}