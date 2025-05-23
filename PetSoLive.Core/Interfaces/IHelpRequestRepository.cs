using PetSoLive.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSoLive.Core.Interfaces
{
    public interface IHelpRequestRepository
    {
        // Create a new help request
        Task CreateHelpRequestAsync(HelpRequest helpRequest);

        // Get all help requests
        Task<List<HelpRequest>> GetHelpRequestsAsync();

        // Get a specific help request by ID
        Task<HelpRequest> GetHelpRequestByIdAsync(int id);

        // Get help requests for a specific user
        Task<List<HelpRequest>> GetHelpRequestsByUserAsync(int userId);
        
        Task UpdateHelpRequestAsync(HelpRequest helpRequest); // Yeni metot
        Task DeleteHelpRequestAsync(int id); // Yeni metot
    }
}