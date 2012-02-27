using System;

namespace Deploy.King.Builds
{
    public class Build
    {
        public Build()
        {
            
        }

        public string Id { get; set; }
        public DateTime Received { get; set; }
        public string PackageUrl { get; set; }

        public Package GetPackage(string packageName)
        {
            return new Package();
        }
    }
}