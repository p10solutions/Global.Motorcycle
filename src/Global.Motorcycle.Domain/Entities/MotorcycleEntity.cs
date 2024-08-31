namespace Global.Motorcycle.Domain.Entities
{
    public class MotorcycleEntity
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public string Plate { get; set; }
        public int Year { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public EMotorcycleStatus Status { get; set; }

        public MotorcycleEntity()
        {
            Id = Guid.NewGuid();
        }

        public MotorcycleEntity(string model, EMotorcycleStatus status, string plate, int year)
        {
            Id = Guid.NewGuid();
            Model = model;
            CreateDate = DateTime.Now;
            Status = status;
            Plate = plate;
            Year = year;
        }
    }
}
