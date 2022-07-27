using Newtonsoft.Json;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using System;
using System.Diagnostics;
using System.IO;

namespace ServerAgent.Actor
{
    public class ProcessActor : ActorRefBase
    {
        private readonly string _serverName;
        private readonly string _binaryPath;

        private Process _process;
        private IActorRef _timeCheckActor;

        public ProcessActor(ServerProcess serverProcess, MonitoringConfig config)
        {
            _serverName = serverProcess.ServerName;
            _binaryPath = serverProcess.BinaryPath;

            _process = null;
            _timeCheckActor = TimeCheckActorFactory.Create(config);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AliveCheckMessage _:
                    OnAliveCheckMessage();
                    break;
                case ProcessKillMessage _message:
                    OnKillProcessMessage(_message);
                    break;
                case WorkerThreadMessage _message:
                    _timeCheckActor.Tell(_message, Self);
                    break;
                case ProcessStoppedMessage _message:
                    Context.Parent?.Tell(_message, Self);
                    break;
                default:
                    // invalid message
                    break;
            }
        }

        private void OnAliveCheckMessage()
        {
            if (_process == null)
            {
                StartProcess();
                return;
            }

            if (_process.HasExited)
            {
                StartProcess();
                return;
            }

            _timeCheckActor.Tell(new AliveCheckMessage(), Self);
        }

        private void OnKillProcessMessage(ProcessKillMessage message)
        {
            if (_process == null || _process.HasExited)
            {
                Sender.Tell(new ProcessKillResponseMessage()
                {
                    ServerName = null
                }, Self);
                return;
            }

            switch (message.Command)
            {
                case "kill":
                    KillProcess();
                    Sender.Tell(new ProcessKillResponseMessage()
                    {
                        ServerName = _serverName,
                        ExitCode = _process.ExitCode,
                        Close = true
                    }, Self);
                    break;
                case "close":
                    var close = CloseProcess();
                    Sender.Tell(new ProcessKillResponseMessage()
                    {
                        ServerName = _serverName,
                        ExitCode = _process.ExitCode,
                        Close = close
                    }, Self);
                    break;
                default:
                    Sender.Tell(new ProcessKillResponseMessage()
                    {
                        ServerName = null
                    }, Self);
                    break;
            }
        }

        private bool StartProcess()
        {
            _process = new Process();
            _process.StartInfo = new ProcessStartInfo()
            {
                FileName = _binaryPath,
                Arguments = $"{_serverName}",
                WorkingDirectory = Path.GetDirectoryName(_binaryPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
            };
            _process.OutputDataReceived += OutputDataReceived;

            bool started = _process.Start();
            _process.BeginOutputReadLine();

            // logger.Info($"start process. {_serverName}, started: {started}, path: {_binaryPath}");
            return started;
        }

        private void KillProcess()
        {
            try
            {
                _process.Kill();
                _process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // logger.Info($"success kill process. {_serverName}, ExitCode: {process.ExitCode}");
        }

        private bool CloseProcess()
        {
            bool ret = _process.CloseMainWindow();
            if (!ret)
            {
                _process.WaitForExit();
                // logger.Info($"success close process. {_serverName}, ExitCode: {process.ExitCode}");
            }
            else
            {
                // logger.Info($"failed close process. {_serverName}, HasExited: {process.HasExited}");
            }

            return ret;
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null || e?.Data.Length <= 0)
                return;

            try
            {
                var message = JsonConvert.DeserializeObject<WorkerThreadMessage>(e.Data);
                Self.Tell(message);
            }
            catch (JsonSerializationException)
            {
                // logger.Error($"json serialize exception. {e.Data}");
            }
            catch (JsonReaderException)
            {
                // logger.Error($"json read exception. {e.Data}");
            }
            catch (Exception)
            {
                // log
            }
        }
    }
}
