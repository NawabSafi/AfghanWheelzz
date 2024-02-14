using AfghanWheelzz.Data;
using AfghanWheelzz.Models;
using AfghanWheelzz.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AfghanWheelzz.Repository
{
    public class CarRepository : ICarRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CarRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CarViewModel>> GetCarsByUserIdAsync(string userId)
        {
            var carEntities = await _dbContext.Cars
                .Include(c => c.User)
                .Where(c => c.UserId == userId)
                .ToListAsync();
            return carEntities.Select(MapCarEntityToViewModel).ToList();
        }
        public async Task<CarViewModel> GetCarByIdAsync(int id)
        {
            var carEntity = await _dbContext.Cars
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
            return MapCarEntityToViewModel(carEntity);
        }

        public async Task<List<CarViewModel>> GetAllCarsAsync()
        {
            var carEntities = await _dbContext.Cars
                .Include(c => c.User)
                .ToListAsync();
            return carEntities.Select(MapCarEntityToViewModel).ToList();
        }

        public async Task AddCarAsync(CarViewModel car, string userId)
        {
            var carEntity = MapViewModelToCarEntity(car);
            carEntity.UserId = userId;
            _dbContext.Cars.Add(carEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCarAsync(CarViewModel car)
        {
            var carEntity = MapViewModelToCarEntity(car);
            _dbContext.Entry(carEntity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var carEntity = await _dbContext.Cars.FindAsync(id);
            _dbContext.Cars.Remove(carEntity);
            await _dbContext.SaveChangesAsync();
        }

        private CarViewModel MapCarEntityToViewModel(Car carEntity)
        {
            if (carEntity == null)
            {
                // Handle case where car entity is null (optional)
                return null;
            }

            // Map properties from car entity to view model
            var viewModel = new CarViewModel
            {
                Id = carEntity.Id,
                Model = carEntity.Model,
                Year = carEntity.Year,
                Price = carEntity.Price,
                Mileage = carEntity.Mileage,
                Make = carEntity.Make,
                Description = carEntity.description,
                ImagePath = carEntity.ImagePath,
                RegistrationId = carEntity.RegistrationId,
                UserId = carEntity.UserId,
                CategoryId = carEntity.CategoryId,
                LocationId = carEntity.LocationId,
                DatePublished = carEntity.DatePublished,

                // If you have more properties to map, add them here
            };

            return viewModel;
        }


        private Car MapViewModelToCarEntity(CarViewModel carViewModel)
        {
            return new Car
            {
                Id = carViewModel.Id,

                Model = carViewModel.Model,
                Year = carViewModel.Year,
                Price = carViewModel.Price,
                Mileage = carViewModel.Mileage,
                Make = carViewModel.Make,
                description = carViewModel.Description,
                ImagePath = carViewModel.ImagePath,
                LocationId = carViewModel.LocationId, // Assuming LocationId is an int property in Car
                CategoryId = carViewModel.CategoryId, // Assuming CategoryId is an int property in Car
                RegistrationId = carViewModel.RegistrationId, //ou might need to map other properties like Registration and User here
                DatePublished = DateOnly.FromDateTime(DateTime.Now),

            };
        }

    }
}
