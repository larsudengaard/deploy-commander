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
        readonly AsyncSubject<Result> processSync;

        public RunExecutableExecutor()
        {
            processSync = new AsyncSubject<Result>();
        }

        protected override Result Execute(RunExecutable task)
        {
            var process = new Process();
            process.StartInfo.Arguments = task.Arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = task.ExecutablePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            
            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) =>
            {
                processSync.OnNext(new Result
                {
                    Success = process.ExitCode == 0, 
                    Message = process.StandardOutput.ReadToEnd()
                });
                processSync.OnCompleted();
            };
            process.Start();

            return processSync.First();
        }
    }
}