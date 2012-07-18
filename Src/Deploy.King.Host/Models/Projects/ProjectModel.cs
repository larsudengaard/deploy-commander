using System.ComponentModel.DataAnnotations;

namespace Deploy.King.Host.Models.Projects
{
    public class ProjectModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Project Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Name of deploypackage is required")]
        public string DeployPackageName { get; set; }

        [Required(ErrorMessage = "Procedure Name is required")]
        public string ProcedureName { get; set; }

        [Required(ErrorMessage = "TeamcityBuildTypeId is required")]
        public string TeamcityBuildTypeId { get; set; }
    }
}