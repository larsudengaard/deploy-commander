using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Deploy.Procedures.Arguments;
using Deploy.Procedures.Builds;
using Deploy.Utilities;

namespace Deploy.King.Builds
{
    public class TeamcityBuildRepository : IBuildRepository
    {
        public const string TeamcityBuildTypeId = "TeamcityBuildTypeId";

        readonly string teamcityUrl;
        readonly string packagePath;
        readonly WebClient webClient;

        public TeamcityBuildRepository(string teamcityUrl, string packagePath)
        {
            webClient = new WebClient();
            webClient.Credentials = new NetworkCredential("larsudengaard", "MetteP83");

            this.teamcityUrl = teamcityUrl;
            this.packagePath = packagePath;
            this.teamcityUrl = teamcityUrl.Last() == '/' ? teamcityUrl.Substring(0, teamcityUrl.Length-1) : teamcityUrl;
        }

        public Build GetBuild(string id)
        {
            XDocument document = Query("/httpAuth/app/rest/builds/id:{1}", id);
            if (document == null)
                return null;

            return GetBuildFromElement(document.Root);
        }

        Build GetBuildFromElement(XElement element)
        {
            string startDate;
            if (element.Element("startDate") != null)
                startDate = element.Element("startDate").Value;
            else
                startDate = element.Attribute("startDate").Value;

            return new Build(this)
            {
                Id = element.Attribute("id").Value,
                Number = element.Attribute("number").Value,
                StartDate = Date(startDate),
            };
        }

        public IEnumerable<Build> GetBuildsFor(IProject arguments)
        {
            var teamcityBuildTypeId = arguments.GetArgument(TeamcityBuildTypeId);
            XDocument document = Query("/httpAuth/app/rest/buildTypes/id:{1}/builds?status=success", teamcityBuildTypeId);
            if (document == null)
                return Enumerable.Empty<Build>();

            var builds = new List<Build>();
            foreach (XElement build in document.Root.Elements())
            {
                builds.Add(GetBuildFromElement(build));
            }

            return builds.OrderByDescending(x => int.Parse(x.Id));
        }

        public Package GetPackage(Build build, string packageName)
        {
            string packagesPath = packagePath + build.Id;
            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
                var packageFilename = GetRawPackage(build);
                Zip.Extract(File.Open(packageFilename, FileMode.Open), packagesPath);
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

        string GetRawPackage(Build build)
        {
            string packageUrl = string.Format("{0}/downloadArtifacts.html?buildId={1}", teamcityUrl, build.Id);
            WebRequest request = WebRequest.Create(packageUrl);
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

            Stream responseStream = response.GetResponseStream();
            if (response.ContentLength == 0 || responseStream == null)
                return null;

            string fileName = packagePath + build.Id + ".zip";
            using (FileStream outputFileStream = File.Open(fileName, FileMode.CreateNew))
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

        public BuildInformation GetBuildInformation(string buildId)
        {
            XDocument buildDocument = Query("/httpAuth/app/rest/builds/id:{1}", buildId);
            if (buildDocument == null)
                return null;

            XDocument changesDocument = Query(buildDocument.Root.Element("changes").Attribute("href").Value);
            if (changesDocument == null)
                return null;

            var buildInformation = new BuildInformation
            {
                Build = GetBuildFromElement(buildDocument.Root),
                Link = buildDocument.Root.Attribute("webUrl").Value
            };
            foreach (XElement changeReferenceDocument in changesDocument.Root.Elements())
            {
                XDocument changeDocument = Query(changeReferenceDocument.Attribute("href").Value);
                buildInformation.Changes.Add(new BuildInformation.Change
                {
                    Comment = changeDocument.Root.Element("comment").Value,
                    Username = changeDocument.Root.Attribute("username").Value
                });
            }

            return buildInformation;
        }

        XDocument Query(string apiUrl, params object[] args)
        {
            var objects = new List<object>(args);
            objects.Insert(0, teamcityUrl);

            string result;
            try
            {
                result = webClient.DownloadString(string.Format("{0}" + apiUrl, objects.ToArray()));
            }
            catch (WebException e)
            {
                return null;
            }

            if (result.StartsWith("Error has occurred during request processing (Not Found)."))
                return null;

            return XDocument.Parse(result);
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