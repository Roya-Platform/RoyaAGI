using System.Threading.Tasks;
using AgentIO.Models;

namespace AgentIO.Services
{
    /// <summary>
    /// Interface for the core kernel service that processes agent requests.
    /// This service acts as the primary interface between agents and the kernel,
    /// handling request routing and response formatting.
    /// </summary>
    public interface IKernelService
    {
        /// <summary>
        /// Sends a request to the kernel and returns the response.
        /// This method routes the request to the appropriate handler based on 
        /// the request category and type.
        /// </summary>
        /// <param name="request">The request to process</param>
        /// <returns>A response containing the operation result or error information</returns>
        Task<KernelResponse> SendRequestAsync(KernelRequest request);
    }
} 