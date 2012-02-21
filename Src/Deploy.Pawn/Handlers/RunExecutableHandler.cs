using System.Diagnostics;
using System.Reactive.Subjects;
using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;
using System.Reactive.Linq;

namespace Deploy.Pawn.Handlers
{
    public class RunExecutableHandler : CommandHandler<RunExecutable>
    {
        readonly AsyncSubject<Result> processSync;

        public RunExecutableHandler()
        {
            processSync = new AsyncSubject<Result>();
        }

        protected override Result Handle(RunExecutable command)
        {
            var process = new Process();
            process.StartInfo.Arguments = command.Arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = command.ExecutablePath;
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