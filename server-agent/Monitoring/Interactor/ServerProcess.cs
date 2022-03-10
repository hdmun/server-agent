using Newtonsoft.Json;
using server_agent.Monitoring.Model;
using System;
using System.Diagnostics;
using System.IO;

namespace server_agent.Monitoring.Interactor
{
    public class ServerProcess
    {
        public readonly string FilePath;
        public readonly string ServerName;
        private readonly DetectTimeModel detectTime;

        private Process process;
        private ProcessInfoModel processInfo;

        public ServerProcess(ServerInfoModel serverInfo, DetectTimeModel detectTime)
        {
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
                return started;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private void Close()
        {
            process.Kill();
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
                Console.WriteLine(e?.Data);
            }
            catch (JsonReaderException)
            {
                Console.WriteLine(e?.Data);  // 읽기 실패하면 무시
            }
        }
    }
}
