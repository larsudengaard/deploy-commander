using System;

namespace Deploy.Procedures.Builds
{
    public class Build
    {
        readonly IBuildRepository repository;

        public Build(IBuildRepository repository)
        {
            this.repository = repository;
        }

        public string Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }

        public Package GetPackage(string packageName)
        {
            return repository.GetPackage(this, packageName);
        }
    }
}