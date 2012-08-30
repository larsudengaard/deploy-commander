using System.Diagnostics;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Executables;

namespace Deploy.Soldier.Application.Executors
{
    public class StopProcessExecutor : TaskExecutor<StopProcess, StopProcess.Result>
    {
        protected override StopProcess.Result Execute(StopProcess task)
        {
            var processes = Process.GetProcessesByName(task.Name);
            foreach (var process in processes)
            {
                if (process.HasExited == false)
                {
                    if (process.Responding)
                    {
                        process.CloseMainWindow();
                    }
                    else
                    {
                        process.Kill();
                    }
                }

                process.WaitForExit(10000);
            }

            return new StopProcess.Result();
        }
    }
}