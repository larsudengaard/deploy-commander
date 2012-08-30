using System.Collections.Generic;
using System.Linq;
using Deploy.Procedures.Arguments;

namespace Deploy.Commander.Application.Projects
{
    public class Project : IProject
    {
        public Project()
        {
            Arguments = new List<Argument>();
        }

        public string Id { get; private set; }
        public List<Argument> Arguments { get; private set; }
        
        public string Name { get; set; }
        public string DeployPackageName { get; set; }
        public string ProcedureName { get; set; }

        public void RemoveArgument(string argumentName)
        {
            var argument = Arguments.SingleOrDefault(x => x.Name == argumentName);
            if (argument != null)
            {
                Arguments.Remove(argument);
            }
        }

        public void ConfigureArgument(string argumentName, string value)
        {
            var argument = Arguments.SingleOrDefault(x => x.Name == argumentName);
            if (argument == null)
            {
                argument = new Argument(argumentName);
                Arguments.Add(argument);
            }
            argument.Value = value;
        }

        public bool TryGetArgument(string name, out string value)
        {
            value = null;
            var argument = Arguments.SingleOrDefault(x => x.Name == name);
            if (argument == null)
            {
                return false;
            }

            value = argument.Value;
            return true;
        }

        public string GetArgument(string name)
        {
            string value;
            if (!TryGetArgument(name, out value))
            {
                throw new MissingProcedureArgumentException(string.Format("Project configuration for project '{0}' is missing the argument named '{1}'", Name, name));
            }

            return value;
        }
    }
}