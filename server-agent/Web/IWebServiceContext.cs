namespace ServerAgent.Web
{
    public interface IWebServiceContext
    {
        bool Monitoring { get; set; }

        void OnServerKill();
    }
}
