namespace Deploy.King.Procedures.Arguments
{
    public class ArgumentsForDeployEnergy10WithoutMigrations : IProcedureArguments
    {
        public string Web1PawnHostname { get; set; }
        public string WebsitePhysicalPath { get; set; }
        public string WebsiteName { get; set; }

        public bool DeployWeb2 { get; set; }
        public string Web2PawnHostname { get; set; }

        public string RavenDBPawnHostname { get; set; }
        
        public string TeamcityBuildType { get; set; }
    }
}