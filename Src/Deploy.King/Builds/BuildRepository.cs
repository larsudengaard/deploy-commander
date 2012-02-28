using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
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
                Number = int.Parse(document.Root.Attribute("number").Value),
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
                    Number = int.Parse(build.Attribute("number").Value),
                    StartDate = Date(build.Attribute("startDate").Value)
                });
            }

            return builds.OrderByDescending(x => int.Parse(x.Id));
        }

        public string GetPackage(Build build)
        {
            var packageUrl = string.Format("{0}downloadArtifacts.html?buildId={1}", teamcityUrl, build.Id);
            var request = WebRequest.Create(packageUrl);
            string authInfo = "larsudengaard:MetteP83";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;

            request.Credentials = new NetworkCredential("larsudengaard", "MetteP83");

            WebResponse response;
            try
            {
                response = request.GetResponse();
            }
            catch (WebException e)
            {
                return null;
            }

            var responseStream = response.GetResponseStream();
            if (response.ContentLength == 0 || responseStream == null)
                return null;

            string fileName = AppSettings.GetPath("PackagePath") + build.Id + ".zip";
            using(var outputFileStream = File.Open(fileName, FileMode.CreateNew))
            using (responseStream)
            {
                var buffer = new byte[1024];
                int bytesRead;
                do
                {
                    bytesRead = responseStream.Read(buffer, 0, 1024);
                    outputFileStream.Write(buffer, 0, bytesRead);
                } while (bytesRead > 0);
            }

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