using System;
using System.Diagnostics;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Executables;

namespace Deploy.Soldier.Application.Executors
{
    public class StartProcessExecutor : TaskExecutor<StartProcess, StartProcess.Result>
    {
        protected override StartProcess.Result Execute(StartProcess task)
        {
            Process.Start(task.ProcessPath, task.Arguments);
            return new StartProcess.Result();
        }
    }
}