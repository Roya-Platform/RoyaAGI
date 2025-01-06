using System;
using System.Threading.Tasks;
using AgentIO.Models;

namespace AgentIO.Services
{
    /// <summary>
    /// Implementation of the kernel service that processes agent requests.
    /// </summary>
    public class KernelService : IKernelService
    {
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;
        private readonly IAliasService _aliasService;

        public KernelService(
            IFileService fileService,
            IDirectoryService directoryService,
            IAliasService aliasService)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _directoryService = directoryService ?? throw new ArgumentNullException(nameof(directoryService));
            _aliasService = aliasService ?? throw new ArgumentNullException(nameof(aliasService));
        }

        public async Task<KernelResponse> SendRequestAsync(KernelRequest request)
        {
            if (request == null)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = "Request cannot be null"
                };
            }

            try
            {
                if (request.RequestCategory != "IO")
                {
                    return new KernelResponse
                    {
                        IsError = true,
                        ErrorMessage = $"Unsupported request category: {request.RequestCategory}"
                    };
                }

                switch (request.RequestType)
                {
                    case "Read":
                        return await HandleReadRequestAsync(request);
                    case "Update":
                        return await HandleUpdateRequestAsync(request);
                    case "Create":
                        return await HandleCreateRequestAsync(request);
                    case "Delete":
                        return await HandleDeleteRequestAsync(request);
                    case "Tree":
                        return await HandleTreeRequestAsync(request);
                    default:
                        return new KernelResponse
                        {
                            IsError = true,
                            ErrorMessage = $"Unsupported request type: {request.RequestType}"
                        };
                }
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<KernelResponse> HandleReadRequestAsync(KernelRequest request)
        {
            try
            {
                var resolvedPath = await _aliasService.ResolvePathAsync(request.FilePath);
                var content = await _fileService.ReadFileAsync(resolvedPath);
                
                return new KernelResponse
                {
                    Success = true,
                    Content = content
                };
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<KernelResponse> HandleUpdateRequestAsync(KernelRequest request)
        {
            try
            {
                if (!request.LineNumber.HasValue)
                {
                    return new KernelResponse
                    {
                        IsError = true,
                        ErrorMessage = "Line number is required for update operations"
                    };
                }

                var resolvedPath = await _aliasService.ResolvePathAsync(request.FilePath);
                var success = await _fileService.UpdateFileAsync(resolvedPath, request.LineNumber.Value, request.Content);
                
                return new KernelResponse
                {
                    Success = success
                };
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<KernelResponse> HandleCreateRequestAsync(KernelRequest request)
        {
            try
            {
                var resolvedPath = await _aliasService.ResolvePathAsync(request.FilePath);
                var success = await _fileService.CreateFileAsync(resolvedPath, request.Content, request.CreateDirectories);
                
                return new KernelResponse
                {
                    Success = success
                };
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<KernelResponse> HandleDeleteRequestAsync(KernelRequest request)
        {
            try
            {
                var resolvedPath = await _aliasService.ResolvePathAsync(request.FilePath);
                var success = await _fileService.DeleteFileAsync(resolvedPath);
                
                return new KernelResponse
                {
                    Success = success
                };
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }

        private async Task<KernelResponse> HandleTreeRequestAsync(KernelRequest request)
        {
            try
            {
                string directory = request.Directory;
                if (string.IsNullOrEmpty(directory))
                {
                    return new KernelResponse
                    {
                        IsError = true,
                        ErrorMessage = "Directory is required for tree operations"
                    };
                }

                var resolvedPath = await _aliasService.ResolvePathAsync(directory);
                var tree = await _directoryService.GetDirectoryTreeAsync(resolvedPath, request.Depth);
                
                return new KernelResponse
                {
                    Success = true,
                    Content = tree
                };
            }
            catch (Exception ex)
            {
                return new KernelResponse
                {
                    IsError = true,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
} 