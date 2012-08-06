using Deploy.Commander.Application.Projects;
using Deploy.Procedures.Builds;

namespace Deploy.Commander.Models.Builds
{
    public class ProjectWithBuildInformationModel
    {
        public Project Project { get; set; }
        public BuildInformation BuildInformation { get; set; }
    }
}