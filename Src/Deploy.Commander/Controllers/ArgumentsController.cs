using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Deploy.Commander.Application.Assemblies;
using Deploy.Commander.Application.Projects;
using Deploy.Commander.Models.Arguments;
using Deploy.Procedures;
using Deploy.Procedures.Builds;
using Raven.Client;

namespace Deploy.Commander.Controllers
{
    public class ArgumentsController : Controller
    {
        readonly IDocumentStore store;
        readonly IBuildRepository buildRepository;
        readonly ProcedureLoader procedureLoader;

        public ArgumentsController(IDocumentStore store, IBuildRepository buildRepository, ProcedureLoader procedureLoader)
        {
            this.store = store;
            this.buildRepository = buildRepository;
            this.procedureLoader = procedureLoader;
        }

        public ActionResult Create(string projectId)
        {
            return View(new ArgumentModel
            {
                ProjectId = projectId            
            });
        }

        public ActionResult GenerateArgumentsFromBuild(string projectId, string buildId)
        {
            var build = buildRepository.GetBuild(buildId);
            Project project;
            using (var session = store.OpenSession())
            {
                project = session.Load<Project>(projectId);
            }

            var procedure = procedureLoader.GetProcedure(project, build);
            var procedureType = procedure.GetType();
            var arguments = new List<ArgumentModel>();
            while (procedureType != typeof(object))
            {
                if (procedureType.IsGenericType && procedureType.GetGenericTypeDefinition() == typeof(Procedure<>))
                {
                    var argumentType = procedureType.GetGenericArguments()[0];
                    foreach (var property in argumentType.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.CanWrite && x.CanRead))
                    {
                        string existingValue;
                        project.TryGetArgument(property.Name, out existingValue);
                        arguments.Add(new ArgumentModel
                        {
                            ProjectId = projectId,
                            Name = property.Name,
                            Value = existingValue
                        });
                    }
                    break;
                }

                procedureType = procedureType.BaseType;
            }

            ViewBag.ProjectId = projectId;
            return View(arguments);
        }

        [HttpPost]
        public ActionResult GenerateArgumentsFromBuild(string projectId, List<ArgumentModel> arguments)
        {
            if (ModelState.IsValid)
            {
                using (var session = store.OpenSession())
                {
                    var project = session.Load<Project>(projectId);
                    foreach (var argument in arguments)
                    {
                        project.ConfigureArgument(argument.Name, argument.Value);
                    }

                    session.SaveChanges();
                    return RedirectToAction("Open", "Projects", new { id = projectId });
                }
            }

            ViewBag.ProjectId = projectId;
            return View(arguments);
        }

        public ActionResult Edit(string projectId, string argumentName)
        {
            using (var session = store.OpenSession())
            {
                var project = session.Load<Project>(projectId);
                string argumentValue;
                project.TryGetArgument(argumentName, out argumentValue);

                return View(new ArgumentModel
                {
                    ProjectId = project.Id,
                    Name = argumentName,
                    Value = argumentValue
                });
            }
        }

        [HttpPost]
        public ActionResult Edit(ArgumentModel argument)
        {
            if (ModelState.IsValid)
            {
                using (var session = store.OpenSession())
                {
                    var project = session.Load<Project>(argument.ProjectId);
                    project.ConfigureArgument(argument.Name, argument.Value);
                    session.SaveChanges();
                    return RedirectToAction("Open", "Projects", new { id = argument.ProjectId });
                }
            }

            if (string.IsNullOrWhiteSpace(argument.Name))
                return View("Create", argument);

            return View(argument);
        }

        public ActionResult Delete(string projectId, string argumentName)
        {
            using (var session = store.OpenSession())
            {
                var project = session.Load<Project>(projectId);
                project.RemoveArgument(argumentName);
                session.SaveChanges();
            }

            return RedirectToAction("Open", "Projects", new { id = projectId });
        }
    }
}