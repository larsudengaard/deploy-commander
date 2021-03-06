using System.Collections.Generic;

namespace Deploy.Procedures.Builds
{
    public class BuildInformation
    {
        public BuildInformation()
        {
            Changes = new List<Change>();
        }

        public Build Build { get; set; }
        public string Link { get; set; }
        public List<Change> Changes { get; set; }

        public class Change
        {
            public string Username { get; set; }
            public string Comment { get; set; }
        }
    }
}