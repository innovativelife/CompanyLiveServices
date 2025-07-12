using System.Text.Json;
using Microsoft.Extensions.Logging;
using InnovativeLife.DataAccess.Post;
using InnovativeLife.Security;
using InnovativeLife.WebApi;
using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.Services.Post.Processors;
using InnovativeLife.Services.Common;

namespace InnovativeLife.Services.Post;

public class PostService : IPostService
{
    private ILogger<PostService> _logger;
    private IPostReadProcessor _readPostProcessor;
    private IPostSaveProcessor _savePostProcessor;

    public PostService(ILogger<PostService> logger, IPostReadProcessor readPostProcessor, IPostSaveProcessor savePostProcessor)
    {
        _logger = logger;
        _readPostProcessor = readPostProcessor;
        _savePostProcessor = savePostProcessor;
    }

    public async Task<PostReadResponse> Read(IUserContext requestContext, string tenantId, string postId)
    {
        _logger.LogInformation("PostService.Read: Executing PostService.Read Read");

        return await _readPostProcessor.ReadSingleton(requestContext, tenantId, postId);
    }

    public async Task<PostRepliesReadResponse> ReadReplies(IUserContext requestContext, string tenantId, string postId)
    {
        _logger.LogInformation("PostService.ReadReplies: Executing PostService.Read ReadReplies");

        return await _readPostProcessor.ReadReplies(requestContext, tenantId, postId);
    }

    public async Task<PostSearchResponse> SearchPost(IUserContext requestContext, string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message)
    {
        var validation = validateTenantId(requestContext, tenantId, false);
        if (validation.Item1 != ServiceResponseBase.ResponseStatus.Ok)
        {
            return new PostSearchResponse(validation.Item1, validation.Item2);
        }

        return await _readPostProcessor.SearchPost(requestContext, tenantId, postId, timeSent, status, sendTo, employeeUID, message);
    }

    public async Task<PostSaveResponse> Save(IUserContext requestContext, string tenantId, PostSaveRequest postModel)
    {
        _logger.LogInformation("PostService.Save: Executing PostService.Save Save");

        return await _savePostProcessor.Save(requestContext, tenantId, postModel);
    }

    public async Task<PostAddReplyResponse> AddPostReply(IUserContext requestContext, string tenantId, string postId, PostAddReplyResquest postReply)
    {
        _logger.LogInformation("PostService.AddPostReply: Executing PostService.AddPostReply Reply");

        return await _savePostProcessor.AddPostReply(tenantId, postId, postReply);
    }

    private Tuple<ServiceResponseBase.ResponseStatus, string> validateTenantId(IUserContext requestContext, string tenantId, bool allowRoot)
    {
        if (allowRoot && requestContext.rootAdmin)
        {
            // operation permitted - this is to enable Adding admin employees when creating new tenant
        }
        else if (!string.Equals(requestContext.tenantId, tenantId))
        {
            return new Tuple<ServiceResponseBase.ResponseStatus, string>(ServiceResponseBase.ResponseStatus.BadRequest, "Tenant mismatch");
        }

        return new Tuple<ServiceResponseBase.ResponseStatus, string>(ServiceResponseBase.ResponseStatus.Ok, "");
    }
}