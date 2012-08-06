using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Deploy.Commander.Application.Projects;
using Deploy.Procedures;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;
using Deploy.Utilities;

namespace Deploy.Commander.Application.Assemblies
{
    public class ProcedureLoader
    {
        readonly IMessenger messenger;

        public ProcedureLoader(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public IProcedure GetProcedure(Project project, Build build)
        {
            var assembly = GetAssembly(project, build);
            var packageName = project.DeployPackageName;

            var procedures = assembly.GetTypes().Where(x => typeof(IProcedure).IsAssignableFrom(x) && (x.Name == project.ProcedureName || x.FullName == project.ProcedureName)).ToList();
            if (!procedures.Any())
            {
                throw new CouldNotLoadProcedureException(string.Format("Error: Could not find any procedure named {0} in deploy package named {1}", project.ProcedureName, packageName));
            }

            if (procedures.Count > 1)
            {
                throw new CouldNotLoadProcedureException(string.Format("Error: Found more than one matching procedure in deploy package named {0}", packageName));
            }

            var procedureType = procedures.Single();

            var procedure = CreateProcedure(procedureType);
            if (procedure == null)
            {
                throw new CouldNotLoadProcedureException(string.Format("Error: Procedure found in deploy package named {0}, does not have parameterless constructor or a constructor taking an IMessenger", packageName));
            }

            return procedure;
        }

        public Assembly GetAssembly(Project project, Build build)
        {
            var packageName = project.DeployPackageName;

            var package = build.GetPackage(packageName);
            if (package == null)
            {
                throw new CouldNotLoadAssemblyException(string.Format("Could not find any deploy package named {0}", packageName));
            }

            var filesInPackage = Zip.Extract(File.Open(package.Path, FileMode.Open));
            var assemblyFileName = project.DeployPackageName + ".dll";
            var assemblyFile = filesInPackage.SingleOrDefault(x => x.Filename == assemblyFileName);
            if (assemblyFile == null)
            {
                throw new CouldNotLoadAssemblyException(string.Format("Could not find assemblyfile named {0} in package named {1}", assemblyFileName, packageName));
            }

            var assembly = Assembly.Load(assemblyFile.Data);
            if (assembly == null)
            {
                throw new CouldNotLoadAssemblyException(packageName);
            }

            return assembly;
        }

        public IProcedure CreateProcedure(Type procedureType)
        {
            IProcedure procedure;
            var constructor = procedureType.GetConstructor(new[] { typeof(IMessenger) });
            if (constructor != null)
            {
                procedure = (IProcedure)constructor.Invoke(new object[] { messenger });
            }
            else
            {
                constructor = procedureType.GetConstructor(new Type[0]);
                if (constructor == null)
                {
                    return null;
                }

                procedure = (IProcedure)constructor.Invoke(new object[0]);
            }
            return procedure;
        }

        public class CouldNotLoadAssemblyException : Exception
        {
            public CouldNotLoadAssemblyException(string message)
                : base(message)
            {
            }
        }

        public class CouldNotLoadProcedureException : Exception
        {
            public CouldNotLoadProcedureException(string message)
                : base(message)
            {
            }
        }
    }
}