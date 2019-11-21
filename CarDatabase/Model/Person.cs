using System.Collections.Generic;

namespace CarDatabase.Model
{
    class Person
    {
        public int PersonId { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public List<Ownership> Cars { get; set; }
    }
}
