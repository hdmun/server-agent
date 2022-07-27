using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using ServerAgent.Data.Provider;
using System;
using System.Collections.Generic;
using System.Net;

namespace ServerAgent.Actor
{
    public class MonitoringActor : ActorRefBase
    {
        private readonly IDataProvider _dataProvider;
        private Dictionary<string, IActorRef> _targetActors;

        private MonitoringConfig _monitoringConfig;
        private bool _running;

        public MonitoringActor(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _targetActors = new Dictionary<string, IActorRef>();

            _monitoringConfig = null;
            _running = false;
        }

        protected override void OnStart()
        {
            var hostName = Dns.GetHostName();
            _monitoringConfig = _dataProvider.FindMonitoringConfig(hostName);

            var processes = _dataProvider.FindProcesses(hostName);
            foreach (var process in processes)
            {
                string actorName = $"/process/{process.ServerName}";
                _targetActors.Add(process.ServerName, Context.ActorOf(
                    new ProcessActor(process, _monitoringConfig), actorName, this));
            }

            StartSingleTimer("AliveMessage", new AliveCheckMessage(), 1000);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AliveCheckMessage _message:
                    OnAliveCheckMessage(_message);
                    break;
                case MonitoringMessage _message:
                    _running = _message.On;
                    Sender.Tell(_message, Self);
                    break;
                case ServerKillRequestMessage _message:
                    OnServerKillRequestMessage(_message);
                    break;
                default:
                    // invalid message
                    string exceptMessage = $"Received message of type [{message.GetType()}] - Invalid message in MonitoringActor";
                    Console.WriteLine(exceptMessage);
                    throw new Exception(exceptMessage);
            }
        }

        private void OnAliveCheckMessage(AliveCheckMessage message)
        {
            if (!_running)
            {
                return;
            }

            foreach (var actor in _targetActors.Values)
            {
                actor.Tell(message, Self);
            }
        }

        private void OnServerKillRequestMessage(ServerKillRequestMessage message)
        {
            var askMessage = new ProcessKillMessage()
            {
                Command = message.KillCommand
            };

            if (message.ServerName != "")
            {
                var actor = FindTargetActor(message.ServerName);
                if (actor == null)
                {
                    Sender.Tell(new ServerKillResponseMessage()
                    {
                        Servers = null
                    }, Self);
                    return;
                }

                var askTask = actor.Ask<ProcessKillResponseMessage>(askMessage);
                Sender.Tell(new ServerKillResponseMessage()
                {
                    Servers = new ProcessKillResponseMessage[] { askTask.Result }
                }, Self);
            }
            else
            {
                var response = AskProcessActorAll<ProcessKillResponseMessage>(askMessage);
                Sender.Tell(new ServerKillResponseMessage()
                {
                    Servers = response
                }, Self);
            }
        }

        private T[] AskProcessActorAll<T>(object message)
        {
            var response = new List<T>();
            foreach (var actor in _targetActors.Values)
            {
                var askTask = actor.Ask<T>(message);
                response.Add(askTask.Result);
            }

            return response.ToArray();
        }

        private IActorRef FindTargetActor(string serverName)
        {
            if (!_targetActors.ContainsKey(serverName))
            {
                return null;
            }

            return _targetActors[serverName];
        }
    }
}
