using System;
using System.IO;
using System.Reflection;
using System.Threading;
using Deploy.King.Projects;
using Deploy.Pawn.Api;
using Deploy.Procedures;
using Deploy.Procedures.Builds;
using System.Linq;
using Deploy.Procedures.Messaging;
using Deploy.Utilities;

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
            messenger.Publish(string.Format("Executing procedure for build {0}", buildId));

            Build build = buildRepository.GetBuild(buildId);
            var packageName = project.DeployPackageName;
            
            var package = build.GetPackage(packageName);
            if (package == null)
            {
                messenger.Publish(string.Format("Error: Could not find any deploy package named {0}", packageName));
                return false;
            }

            var filesInPackage = Zip.Extract(File.Open(package.Path, FileMode.Open));
            var assemblyFileName = project.DeployPackageName + ".dll";
            var assemblyFile = filesInPackage.SingleOrDefault(x => x.Filename == assemblyFileName);
            if (assemblyFile == null)
            {
                messenger.Publish(string.Format("Error: Could not find assembly file named {0} any deploy assembly in package named {1}", assemblyFileName, packageName));
                return false;
            }

            var assembly = Assembly.Load(assemblyFile.Data);
            if (assembly == null)
            {
                messenger.Publish(string.Format("Error: Could not find any deploy assembly in package named {0}", packageName));
                return false;
            }

            var procedures = assembly.GetTypes().Where(x => typeof(IProcedure).IsAssignableFrom(x) && (x.Name == project.ProcedureName || x.FullName == project.ProcedureName)).ToList();
            if (!procedures.Any())
            {
                messenger.Publish(string.Format("Error: Could not find any procedure named {0} in deploy package named {1}", project.ProcedureName, packageName));
                return false;
            }
            
            if (procedures.Count > 1)
            {
                messenger.Publish(string.Format("Error: Found more than one matching procedure in deploy package named {0}", packageName));
                return false;
            }

            var procedureType = procedures.Single();

            var procedure = CreateProcedure(procedureType);
            if (procedure == null)
            {
                messenger.Publish(string.Format("Error: Procedure found in deploy package named {0}, does not have parameterless constructor or a constructor taking an IMessenger", packageName));
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