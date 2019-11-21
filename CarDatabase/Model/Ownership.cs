namespace CarDatabase.Model
{
    class Ownership
    {
        public int OwnershipId { get; set; }

        public string VehicleIdentificationNumber { get; set; }

        public int PersonId { get; set; }

        public Person Person { get; set; }

        public int CarModelId { get; set; }

        public CarModel CarModel { get; set; }
    }
}
