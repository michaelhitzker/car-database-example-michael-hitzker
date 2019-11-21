using System.Collections.Generic;

namespace CarDatabase.Model
{
    class CarModel
    {
        public int CarModelId { get; set; }

        public string Model { get; set; }

        public int CarMakeId { get; set; }

        public CarMake CarMake { get; set; }

        public List<Ownership> Owners { get; set; }
    }
}
