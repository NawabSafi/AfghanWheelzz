using AfghanWheelzz.ViewModels;

namespace AfghanWheelzz.Repository
{
    public interface ICarRepository
    {
        Task<CarViewModel> GetCarByIdAsync(int id);
        Task<List<CarViewModel>> GetAllCarsAsync();
        Task<List<CarViewModel>> GetCarsByUserIdAsync(string userId);
        Task AddCarAsync(CarViewModel car, string userId);
        Task UpdateCarAsync(CarViewModel car);
        Task DeleteCarAsync(int id);
    }
}
