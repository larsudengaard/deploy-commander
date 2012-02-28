using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using Deploy.Pawn.Api;
using System.Reactive.Linq;
using Deploy.Pawn.Api.Tasks;
using Deploy.Pawn.Infrastructure;

namespace Deploy.Pawn.Executors
{
    public class RunExecutableExecutor : TaskExecutor<RunExecutable, Result>
    {
        protected override Result Execute(RunExecutable task)
        {
            var process = new Process();
            process.StartInfo.Arguments = task.Arguments;
            process.StartInfo.FileName = task.ExecutablePath;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;

            process.Start();
            var message = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return new Result
            {
                Success = process.ExitCode == 0,
                Message = message
            };
        }
    }
}