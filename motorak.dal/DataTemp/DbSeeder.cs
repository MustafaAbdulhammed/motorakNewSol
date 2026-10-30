//using Microsoft.EntityFrameworkCore;
//using motorak.dal.Entites;
//using motorak.dal.Enums.CarEnums;
//using motorak.DAL.DataBase;
//using Motorak.DAL.Enums.CarEnums;

namespace motorak.dal.DataTemp
{
    public class DbSeeder
    {
        public static async Task SeedAsync(MotorakDbContext context)
        {
            context.Database.EnsureCreated();
            if (!await context.Cars.AnyAsync())
            {
                var carsToSeed = new List<Car>
                {
                    new Car("Porsche", "Turbo S", 2024, "White", 500, CarType.SportsCar, CarTransmission.Automatic, CarCondition.New, 175900m, 499, CarCategory.ForRent, "assets/img/popular1.png"),
                    new Car("Porsche", "Taycan", 2024, "Black", 400, CarType.Electric, CarTransmission.Automatic, CarCondition.New, 114900m, 399, CarCategory.ForRent, "assets/img/popular2.png"),
                    new Car("Porsche", "Turbo S Cross", 2024, "Blue", 300, CarType.SUV, CarTransmission.Automatic, CarCondition.New, 150900m, 459, CarCategory.ForRent, "assets/img/popular3.png"),
                    new Car("Porsche", "Boxster 718", 2024, "Red", 350, CarType.SportsCar, CarTransmission.Automatic, CarCondition.New, 125900m, 429, CarCategory.ForRent, "assets/img/popular4.png"),
                    new Car("Porsche", "Cayman", 2024, "Yellow", 320, CarType.SportsCar, CarTransmission.Automatic, CarCondition.New, 128900m, 439, CarCategory.ForRent, "assets/img/popular5.png"),

                    new Car("Tesla", "Model X", 2024, "White", 200, CarType.SUV, CarTransmission.Automatic, CarCondition.New, 98900m, null, CarCategory.ForSale, "assets/img/featured1.png"),
                    new Car("Tesla", "Model 3", 2024, "Red", 150, CarType.Sedan, CarTransmission.Automatic, CarCondition.New, 45900m, null, CarCategory.ForSale, "assets/img/featured2.png"),
                    new Car("Audi", "E-tron", 2024, "Gray", 250, CarType.SUV, CarTransmission.Automatic, CarCondition.New, 175900m, null, CarCategory.ForSale, "assets/img/featured3.png"),
                    new Car("Porsche", "Boxster 987", 2024, "Black", 100, CarType.SportsCar, CarTransmission.Automatic, CarCondition.New, 126900m, null, CarCategory.ForSale, "assets/img/featured4.png"),
                    new Car("Porsche", "Panamera", 2024, "Blue", 120, CarType.Sedan, CarTransmission.Automatic, CarCondition.New, 175900m, null, CarCategory.ForSale, "assets/img/featured5.png")
                };
                await context.Cars.AddRangeAsync(carsToSeed);
                await context.SaveChangesAsync();
            }
        }
    }
}
