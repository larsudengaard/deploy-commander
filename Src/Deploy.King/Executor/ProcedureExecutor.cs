using System;
using System.Reflection;
using Deploy.King.Projects;
using Deploy.Procedures;
using Deploy.Procedures.Builds;
using System.Linq;
using Deploy.Procedures.Messaging;

namespace Deploy.King.Executor
{
    public class ProcedureExecutor
    {
        readonly IBuildRepository buildRepository;
        readonly IMessenger messenger;

        public ProcedureExecutor(IBuildRepository buildRepository, IMessenger messenger)
        {
            this.buildRepository = buildRepository;
            this.messenger = messenger;
        }

        public bool Execute(Project project, string buildId)
        {
            Build build = buildRepository.GetBuild(buildId);
            var packageName = project.ProcedureName;
            
            var package = build.GetPackage(packageName);
            if (package == null)
            {
                messenger.Publish(string.Format("Could not find any deploy package named {0}", packageName));
                return false;
            }
            
            var assembly = Assembly.LoadFrom(string.Format("{0}\\{1}.dll", package.Path, packageName));
            if (assembly == null)
            {
                messenger.Publish(string.Format("Could not find any deploy assembly in package named {0}", packageName));
                return false;
            }

            var procedures = assembly.GetTypes().Where(x => x.IsAssignableFrom(typeof (IProcedure)) && x.Name == project.ProcedureName).ToList();
            if (!procedures.Any())
            {
                messenger.Publish(string.Format("Could not find any procedure in deploy package named {0}", packageName));
                return false;
            }
            
            if (procedures.Count > 1)
            {
                messenger.Publish(string.Format("Found more than one matching procedure in deploy package named {0}", packageName));
                return false;
            }

            var procedureType = procedures.Single();

            var procedure = CreateProcedure(procedureType);
            if (procedure == null)
            {
                messenger.Publish(string.Format("Procedure found in deploy package named {0}, does not have parameterless constructor or a constructor taking an IMessenger", packageName));
                return false;
            }

            return procedure.Perform(build, project);
        }

        IProcedure CreateProcedure(Type procedureType)
        {
            IProcedure procedure;
            var constructor = procedureType.GetConstructor(new[] { typeof (IMessenger) });
            if (constructor != null)
            {
                procedure = (IProcedure) constructor.Invoke(new object[] { messenger });
            }
            else
            {
                constructor = procedureType.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    return null;
                }

                procedure = (IProcedure) constructor.Invoke(new object[0]);
            }
            return procedure;
        }
    }
}