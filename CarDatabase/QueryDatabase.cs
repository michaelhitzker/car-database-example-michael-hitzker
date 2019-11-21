using CarDatabase.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CarDatabase
{
    public static partial class Program
    {
        static async Task QueryCarOwnerships(CarDataContext context)
        {
            // Note Include method to query books and genre with a single query.
            var carModels = (await context
                 .CarModels
                 .Include(m => m.Owners)
                 .Include(m => m.CarMake)
                 .ToListAsync());
            carModels.Sort((a, b) => a.Model.CompareTo(b.Model));

            Console.WriteLine("Ownerships: ");
            foreach (CarModel carModel in carModels)
            {
                Console.WriteLine(carModel.CarMake.Make + " " + carModel.Model + ":\t" + carModel.Owners.Count);
            }
        }

        static async Task QueryNotAssignedOwnerships(CarDataContext context)
        {
            var carModels = (await context
                 .CarModels
                 .Include(m => m.Owners)
                 .Include(m => m.CarMake)
                 .Where(m => m.Owners.Count() <= 0)
                 .ToListAsync());
            carModels.Sort((a, b) => a.Model.CompareTo(b.Model));

            Console.WriteLine("Unassigned Ownerships: ");
            foreach (CarModel carModel in carModels)
            {
                Console.WriteLine(carModel.CarMake.Make + " " + carModel.Model);
            }
        }
    }
}
