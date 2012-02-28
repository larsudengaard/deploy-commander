using System;
using System.IO;
using Deploy.Utilities;

namespace Deploy.King.Builds
{
    public class Build
    {
        private readonly IBuildRepository buildRepository;

        public Build(IBuildRepository buildRepository)
        {
            this.buildRepository = buildRepository;
        }

        public string Id { get; set; }
        public int Number { get; set; }
        public DateTime StartDate { get; set; }

        public Package GetPackage(string packageName)
        {
            string packagesPath = AppSettings.GetPath("PackagePath") + Id;
            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
                var packageFilename = buildRepository.GetPackage(this);
                Zip.Extract(File.Open(packageFilename, FileMode.Open), packagesPath);
            }

            string childPackageFilename = packagesPath + "\\" + packageName + ".zip";
            if (!File.Exists(childPackageFilename))
                return null;

            return new Package
            {
                Name = packageName,
                Path = childPackageFilename
            };
        }
    }
}