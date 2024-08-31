namespace Global.Motorcycle.Domain.Entities
{
    public class Plan
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
        public double Daily { get; set; }
        public double? FeeBefore { get; set; }
        public double? FeeAfter { get; set; }

        public Plan(string name, int days, double daily)
        {
            Id = Guid.NewGuid();
            Name = name;
            Days = days;
            Daily = daily;
        }
    }
}
