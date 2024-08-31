using Global.Motorcycle.Domain.Contracts.Notifications;
using Global.Motorcycle.Domain.Models.Notifications;

namespace Global.Motorcycle.Infraestructure.Validation
{
    public class NotificationHandler : INotificationsHandler
    {
        readonly IList<Notification> _notifications = new List<Notification>();

        public ISet<Notification> Erros =>
            new HashSet<Notification>(_notifications.Where(x => x.NotificationType != ENotificationType.Warning));

        public ISet<Notification> Warnings =>
            new HashSet<Notification>(_notifications.Where(x => x.NotificationType == ENotificationType.Warning));

        public INotificationsHandler AddNotification(string details, ENotificationType type, object data = null)
        {
            _notifications.Add(new Notification
            {
                NotificationType = type,
                Data = data,
                Details = details
            });

            return this;
        }

        public INotificationsHandler AddNotification(IEnumerable<string> listDetails, ENotificationType type, object data = null)
        {
            foreach (var details in listDetails)
                AddNotification(details, type, data);

            return this;
        }

        public ENotificationType GetNotificationType()
            => _notifications.FirstOrDefault().NotificationType;

        public bool HasErrors()
            => _notifications.Any(x => x.NotificationType != ENotificationType.Warning);

        public bool HasWarnings()
            => _notifications.Any(x => x.NotificationType == ENotificationType.Warning);

        public T ReturnDefault<T>()
            => default;
    }
}
