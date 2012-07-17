using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Deploy.Procedures.Arguments;

namespace Deploy.King.Projects
{
    public class Arguments : IEnumerable<Argument>, IProcedureArguments
    {
        readonly Project project;
        readonly Dictionary<string, string> arguments;

        public Arguments(Project project)
        {
            this.project = project;
            arguments = new Dictionary<string, string>();
        }

        public void ConfigureArgument(string argumentName, string value)
        {
            arguments[argumentName] = value;
        }

        public string GetArgument(string name)
        {
            string value;
            if (!arguments.TryGetValue(name, out value))
            {
                throw new MissingProcedureArgumentException(string.Format("Project configuration for project '{0}' is missing the argument named '{1}'", project.Name, name));
            }

            return value;
        }

        public IEnumerator<Argument> GetEnumerator()
        {
            return arguments.Select(x => new Argument(x.Key, x.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}