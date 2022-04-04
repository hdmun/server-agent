using ServerAgent.Web.Model;

namespace ServerAgent.Web
{
    public interface IWebServiceContext
    {
        bool Monitoring { get; set; }

        ServerKillResponseModel[] OnServerKill();

        ServerKillResponseModel[] OnServerClose();

        ServerKillResponseModel OnServerKill(string serverName);

        ServerKillResponseModel OnServerClose(string serverName);
    }
}
