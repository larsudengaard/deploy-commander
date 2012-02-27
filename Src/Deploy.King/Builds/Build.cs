using System;
using System.IO;
using System.Net;
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
        public DateTime StartDate { get; set; }

        public Package GetPackage(string packageName)
        {
            string packagesPath = AppSettings.GetPath("packagesPath") + "\\" + Id;
            if (!Directory.Exists(packagesPath))
            {
                byte[] package = buildRepository.GetPackage(this);
                Zip.Extract(new MemoryStream(package), packagesPath);
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