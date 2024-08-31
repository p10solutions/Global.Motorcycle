using Global.Motorcycle.Domain.Contracts.Date;

namespace Global.Motorcycle.Domain.Entities
{
    public class Location
    {
        public Guid Id { get; set; }
        public Guid DeliverymanId { get; set; }
        public Guid PlanId { get; set; }
        public Guid MotorcycleId { get; set; }
        public double? Amount { get; private set; }
        public DateTime InitialDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public DateTime? ReturnDate { get; private set; }
        public Plan? Plan { get; private set; }
        public MotorcycleEntity? Motorcycle { get; set; }
        public bool? Paid { get; set; }
        public double? Fee { get; private set; }
        public int? DaysUse { get; private set; }
        public ELocationStatus Status { get; private set; }

        public Location(Guid deliverymanId, Guid planId, Guid motorcycleId)
        {
            Id = Guid.NewGuid();
            DeliverymanId = deliverymanId;
            PlanId = planId;
            MotorcycleId = motorcycleId;
            Status = ELocationStatus.Active;
        }

        public Location SetInitialDate(ISystemDate systemDate)
        {
            InitialDate = systemDate.Now.AddDays(1);

            return this;
        }

        public Location SetPlan(Plan plan)
        {
            Plan = plan;
            EndDate = InitialDate.AddDays(plan.Days);

            return this;
        }

        public Location GiveBack(DateTime returnDate, Plan plan)
        {
            ReturnDate = returnDate;

            CalculateFee(returnDate, plan);
            CalculateDaysUse(returnDate);

            Amount = (DaysUse * plan.Daily) + Fee;

            Status = ELocationStatus.Returned;

            return this;
        }

        void CalculateDaysUse(DateTime returnDate)
        {
            var differenceDate = returnDate.Date - EndDate.Date;

            if (differenceDate.Days < 0)
            {
                DaysUse = (returnDate.Date - InitialDate.Date).Days;

                return;
            }

            DaysUse = (EndDate.Date - InitialDate.Date).Days;
        }

        void CalculateFee(DateTime returnDate, Plan plan)
        {
            if (EndDate.Date == returnDate.Date)
            {
                Fee = 0;
                return;
            }

            var differenceDate = returnDate.Date - EndDate.Date;

            if (differenceDate.Days < 0)
            {
                var days = differenceDate.Days * -1;
                var percent = plan.FeeBefore / 100;
                Fee = (days * plan.Daily) * percent;
            }
            else
            {
                var days = differenceDate.Days;
                Fee = (days * plan.FeeAfter);
            }
        }
    }
}
