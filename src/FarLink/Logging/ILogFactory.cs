namespace FarLink.Logging
{
    public interface ILogFactory
    {
        ILog CreateLog(string category);
    }
}