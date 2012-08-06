using System;
using Deploy.King.Assemblies;
using Deploy.King.Projects;
using Deploy.Procedures;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;

namespace Deploy.King.Executor
{
    public class ProcedureExecutor
    {
        readonly ProcedureLoader procedureLoader;
        readonly IBuildRepository buildRepository;
        readonly IMessenger messenger;

        public ProcedureExecutor(ProcedureLoader procedureLoader, IBuildRepository buildRepository, IMessenger messenger)
        {
            this.procedureLoader = procedureLoader;
            this.buildRepository = buildRepository;
            this.messenger = messenger;
        }

        public bool Execute(Project project, string buildId)
        {
            messenger.Publish(string.Format("Executing procedure for build {0}", buildId));
            Build build = buildRepository.GetBuild(buildId);

            IProcedure procedure;
            try
            {
                procedure = procedureLoader.GetProcedure(project, build);
            }
            catch(Exception e)
            {
                messenger.Publish("Error: " + e.Message);
                return false;
            }

            return procedure.Perform(build, project);
        }
    }
}