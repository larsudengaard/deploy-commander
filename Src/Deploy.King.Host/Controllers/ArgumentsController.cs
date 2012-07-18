using System.Web.Mvc;
using Deploy.King.Host.Models.Arguments;
using Deploy.King.Projects;
using Raven.Client;
using System.Linq;

namespace Deploy.King.Host.Controllers
{
    public class ArgumentsController : Controller
    {
        readonly IDocumentStore store;

        public ArgumentsController(IDocumentStore store)
        {
            this.store = store;
        }

        public ActionResult Create(string projectId)
        {
            return View(new ArgumentModel
            {
                ProjectId = projectId            
            });
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