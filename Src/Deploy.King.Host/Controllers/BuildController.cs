using System.Diagnostics;
using System.Web.Mvc;
using Deploy.King.Executor;
using Deploy.King.Host.Infrastructure;
using Deploy.King.Host.Infrastructure.Poll;
using Deploy.King.Projects;
using Deploy.Procedures.Builds;
using Raven.Client;

namespace Deploy.King.Host.Controllers
{
    [Authorize]
    public class BuildController : AsyncController
    {
        readonly IDocumentStore store;
        readonly IBuildRepository buildRepository;
        readonly ProcedureExecutor procedureExecutor;
        readonly IPollMessengerFactory pollMessengerFactory;

        public BuildController(IDocumentStore store, IBuildRepository buildRepository, ProcedureExecutor procedureExecutor, IPollMessengerFactory pollMessengerFactory)
        {
            this.store = store;
            this.buildRepository = buildRepository;
            this.procedureExecutor = procedureExecutor;
            this.pollMessengerFactory = pollMessengerFactory;
        }

        public ActionResult Index(string projectId, string buildId)
        {
            Project project;
            using (var session = store.OpenSession())
            {
                project = session.Load<Project>(projectId);
            }

            var buildInformation = buildRepository.GetBuildInformation(buildId);
            return View(new Model
            {
                Project = new Model.ProjectModel(project),
                BuildInformation = buildInformation,
            });
        }

        public ActionResult Deploy(string projectId, string buildId)
        {
            Project project;
            using (var session = store.OpenSession())
            {
                project = session.Load<Project>(projectId);
            }

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            bool success;
            using (pollMessengerFactory.CreateMessenger(projectId, buildId))
            {
                success = procedureExecutor.Execute(project, buildId);
            }

            stopwatch.Stop();

            return new JsonNetResult
            {
                Data = new
                {
                    Success = success,
                    Time = stopwatch.Elapsed.ToString()
                }
            };
        }

        public class Model
        {
            public BuildInformation BuildInformation { get; set; }
            public ProjectModel Project { get; set; }

            public class ProjectModel
            {
                public ProjectModel(Project project)
                {
                    Id = project.Id;
                    Name = project.Name;
                }

                public string Id { get; set; }
                public string Name { get; set; }
                public string ProcedureType { get; set; }
            }
        }
    }
}