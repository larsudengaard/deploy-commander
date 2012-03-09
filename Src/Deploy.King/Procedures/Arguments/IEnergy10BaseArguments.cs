namespace Deploy.King.Procedures.Arguments
{
    public interface IEnergy10BaseArguments : IProcedureArguments
    {
        string WebsiteName { get; }
        string WebsitePhysicalPath { get; }
        string RavenDBPawnHostname { get; }
    }
}