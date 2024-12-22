using Microsoft.EntityFrameworkCore;
using PetSoLive.Core.Entities;
using PetSoLive.Core.Enums;
using PetSoLive.Data;

public class AdoptionRepository : IAdoptionRepository
{
    private readonly ApplicationDbContext _context;

    public AdoptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Adoption adoption)
    {
        await _context.Adoptions.AddAsync(adoption);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(AdoptionRequest adoptionRequest)
    {
        await _context.AdoptionRequests.AddAsync(adoptionRequest);
        await _context.SaveChangesAsync();
    }

    public async Task<Adoption?> GetAdoptionByPetIdAsync(int petId)
    {
        return await _context.Adoptions
            .FirstOrDefaultAsync(a => a.PetId == petId);
    }

    public async Task<AdoptionRequest?> GetAdoptionRequestByPetIdAsync(int petId)
    {
        return await _context.AdoptionRequests
            .FirstOrDefaultAsync(ar => ar.PetId == petId);
    }

    public async Task<AdoptionRequest?> GetAdoptionRequestByIdAsync(int id)
    {
        return await _context.AdoptionRequests
            .FirstOrDefaultAsync(ar => ar.Id == id);
    }

    public async Task UpdateAsync(AdoptionRequest adoptionRequest)
    {
        _context.AdoptionRequests.Update(adoptionRequest);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Adoption adoption)
    {
        _context.Adoptions.Update(adoption);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsPetAlreadyAdoptedAsync(int petId)
    {
        return await _context.Adoptions
            .AnyAsync(a => a.PetId == petId && a.Status == AdoptionStatus.Approved);
    }
    
    public async Task<bool> HasUserAlreadyRequestedAdoptionAsync(int userId, int petId)
    {
        return await _context.AdoptionRequests
            .AnyAsync(ar => ar.UserId == userId && ar.PetId == petId);
    }
    
    
    public async Task<AdoptionRequest?> GetAdoptionRequestByUserAndPetAsync(int userId, int petId)
    {
        return await _context.AdoptionRequests
            .FirstOrDefaultAsync(ar => ar.UserId == userId && ar.PetId == petId);
    }
}