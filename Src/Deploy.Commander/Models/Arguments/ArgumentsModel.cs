using System.ComponentModel.DataAnnotations;

namespace Deploy.Commander.Models.Arguments
{
    public class ArgumentModel
    {
        public string ProjectId { get; set; }

        [Required(ErrorMessage = "Name is required", AllowEmptyStrings = false)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Value is required", AllowEmptyStrings = false)]
        public string Value { get; set; }
    }
}