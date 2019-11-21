using CarDatabase.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CarDatabase
{
    public partial class Program
    {
        private static Random Random = new Random();

        static async Task CleanDatabaseAsync(CarDataContext context)
        {
            // Note that we are using a DB transaction here. Either all records are
            // inserted or none of them (A in ACID).
            using var transaction = context.Database.BeginTransaction();

            // Note that we are using a "Raw" SQL statement here. With that, we can use
            // all features of the underlying database. We are not limited to what EFCore
            // can do.
            await context.Database.ExecuteSqlRawAsync("DELETE FROM CarMakes");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM CarModels");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Ownerships");
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Persons");

            await transaction.CommitAsync();
        }

        private static async Task FillCarMakeAsync(CarDataContext context)
        {
            var makeData = await File.ReadAllTextAsync("Data/mockCarMakes.json");
            var makeStrings = JsonSerializer.Deserialize<IEnumerable<MakeHolder>>(makeData);
            List<CarMake> makes = new List<CarMake>();
            foreach (var makeName in makeStrings)
            {
                var make = new CarMake { Make = makeName.Make };
                makes.Add(make);
            }

            using var transaction = context.Database.BeginTransaction();

            // Note how we combine transaction with exception handling
            try
            {
                // Note that we add all genre data rows BEFORE calling SaveChanges.
                foreach (var m in makes)
                {
                    context.CarMakes.Add(m);
                }

                await context.SaveChangesAsync();

                // Commit transaction if all commands succeed, transaction will auto-rollback
                // when disposed if either commands fails
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Something bad happened: {ex.Message}");

                // Note re-throw of exception
                throw;
            }
        }

        private static async Task FillCarModelsAsync(CarDataContext context)
        {
            // Demonstrate tracking queries here. Set a breakpoint up in
            // FillGenreAsync when genre rows are added. Afterwards, show that
            // the query returns THE SAME objects because of identical primary keys.
            var make = await context.CarMakes.ToArrayAsync();

            var modelStrings = JsonSerializer.Deserialize<IEnumerable<ModelHolder>>(
                await File.ReadAllTextAsync("Data/mockCarModels.json"));

            List<CarModel> models = new List<CarModel>();
            foreach (var modelName in modelStrings)
            {
                var model = new CarModel { Model = modelName.Model };
                models.Add(model);
            }

            using var transaction = context.Database.BeginTransaction();

            var rand = new Random();
            foreach (var model in models)
            {
                var dbModel = new CarModel
                {
                    CarMakeId = make[rand.Next(make.Length)].CarMakeId,
                    Model = model.Model
                };
                context.CarModels.Add(dbModel);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private static async Task FillPersonsAsync(CarDataContext context)
        {
            // Note that we are jus reading primary keys of books, not entire 
            // book records. Tip: Always read only those columns that you REALLY need.
            var modelIds = await context.CarModels.Select(m => m.CarModelId).ToArrayAsync();

            var personNames = JsonSerializer.Deserialize<IEnumerable<FirstnameLastname>>(
                await File.ReadAllTextAsync("Data/mockPeople.json"));

            List<Person> persons = new List<Person>();
            foreach (var personName in personNames)
            {
                var person = new Person { Firstname = personName.Firstname, Lastname = personName.Lastname };
                persons.Add(person);
            }


            using var transaction = context.Database.BeginTransaction();

            var rand = new Random();
            foreach (var person in persons)
            {
                var dbPerson = new Person
                {
                    Firstname = person.Firstname,
                    Lastname = person.Lastname
                };

                // Randomly assign each author one book.
                // Note that we can use the dbAuthor, although we have not yet written
                // it to the database. Also note that we are using the book ID as a
                // foreign key.
                var dbOwnership = new Ownership
                {
                    Person = dbPerson,
                    CarModelId = modelIds[rand.Next(modelIds.Length)],
                    VehicleIdentificationNumber = RandomString()
                };

                // Note that we do NOT need to add dbAuthor. It is referenced by
                // dbBookAuthor, that is enough.
                context.Ownerships.Add(dbOwnership);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        private static string RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}
