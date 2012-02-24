using System;
using System.ComponentModel;

namespace Deploy.Models.Project
{
    public class ProjectModel
    {
        public string Id { get; set; }

        [DisplayName("Navn")]
        public string Name { get; set; }

        [DisplayName("Procedure")]
        public string ProcedureType { get; set; }
    }
}