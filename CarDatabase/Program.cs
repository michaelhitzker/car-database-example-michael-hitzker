using System.Threading.Tasks;

namespace CarDatabase
{
    public static partial class Program
    {
        static async Task Main()
        {
            using var context = new CarDataContext();
            /*await CleanDatabaseAsync(context);
            await FillCarMakeAsync(context);
            await FillCarModelsAsync(context);
            await FillPersonsAsync(context);*/

            await QueryCarOwnerships(context);
            await QueryNotAssignedOwnerships(context);
        }
    }
}
