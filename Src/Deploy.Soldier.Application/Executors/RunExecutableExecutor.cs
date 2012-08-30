using System.Diagnostics;
using Deploy.Soldier.Application.Infrastructure;
using Deploy.Tasks.Executables;

namespace Deploy.Soldier.Application.Executors
{
    public class RunExecutableExecutor : TaskExecutor<RunExecutable, RunExecutable.Result>
    {
        protected override RunExecutable.Result Execute(RunExecutable task)
        {
            var process = new Process();
            process.StartInfo.Arguments = task.Arguments;
            process.StartInfo.FileName = task.ExecutablePath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.ErrorDialog = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;

            process.Start();
            var message = process.StandardOutput.ReadToEnd();
            var errorMessage = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return new RunExecutable.Result
            {
                Success = process.ExitCode == 0,
                Message = message,
                ErrorMessage = errorMessage
            };
        }
    }
}