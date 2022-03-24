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
        private readonly DetectTimeModel detectTime;

        private Process process;
        private ProcessInfoModel processInfo;

        public ServerProcess(ServerInfoModel serverInfo, DetectTimeModel detectTime)
        {
            logger = LogManager.GetLogger(typeof(ServerProcess));

            FilePath = serverInfo.BinaryPath;
            ServerName = serverInfo.ServerName;
            this.detectTime = detectTime;

            process = null;
            processInfo = new ProcessInfoModel()
            {
                ProcessingTime = 0,
                ThreadId = 0,
                LastReceiveTime = DateTime.Now
            };
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
                lock (processInfo)
                {
                    return new ProcessInfoModel()
                    {
                        ProcessingTime = processInfo.ProcessingTime,
                        ThreadId = processInfo.ThreadId,
                        LastReceiveTime = processInfo.LastReceiveTime
                    };
                }
            }
        }

        private bool IsDead
        {
            get
            {
                return process?.HasExited ?? true;
            }
        }

        private bool IsStopped
        {
            get
            {
                lock (processInfo)
                {
                    if (processInfo.ProcessingTime > detectTime.StoppedMin)
                        return true;

                    DateTime lastRecvTime = processInfo.LastReceiveTime;
                    var stoppedReciveTime = lastRecvTime.AddMinutes(detectTime.StoppedMin);
                    if (stoppedReciveTime <= DateTime.Now)
                        return true;

                    var stoppedProcessingTime = lastRecvTime.AddMinutes(detectTime.StoppedMin - processInfo.ProcessingTime);
                    if (stoppedProcessingTime <= DateTime.Now)
                        return true;

                    return false;
                }
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

                processInfo.ProcessingTime = 0;
                processInfo.ThreadId = 0;
                processInfo.LastReceiveTime = DateTime.Now;

                bool started = process.Start();
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

        public void Close()
        {
            process.Kill();
            logger.Info($"on kill process. {ServerName}");
        }

        private void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e?.Data == null || e?.Data.Length <= 0)
                return;

            try
            {
                var model = JsonConvert.DeserializeObject<ProcessInfoModel>(e.Data);
                lock (processInfo)
                {
                    processInfo.ProcessingTime = model.ProcessingTime / 60;  // second
                    processInfo.ThreadId = model.ThreadId;
                    processInfo.LastReceiveTime = DateTime.Now;
                }
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
