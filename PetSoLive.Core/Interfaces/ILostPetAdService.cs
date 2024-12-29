using PetSoLive.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSoLive.Core.Interfaces
{
    public interface ILostPetAdService
    {
        // Yeni bir kayıp ilanı oluşturmak için metod
        Task CreateLostPetAdAsync(LostPetAd lostPetAd, string city, string district);

        // Tüm kayıp ilanlarını almak için metod
        Task<IEnumerable<LostPetAd>> GetAllLostPetAdsAsync();

        // ID'ye göre bir kayıp ilanını almak için metod
        Task<LostPetAd> GetLostPetAdByIdAsync(int id);

        // Kayıp ilanını güncellemek için metod
        Task UpdateLostPetAdAsync(LostPetAd lostPetAd);

        // Kayıp ilanını silmek için metod
        Task DeleteLostPetAdAsync(LostPetAd lostPetAd);
    }
}