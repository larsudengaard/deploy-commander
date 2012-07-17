using Deploy.King.Projects;
using Deploy.Procedures.Builds;

namespace Deploy.King.Host.Models.Builds
{
    public class ProjectWithBuildInformationModel
    {
        public Project Project { get; set; }
        public BuildInformation BuildInformation { get; set; }
    }
}