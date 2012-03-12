using System.Diagnostics;
using System.Web.Mvc;
using Deploy.King.Builds;
using Deploy.King.Host.Infrastructure;
using Deploy.King.Host.Infrastructure.Poll;
using Deploy.King.Messaging;
using Deploy.King.Projects;
using Raven.Client;
using Message = Deploy.King.Messaging.Message;

namespace Deploy.King.Host.Controllers
{
    [Authorize]
    public class BuildController : AsyncController, IMessageListener
    {
        readonly IDocumentStore store;
        readonly IProcedureFactory procedureFactory;
        readonly IBuildRepository buildRepository;
        readonly IMessageSubscriber messageSubscriber;
        string listeningToProjectId;
        string listeningToBuildId;

        public BuildController(IDocumentStore store, IProcedureFactory procedureFactory, IBuildRepository buildRepository, IMessageSubscriber messageSubscriber)
        {
            this.store = store;
            this.procedureFactory = procedureFactory;
            this.buildRepository = buildRepository;
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
            return View(new Model
            {
                Project = new Model.ProjectModel(project, procedureFactory.CreateFor(project.Arguments).Name),
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

            var build = buildRepository.GetBuild(buildId);
            var procedure = procedureFactory.CreateFor(project.Arguments);

            listeningToProjectId = projectId;
            listeningToBuildId = buildId;
            messageSubscriber.AddListener(this);
            var stopwatch = Stopwatch.StartNew();
            var success = procedure.Perform(build, project.Arguments);
            stopwatch.Stop();
            messageSubscriber.RemoveListener(this);

            return new JsonNetResult
            {
                Data = new
                {
                    Success = success,
                    Time = stopwatch.Elapsed.ToString()
                }
            };
        }

        public void HandleMessage(Message message)
        {
            Bus.Publish(new Infrastructure.Poll.Message(listeningToProjectId, listeningToBuildId, message));
        }

        public class Model
        {
            public BuildInformation BuildInformation { get; set; }
            public ProjectModel Project { get; set; }

            public class ProjectModel
            {
                public ProjectModel(Project project, string procedureType)
                {
                    Id = project.Id;
                    Name = project.Name;
                    ProcedureType = procedureType;
                }

                public string Id { get; set; }
                public string Name { get; set; }
                public string ProcedureType { get; set; }
            }
        }
    }
}