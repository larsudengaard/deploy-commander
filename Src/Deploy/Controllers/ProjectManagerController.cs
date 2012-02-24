using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Deploy.King.Procedures;
using Deploy.King.Procedures.Arguments;
using Deploy.King.Projects;
using Deploy.Models.Project;
using System.Linq;
using Raven.Client;

namespace Deploy.Controllers
{
    public class ProjectManagerController : Controller
    {
        readonly IDocumentStore store;

        public ProjectManagerController(IDocumentStore store)
        {
            this.store = store;
        }

        public ActionResult Index()
        {
            //var projects = session.Query<Project>().ToList().Select(x => );

            return View(new List<ProjectModel>
            {
                new ProjectModel
                {
                    Id = "1",
                    Name = "Energy10 without migrations",
                    ProcedureType = typeof(DeployEnergy10WithoutMigrations).Name
                }
            });
        }

        public ActionResult Create()
        {
            return View(new SelectProcedureType());
        }

        [HttpPost]
        public ActionResult CreateArguments(SelectProcedureType selection)
        {
            var procedureType = typeof (IDeployProcedure).Assembly.GetType(selection.ProcedureType, false, true);
            var argumentsType = procedureType.GetInterfaces().Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof (IDeployProcedure<>)).GetGenericArguments()[0];
            var arguments = Activator.CreateInstance(argumentsType);

            ViewBag.ProcedureType = procedureType.Name;
            
            return View(arguments);
        }

        [HttpPost]
        public ActionResult CreateCompleted(string name, string procedureType, ArgumentsForDeployEnergy10WithoutMigrations arguments)
        {
            using (var session = store.OpenSession())
            {
                session.Store(new Project(name, procedureType, arguments));
                session.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(ProjectModel model)
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            return RedirectToAction("Index");
        }
    }
}