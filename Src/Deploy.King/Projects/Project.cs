using Deploy.Procedures.Arguments;

namespace Deploy.King.Projects
{
    public class Project
    {
        public Project()
        {
        }

        public Project(string name, IProcedureArguments arguments)
        {
            Name = name;
            Arguments = arguments;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public IProcedureArguments Arguments { get; set; }
    }
}