namespace Deploy.King.Procedures
{
    public interface IDeployProcedure<in TArguments>
    {
        bool Perform(DeployPackage package, TArguments arguments);
    }

    public class DeployPackage
    {
        public ChildPackage GetChildPackage(string packageName)
        {
            return new ChildPackage();
        }
    }

    public class ChildPackage
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