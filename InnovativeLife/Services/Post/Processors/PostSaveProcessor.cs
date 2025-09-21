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
        string timeSent = DateTime.UtcNow.ToString("o");

        var postModel = new PostModel
        {
            tenantId = tenantId,
            postId = postId,
            timeSent = timeSent,
            status = saveRequest.status,
            sendTo = saveRequest.sendTo,
            employeeUID = saveRequest.employeeUID,
            message = saveRequest.message,
            imageURL = saveRequest.imageURL
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
                    postModel.message,
                    postModel.imageURL
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
    async Task<PostAddReplyResponse> IPostSaveProcessor.AddPostReply(string tenantId, string postId, PostAddReplyResquest postReply)
    {
        _logger.LogInformation("Executing Post Reply Save");

        var validationResult = postReply.Validate();
        if (validationResult.Count > 0)
        {
            return new PostAddReplyResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);

        }

        string postReplyId = Ulid.NewUlid().ToString();
        string timeSent = DateTime.UtcNow.ToString("o");

        var postModel = new PostReplyModel
        {
            timeSent = timeSent,
            postReplyId = postReplyId,
            employeeUID = postReply.employeeUID,
            message = postReply.message
        };

        var replyResponse = await _postActions.AddPostReply(tenantId, postId, postModel);

        if (replyResponse.Success)
        {
            _logger.LogInformation("Post Reply Saved successfully");

            var processorResponse = new PostAddReplyResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Post reply updated successfully")
            {
                postReply = new PostReply(
                    tenantId,
                    postId,
                    postModel.timeSent,
                    postModel.employeeUID,
                    postModel.message
                )
            };
            return processorResponse;
        }
        else
        {
            _logger.LogError($"Error saving Post Reply");
            return new PostAddReplyResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Post Reply could not be added due to unexpected error");
        }
    }

    async Task<PostAddReactionResponse> IPostSaveProcessor.AddPostReaction(string tenantId, string postId, PostAddReactionResquest postReaction)
    {
        _logger.LogInformation("Executing Post Reply Save");

        var validationResult = postReaction.Validate();
        if (validationResult.Count > 0)
        {
            return new PostAddReactionResponse(Common.ServiceResponseBase.ResponseStatus.InvalidData, validationResult);

        }

        string postReactionId = Ulid.NewUlid().ToString();
        string timeSent = DateTime.UtcNow.ToString("o");

        var postModel = new PostReactionModel
        {
            timeSent = timeSent,
            postReactionId = postReactionId,
            employeeUID = postReaction.employeeUID,
            reaction = postReaction.reaction
        };

        var reactionResponse = await _postActions.AddPostReaction(tenantId, postId, postModel);

        if (reactionResponse.Success)
        {
            _logger.LogInformation("Post Reaction Saved successfully");

            var processorResponse = new PostAddReactionResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Post reaction updated successfully")
            {
                postReaction = new PostReaction(
                    tenantId,
                    postId,
                    postModel.timeSent,
                    postModel.employeeUID,
                    postModel.reaction
                )
            };
            return processorResponse;
        }
        else
        {
            _logger.LogError($"Error saving Post Reaction");
            return new PostAddReactionResponse(Common.ServiceResponseBase.ResponseStatus.Exception, "Post Reaction could not be added due to unexpected error");
        }
    }
}