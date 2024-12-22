using PetSoLive.Core.Entities;
using PetSoLive.Core.Interfaces;

public class AdoptionService : IAdoptionService
{
    private readonly IAdoptionRepository _adoptionRepository;

    public AdoptionService(IAdoptionRepository adoptionRepository)
    {
        _adoptionRepository = adoptionRepository ?? throw new ArgumentNullException(nameof(adoptionRepository));
    }

    public async Task CreateAdoptionAsync(Adoption adoption)
    {
        if (adoption == null)
            throw new ArgumentNullException(nameof(adoption));

        var isAlreadyAdopted = await _adoptionRepository.IsPetAlreadyAdoptedAsync(adoption.PetId);
        if (isAlreadyAdopted)
            throw new InvalidOperationException("This pet has already been adopted.");

        await _adoptionRepository.AddAsync(adoption);
    }

    public async Task<Adoption?> GetAdoptionByPetIdAsync(int petId)
    {
        return await _adoptionRepository.GetAdoptionByPetIdAsync(petId);
    }

    public async Task CreateAdoptionRequestAsync(AdoptionRequest adoptionRequest)
    {
        if (adoptionRequest == null)
            throw new ArgumentNullException(nameof(adoptionRequest));

        var existingAdoption = await _adoptionRepository.GetAdoptionByPetIdAsync(adoptionRequest.PetId);
        if (existingAdoption != null)
            throw new InvalidOperationException("This pet has already been adopted.");

        await _adoptionRepository.AddAsync(adoptionRequest);
    }

    public async Task<bool> HasUserAlreadyRequestedAdoptionAsync(int userId, int petId)
    {
        return await _adoptionRepository.HasUserAlreadyRequestedAdoptionAsync(userId, petId);
    }

    public async Task<AdoptionRequest?> GetAdoptionRequestByUserAndPetAsync(int userId, int petId)
    {
        return await _adoptionRepository.GetAdoptionRequestByUserAndPetAsync(userId, petId);
    }
}