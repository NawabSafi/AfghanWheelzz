using AfghanWheelzz.Repository;
using AfghanWheelzz.ViewModels;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace AfghanWheelzz.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository;

        public CarService(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public Task<CarViewModel> GetCarByIdAsync(int id)
        {
            return _carRepository.GetCarByIdAsync(id);
        }


        public Task<List<CarViewModel>> GetAllCarsAsync()
        {
            return _carRepository.GetAllCarsAsync();
        }


        public Task<List<CarViewModel>> GetCarsByUserIdAsync(string userId)
        {
            return _carRepository.GetCarsByUserIdAsync(userId);
        }
        public Task<List<CarViewModel>> GetAllCarsAsync(int pageNumber, int pageSize)
        {
            return _carRepository.GetAllCarsAsync(pageNumber, pageSize);
        }


        public Task AddCarAsync(CarViewModel car, string userId)
        {
            return _carRepository.AddCarAsync(car, userId);
        }

        public Task UpdateCarAsync(CarViewModel car)
        {
            return _carRepository.UpdateCarAsync(car);
        }

        public Task DeleteCarAsync(int id)
        {
            return _carRepository.DeleteCarAsync(id);
        }
    }
}
