using Deploy.Procedures.Arguments;

namespace Deploy.Procedures.Examples.Arguments
{
    public interface IEnergy10BaseArguments
    {
        string WebsiteName { get; }
        string WebsitePhysicalPath { get; }
        string RavenDBPawnHostname { get; }
    }
}