using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Deploy.King;
using Deploy.King.Builds;
using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;
using Deploy.King.Projects;
using Deploy.Models.Project;
using System.Linq;
using Raven.Client;

namespace Deploy.Controllers
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
                var procedure = procedureFactory.CreateFor(project.Arguments);
                var builds = buildRepository.GetBuildsFor(procedure);
                models.Add(new ListModel(project, procedure.GetType().Name, builds.Select(x => new ListModel.Build(x)).ToList()));
            }

            return View(models);
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
        public ActionResult Completed(string id, string name, ArgumentsForDeployEnergy10WithoutMigrations arguments)
        {
            using (var session = store.OpenSession())
            {
                if (id == null)
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

            return RedirectToAction("Index");
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