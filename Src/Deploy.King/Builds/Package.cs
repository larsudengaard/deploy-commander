namespace Deploy.King.Builds
{
    public class Package
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }

        public string Fullname
        {
            get { return Name + Version; }
        }
    }
}