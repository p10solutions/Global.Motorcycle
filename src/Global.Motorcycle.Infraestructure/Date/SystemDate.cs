using Global.Motorcycle.Domain.Contracts.Date;

namespace Global.Motorcycle.Infraestructure.Date
{
    public class SystemDate : ISystemDate
    {
        public DateTime Now => DateTime.Now;
    }
}
