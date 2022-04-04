using log4net;
using Newtonsoft.Json;
using ServerAgent.Monitoring.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace ServerAgent.Monitoring.Interactor
{
    public class ServerProcess
    {
        public readonly ILog logger;

        public readonly string FilePath;
        public readonly string ServerName;
        private readonly ITimeChecker timeChecker;

        private Process process;

        public ServerProcess(ServerInfoModel serverInfo, ITimeChecker timeChecker)
        {
            logger = LogManager.GetLogger(typeof(ServerProcess));

            FilePath = serverInfo.BinaryPath;
            ServerName = serverInfo.ServerName;
            this.timeChecker = timeChecker;

            process = null;
        }

        public void OnMonitoring()
        {
            if (IsDead)
            {
                Start();
                return;
            }

            if (IsStopped)
            {
                // check deadlock

                Close();
                return;
            }
        }

        public ProcessInfoModel ProcessInfo
        {
            get
            {
                lock (timeChecker)
                {
                    return new ProcessInfoModel()
                    {
                        ProcessingTime = timeChecker.ProcessingTime,
                        ThreadId = timeChecker.ThreadId,
                        LastReceiveTime = timeChecker.LastReceiveTime
                    };
                }
            }
        }

        public bool IsDead
        {
            get
            {
                return process?.HasExited ?? true;
            }
        }

        public int ExitCode
        {
            get
            {
                if (IsDead)
                    return process?.ExitCode ?? int.MaxValue;
                return int.MaxValue;
            }
        }

        private bool IsStopped
        {
            get
            {
                lock (timeChecker)
                    return timeChecker.IsStopped;
            }
        }

        private bool Start()
        {
            try
            {
                process = new Process();
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = FilePath,
                    Arguments = $"{ServerName}",
                    WorkingDirectory = Path.GetDirectoryName(FilePath),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };
                process.OutputDataReceived += OutputDataReceived;

                bool started = process.Start();
                timeChecker.Start();
                process.BeginOutputReadLine();

                logger.Info($"start process. {ServerName}, started: {started}");
                return started;
            }
            catch (Exception ex)
            {
                logger.Error("start exception", ex);
                return false;
            }
        }

        public void Kill()
        {
            process.Kill();
            logger.Info($"success kill process. {ServerName}, ExitCode: {process.ExitCode}");
        }

        public bool Close()
        {
            bool ret = process.CloseMainWindow();
            if (ret)
                logger.Info($"success close process. {ServerName}, ExitCode: {process.ExitCode}");
            else
                logger.Info($"failed close process. {ServerName}, HasExited: {process.HasExited}");
            return ret;
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null || e?.Data.Length <= 0)
                return;

            try
            {
                var model = JsonConvert.DeserializeObject<ProcessInfoModel>(e.Data);
                lock (timeChecker)
                    timeChecker.Update(model);
            }
            catch (JsonSerializationException)
            {
                logger.Error($"json serialize exception. {e.Data}");
            }
            catch (JsonReaderException)
            {
                logger.Error($"json read exception. {e.Data}");
            }
        }
    }
}
