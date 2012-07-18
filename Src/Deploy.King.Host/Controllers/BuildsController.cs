using System.Diagnostics;
using System.Web.Mvc;
using Deploy.King.Executor;
using Deploy.King.Host.Infrastructure;
using Deploy.King.Host.Infrastructure.Poll;
using Deploy.King.Host.Models.Builds;
using Deploy.King.Messaging;
using Deploy.King.Projects;
using Deploy.Procedures.Builds;
using Raven.Client;

namespace Deploy.King.Host.Controllers
{
    [Authorize]
    public class BuildsController : AsyncController
    {
        readonly IDocumentStore store;
        readonly IBuildRepository buildRepository;
        readonly ProcedureExecutor procedureExecutor;
        readonly IMessageSubscriber messageSubscriber;

        public BuildsController(IDocumentStore store, IBuildRepository buildRepository, ProcedureExecutor procedureExecutor, IMessageSubscriber messageSubscriber)
        {
            this.store = store;
            this.buildRepository = buildRepository;
            this.procedureExecutor = procedureExecutor;
            this.messageSubscriber = messageSubscriber;
        }

        public ActionResult Index(string projectId, string buildId)
        {
            Project project;
            using (var session = store.OpenSession())
            {
                project = session.Load<Project>(projectId);
            }

            var buildInformation = buildRepository.GetBuildInformation(buildId);
            return View(new ProjectWithBuildInformationModel
            {
                Project = project,
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
            using (new PollChannel(messageSubscriber, projectId, buildId))
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
    }
}