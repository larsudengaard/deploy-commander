using System.Web.Mvc;
using Deploy.King;
using Deploy.King.Builds;
using Deploy.King.Projects;
using Raven.Client;

namespace Deploy.Controllers
{
    public class BuildController : Controller
    {
        readonly IDocumentStore store;
        readonly IProcedureFactory procedureFactory;
        readonly IBuildRepository buildRepository;

        public BuildController(IDocumentStore store, IProcedureFactory procedureFactory, IBuildRepository buildRepository)
        {
            this.store = store;
            this.procedureFactory = procedureFactory;
            this.buildRepository = buildRepository;
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
            procedure.Perform(build, project.Arguments);

            return RedirectToAction("Index", "Project");
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