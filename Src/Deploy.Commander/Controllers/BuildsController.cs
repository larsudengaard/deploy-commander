using System.Diagnostics;
using System.Web.Mvc;
using Deploy.Commander.Application.Executor;
using Deploy.Commander.Application.Messaging;
using Deploy.Commander.Application.Projects;
using Deploy.Commander.Infrastructure;
using Deploy.Commander.Infrastructure.Poll;
using Deploy.Commander.Models.Builds;
using Deploy.Procedures.Builds;
using Raven.Client;

namespace Deploy.Commander.Controllers
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