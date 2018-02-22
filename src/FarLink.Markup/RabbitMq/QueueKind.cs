namespace FarLink.Markup.RabbitMq
{
    public enum QueueKind
    {
        PerApplication,
        Separate,
        PerConsumer,
        PerSession
    }
}