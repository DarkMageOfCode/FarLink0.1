namespace FarLink.Declarations
{
    public abstract class Service<T> 
        where T : Service<T>, new()
    {
        protected Service(QualifiedIdentifier name)
        {
            Name = name;
        }

        public QualifiedIdentifier Name { get; }
    }

    public abstract class Event<TService> 
        where TService : Service<TService>, new()
    {
        
    }
}