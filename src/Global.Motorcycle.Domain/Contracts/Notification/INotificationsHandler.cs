using Global.Motorcycle.Domain.Models.Notifications;

namespace Global.Motorcycle.Domain.Contracts.Notifications
{
    public interface INotificationsHandler
    {
        ISet<Notification> Erros { get; }
        ISet<Notification> Warnings { get; }
        INotificationsHandler AddNotification(string details, ENotificationType type, object data = null);
        INotificationsHandler AddNotification(IEnumerable<string> listDetails, ENotificationType type, object data = null);
        bool HasErrors();
        bool HasWarnings();
        ENotificationType GetNotificationType();
        T ReturnDefault<T>();
    }
}
