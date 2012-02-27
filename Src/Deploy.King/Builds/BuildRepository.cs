using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
            return new Build(this)
            {
                Id = id,
                StartDate = Date(document.Root.Element("startDate").Value),
            };
        }

        public IEnumerable<Build> GetBuildsFor(IProcedureArguments arguments)
        {
            var buildsXml = webClient.DownloadString(string.Format("{0}httpAuth/app/rest/buildTypes/id:{1}/builds?status=success", teamcityUrl, arguments.TeamcityBuildType));
            var document = XDocument.Parse(buildsXml);

            var builds = new List<Build>();
            foreach (var build in document.Root.Elements())
            {
                var id = build.Attribute("id").Value;
                builds.Add(new Build(this)
                {
                    Id = id,
                    StartDate = Date(build.Attribute("startDate").Value)
                });
            }

            return builds.OrderBy(x => x.StartDate);
        }

        public string GetPackage(Build build)
        {
            var packageUrl = string.Format("{0}downloadArtifacts.html?buildId={1}", teamcityUrl, build.Id);
            string fileName = AppSettings.GetPath("PackagePath") + build.Id + ".zip";
            webClient.DownloadFile(packageUrl, fileName);
            return fileName;
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