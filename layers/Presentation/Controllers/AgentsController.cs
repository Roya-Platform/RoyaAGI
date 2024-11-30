using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net;

namespace RoyaAi.Layers.Presentation.Controllers
{
    /// <summary>
    /// Controller implementing the Agent Protocol specification for Agents endpoints.
    /// </summary>
    [ApiController]
    [Route("/v1/agents")]
    public class AgentsController : ControllerBase
    {
        private readonly ILogger<AgentsController> _logger;

        public AgentsController(ILogger<AgentsController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// List all agents
        /// </summary>
        /// <returns>A list of all available agents</returns>
        [HttpGet]
        [ProducesResponseType(typeof(AgentsListResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListAgents()
        {
            try
            {
                // Implementation would retrieve agents from a service
                var response = new AgentsListResponse
                {
                    Agents = new List<AgentInfo>
                    {
                        // Sample data for demonstration purposes
                        new AgentInfo 
                        { 
                            Id = "agent-1", 
                            Name = "Default Agent",
                            Description = "Default agent for processing tasks",
                            IsDefault = true
                        }
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing agents");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse 
                { 
                    Error = new ErrorDetails 
                    { 
                        Message = "Failed to list agents", 
                        Type = "internal_error" 
                    } 
                });
            }
        }

        /// <summary>
        /// Get details about a specific agent
        /// </summary>
        /// <param name="agentId">The ID of the agent to retrieve</param>
        /// <returns>Details about the specified agent</returns>
        [HttpGet("{agentId}")]
        [ProducesResponseType(typeof(AgentInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAgent(string agentId)
        {
            try
            {
                // Implementation would retrieve the agent from a service
                if (agentId != "agent-1")
                {
                    return NotFound(new ErrorResponse 
                    { 
                        Error = new ErrorDetails 
                        { 
                            Message = $"Agent with ID {agentId} not found", 
                            Type = "not_found" 
                        } 
                    });
                }

                var agent = new AgentInfo
                {
                    Id = agentId,
                    Name = "Default Agent",
                    Description = "Default agent for processing tasks",
                    IsDefault = true
                };

                return Ok(agent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving agent {AgentId}", agentId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse 
                { 
                    Error = new ErrorDetails 
                    { 
                        Message = $"Failed to retrieve agent {agentId}", 
                        Type = "internal_error" 
                    } 
                });
            }
        }

        /// <summary>
        /// Create a new agent
        /// </summary>
        /// <param name="request">The agent creation request</param>
        /// <returns>Details of the created agent</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AgentInfo), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateAgent([FromBody] CreateAgentRequest request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.Name))
                {
                    return BadRequest(new ErrorResponse 
                    { 
                        Error = new ErrorDetails 
                        { 
                            Message = "Agent name is required", 
                            Type = "invalid_request" 
                        } 
                    });
                }

                // Implementation would create a new agent via a service
                var newAgent = new AgentInfo
                {
                    Id = $"agent-{Guid.NewGuid()}",
                    Name = request.Name,
                    Description = request.Description,
                    IsDefault = request.IsDefault ?? false
                };

                return CreatedAtAction(nameof(GetAgent), new { agentId = newAgent.Id }, newAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating agent");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse 
                { 
                    Error = new ErrorDetails 
                    { 
                        Message = "Failed to create agent", 
                        Type = "internal_error" 
                    } 
                });
            }
        }

        /// <summary>
        /// Update an existing agent
        /// </summary>
        /// <param name="agentId">The ID of the agent to update</param>
        /// <param name="request">The agent update request</param>
        /// <returns>Details of the updated agent</returns>
        [HttpPut("{agentId}")]
        [ProducesResponseType(typeof(AgentInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateAgent(string agentId, [FromBody] UpdateAgentRequest request)
        {
            try
            {
                // Validate agent exists
                if (agentId != "agent-1")
                {
                    return NotFound(new ErrorResponse 
                    { 
                        Error = new ErrorDetails 
                        { 
                            Message = $"Agent with ID {agentId} not found", 
                            Type = "not_found" 
                        } 
                    });
                }

                // Implementation would update the agent via a service
                var updatedAgent = new AgentInfo
                {
                    Id = agentId,
                    Name = request.Name ?? "Default Agent",
                    Description = request.Description ?? "Default agent for processing tasks",
                    IsDefault = request.IsDefault ?? false
                };

                return Ok(updatedAgent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating agent {AgentId}", agentId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse 
                { 
                    Error = new ErrorDetails 
                    { 
                        Message = $"Failed to update agent {agentId}", 
                        Type = "internal_error" 
                    } 
                });
            }
        }

        /// <summary>
        /// Delete an agent
        /// </summary>
        /// <param name="agentId">The ID of the agent to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{agentId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteAgent(string agentId)
        {
            try
            {
                // Validate agent exists
                if (agentId != "agent-1")
                {
                    return NotFound(new ErrorResponse 
                    { 
                        Error = new ErrorDetails 
                        { 
                            Message = $"Agent with ID {agentId} not found", 
                            Type = "not_found" 
                        } 
                    });
                }

                // Implementation would delete the agent via a service
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting agent {AgentId}", agentId);
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorResponse 
                { 
                    Error = new ErrorDetails 
                    { 
                        Message = $"Failed to delete agent {agentId}", 
                        Type = "internal_error" 
                    } 
                });
            }
        }
    }

    // DTO classes for the Agent Protocol

    public class AgentsListResponse
    {
        public List<AgentInfo> Agents { get; set; } = new List<AgentInfo>();
    }

    public class AgentInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
    }

    public class CreateAgentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class UpdateAgentRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsDefault { get; set; }
    }

    public class ErrorResponse
    {
        public ErrorDetails Error { get; set; }
    }

    public class ErrorDetails
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }
}

