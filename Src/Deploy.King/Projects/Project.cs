using System.Collections.Generic;
using System.Linq;
using Deploy.Procedures.Arguments;

namespace Deploy.King.Projects
{
    public class Project : IProcedureArguments
    {
        readonly Dictionary<string, string> arguments;
        
        public Project()
        {
        }

        public Project(string name)
        {
            Name = name;
            arguments = new Dictionary<string, string>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        
        public IEnumerable<Argument> Arguments
        {
            get { return arguments.Select(x => new Argument(x.Key, x.Value)); }
        }

        public void ConfigureArgument(string name, string value)
        {
            arguments[name] = value;
        }

        public string GetArgument(string name)
        {
            string value;
            if (!arguments.TryGetValue(name, out value))
            {
                throw new MissingProcedureArgumentException(string.Format("Project configuration for project '{0}' is missing the argument named '{1}'", Name, name));
            }

            return value;
        }

        public class Argument
        {
            public Argument(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; private set; }
            public string Value { get; private set; }
        }
    }
}