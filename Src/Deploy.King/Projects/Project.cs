namespace Deploy.King.Projects
{
    public class Project
    {
        public Project()
        {
            Arguments = new Arguments(this);
        }

        public string Id { get; private set; }
        public Arguments Arguments { get; private set; }
        
        public string Name { get; set; }
        public string ProcedureName { get; set; }
    }
}