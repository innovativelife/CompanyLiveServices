using NUlid;
using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.DataAccess.Post;

namespace InnovativeLife.Services.Post.Processors;

public class PostSaveProcessor : IPostSaveProcessor
{
    private readonly ILogger _logger;
    private readonly IPostActions _postActions;

    public PostSaveProcessor(ILogger<IPostSaveProcessor> logger, IPostActions postActions)
    {
        _logger = logger;
        _postActions = postActions;
    }

    async Task<PostSaveResponse> IPostSaveProcessor.Save(IUserContext requestContext, string tenantId, PostSaveRequest saveRequest)
    {
        _logger.LogInformation("Executing Post Save");

        var validationResult = saveRequest.Validate();
        if (validationResult.Count > 0)
        {
            return new PostSaveResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);
        }

        string postId = Ulid.NewUlid().ToString();
        string timeSent = DateTimeOffset.UtcNow.ToString();

        var postModel = new PostModel
        {
            tenantId = tenantId,
            postId = postId,
            timeSent = timeSent,
            status = saveRequest.status,
            sendTo = saveRequest.sendTo,
            employeeUID = saveRequest.employeeUID,
            message = saveRequest.message
        };

        var saveResponse = await _postActions.Save(tenantId, postModel);

        if (saveResponse.Success)
        {
            _logger.LogInformation("Post Saved successfully");

            var processorResponse = new PostSaveResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Post updated successfully")
            {
                postItem = new PostItem(
                    postModel.tenantId,
                    postModel.postId,
                    postModel.timeSent,
                    postModel.status,
                    postModel.sendTo,
                    postModel.employeeUID,
                    postModel.message
                )
            };
            return processorResponse;
        }
        else
        {
            _logger.LogError($"Error saving Post");
            return new PostSaveResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Post could not be added due to unexpected error");
        }
    }
}