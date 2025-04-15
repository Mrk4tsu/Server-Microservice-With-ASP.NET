namespace FN.Application.Catalog.Product.Notifications
{
    public interface ITypedHubClient
    {
        Task SendMessage(Message message);
    }
}
