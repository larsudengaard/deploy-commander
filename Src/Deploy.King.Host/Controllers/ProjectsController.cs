using System.Collections.Generic;
using System.Web.Mvc;
using Deploy.King.Builds;
using Deploy.King.Host.Models.Projects;
using Deploy.King.Projects;
using System.Linq;
using Deploy.Procedures.Builds;
using Raven.Client;

namespace Deploy.King.Host.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        readonly IDocumentStore store;
        readonly IBuildRepository buildRepository;

        public ProjectsController(IDocumentStore store, IBuildRepository buildRepository)
        {
            this.store = store;
            this.buildRepository = buildRepository;
        }

        public ActionResult Index()
        {
            List<Project> projects;
            using (var session = store.OpenSession())
            {
                projects = session.Query<Project>().ToList();
            }

            var models = projects.OrderBy(x => x.Id).Select(GetModelFromProject);
            return View(models.ToList());
        }

        public ActionResult Open(string id)
        {
            Project project;
            using (var session = store.OpenSession())
            {
                project = session.Load<Project>(id);
            }

            return View(GetModelFromProject(project));
        }

        ProjectWithBuildsModel GetModelFromProject(Project project)
        {
            var builds = buildRepository.GetBuildsFor(project.Arguments);
            var model = new ProjectWithBuildsModel(project, builds.ToList());
            return model;
        }

        public ActionResult Create()
        {
            return View("Edit", new ProjectModel());
        }

        public ActionResult Edit(string id)
        {
            using (var session = store.OpenSession())
            {
                var project = session.Load<Project>(id);
                return View(new ProjectModel
                {
                    Id = project.Id,
                    Name = project.Name,
                    ProcedureName = project.ProcedureName
                });
            }
        }

        [HttpPost]
        public ActionResult Edit(ProjectModel model)
        {
            if (ModelState.IsValid)
            {
                using (var session = store.OpenSession())
                {
                    if (model.Id != null)
                    {
                        var project = session.Load<Project>(model.Id);
                        project.Name = model.Name;
                        project.ProcedureName = model.ProcedureName;
                        project.Arguments.ConfigureArgument(TeamcityBuildRepository.TeamcityBuildTypeId, model.TeamcityBuildTypeId);
                    }
                    else
                    {
                        var project = new Project
                        {
                            Name = model.Name, ProcedureName = model.ProcedureName
                        };
                        project.Arguments.ConfigureArgument(TeamcityBuildRepository.TeamcityBuildTypeId, model.TeamcityBuildTypeId);
                        session.Store(project);
                    }
                    session.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            using (var session = store.OpenSession())
            {
                var project = session.Load<Project>(id);
                session.Delete(project);
            }

            return RedirectToAction("Index");
        }
    }
}