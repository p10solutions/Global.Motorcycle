using Global.Motorcycle.Domain.Entities;

namespace Global.Motorcycle.Domain.Models.Events.Motorcycles
{
    public record UpdatedMotorcycleEvent
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public int Year { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public EMotorcycleStatus Status { get; set; }
    }
}
