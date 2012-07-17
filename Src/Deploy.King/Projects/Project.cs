namespace Deploy.King.Projects
{
    public class Project
    {
        readonly Arguments arguments;
        
        public Project()
        {
        }

        public Project(string name)
        {
            Name = name;
            arguments = new Arguments(this);
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public Arguments Arguments
        {
            get { return arguments; }
        }
    }
}