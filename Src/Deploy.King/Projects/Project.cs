using Deploy.King.Procedures.Arguments;

namespace Deploy.King.Projects
{
    public class Project
    {
        public Project(string name, string procedureType, IProcedureArguments arguments)
        {
            Name = name;
            ProcedureType = procedureType;
            Arguments = arguments;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string ProcedureType { get; set; }
        public IProcedureArguments Arguments { get; set; }
    }
}