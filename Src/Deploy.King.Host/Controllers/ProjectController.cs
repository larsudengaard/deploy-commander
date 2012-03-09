using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Deploy.King.Builds;
using Deploy.King.Host.Models.Project;
using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;
using Deploy.King.Projects;
using System.Linq;
using Raven.Client;

namespace Deploy.King.Host.Controllers
{
    public class ProjectController : Controller
    {
        readonly IDocumentStore store;
        readonly IProcedureFactory procedureFactory;
        readonly IBuildRepository buildRepository;

        public ProjectController(IDocumentStore store, IProcedureFactory procedureFactory, IBuildRepository buildRepository)
        {
            this.store = store;
            this.procedureFactory = procedureFactory;
            this.buildRepository = buildRepository;
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

        public ActionResult Index()
        {
            List<Project> projects;
            using (var session = store.OpenSession())
            {
                projects = session.Query<Project>().ToList();
            }

            var models = new List<ListModel>();
            foreach (var project in projects)
            {
                models.Add(GetModelFromProject(project));
            }

            return View(models);
        }

        ListModel GetModelFromProject(Project project)
        {
            var procedure = procedureFactory.CreateFor(project.Arguments);
            var builds = buildRepository.GetBuildsFor(project.Arguments);
            var model = new ListModel(project, procedure.Name, builds.Select(x => new ListModel.Build(x)).ToList());
            return model;
        }

        public ActionResult Create()
        {
            return View(new SelectProcedureType());
        }

        [HttpPost]
        public ActionResult CreateArguments(SelectProcedureType selection)
        {
            var procedureType = typeof (IProcedure).Assembly.GetType(selection.ProcedureType, false, true);
            var argumentsType = procedureType.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IProcedure<>)).GetGenericArguments()[0];
            var arguments = Activator.CreateInstance(argumentsType);
            
            return View(arguments);
        }

        [HttpPost]
        public ActionResult ArgumentsForDeployEnergy10(string id, string name, ArgumentsForDeployEnergy10 arguments)
        {
            CreateOrEditProject(id, name, arguments);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ArgumentsForDeployEnergy10WithMigrations(string id, string name, ArgumentsForDeployEnergy10WithMigrations arguments)
        {
            CreateOrEditProject(id, name, arguments);
            return RedirectToAction("Index");
        }

        void CreateOrEditProject(string id, string name, IProcedureArguments arguments)
        {
            using (var session = store.OpenSession())
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    session.Store(new Project(name, arguments));
                }
                else
                {
                    var project = session.Load<Project>(id);
                    project.Name = name;
                    project.Arguments = arguments;
                }

                session.SaveChanges();
            }
        }

        public ActionResult Edit(string id)
        {
            using (var session = store.OpenSession())
            {
                var project = session.Load<Project>(id);
                ViewBag.Id = project.Id;
                ViewBag.Name = project.Name;
                return View(project.Arguments);
            }
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            return RedirectToAction("Index");
        }
    }
}