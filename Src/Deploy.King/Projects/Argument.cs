namespace Deploy.King.Projects
{
    public class Argument
    {
        public Argument(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
        public string Value { get; set; }
    }
}