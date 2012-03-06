namespace Deploy.King.Procedures.Arguments
{
    public interface IDeployEnergy10BaseArguments : IProcedureArguments
    {
        string WebsiteName { get; }
        string WebsitePhysicalPath { get; }
        string RavenDBPawnHostname { get; }
    }
}