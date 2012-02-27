using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Xml.Linq;
using Deploy.King.Procedures.Arguments;
using Deploy.Utilities;
using System.Linq;
using Raven.Abstractions;

namespace Deploy.King.Builds
{
    public class BuildRepository : IBuildRepository
    {
        readonly WebClient webClient;
        readonly string teamcityUrl;

        public BuildRepository()
        {
            webClient = new WebClient();
            webClient.Credentials = new NetworkCredential("larsudengaard", "MetteP83");

            teamcityUrl = AppSettings.GetString("TeamcityUrl");
            teamcityUrl += teamcityUrl.Last() != '/' ? "/" : "";
        }

        public Build GetBuild(string id)
        {
            var build = webClient.DownloadString(string.Format("{0}httpAuth/app/rest/builds/id:{1}", teamcityUrl, id));
            if (build.StartsWith("Error has occurred during request processing (Not Found)."))
                return null;

            var document = XDocument.Parse(build);
            return new Build
            {
                Id = id,
                Received = Date(document.Root.Element("startDate").Value),
                PackageUrl = string.Format("{0}downloadArtifacts.html?buildId={1}", teamcityUrl, id)
            };
        }

        public IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments)
        {
            var builds = webClient.DownloadString(string.Format("{0}httpAuth/app/rest/buildTypes/id:{1}/builds", teamcityUrl, arguments.TeamcityBuildType));
            var document = XDocument.Parse(builds);

            foreach (var build in document.Root.Elements())
            {
                var id = build.Attribute("id").Value;
                yield return new Build
                {
                    Id = id,
                    Received = Date(build.Attribute("startDate").Value),
                    PackageUrl = string.Format("{0}downloadArtifacts.html?buildId={1}", teamcityUrl, id)
                };
            }
        }

        public static DateTime Date(string s)
        {
            DateTime time;
            if (DateTime.TryParseExact(s, "yyyyMMddTHHmmsszzz", CultureInfo.InvariantCulture,
                                       DateTimeStyles.RoundtripKind, out time))
            {
                if (time.Kind == DateTimeKind.Unspecified)
                    return DateTime.SpecifyKind(time, DateTimeKind.Local);
                return time;
            }

            throw new ArgumentException("Not a valid DateTime: " + s, "s");
        }
    }
}