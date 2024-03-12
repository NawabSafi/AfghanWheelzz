using AfghanWheelzz.ViewModels;

namespace AfghanWheelzz.Services
{
    public interface ICarService
    {
        Task<CarViewModel> GetCarByIdAsync(int id);
        Task<List<CarViewModel>> GetCarsByUserIdAsync(string userId);
        Task<List<CarViewModel>> GetAllCarsAsync(int pageNumber, int pageSize);
        Task AddCarAsync(CarViewModel car, string userId);
        Task UpdateCarAsync(CarViewModel car);
        Task DeleteCarAsync(int id);
    }
}
