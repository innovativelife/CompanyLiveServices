using Microsoft.Extensions.Logging;
using InnovativeLife.Security;
using InnovativeLife.Services.Post.ServiceMessages;
using InnovativeLife.DataAccess.Post;

namespace InnovativeLife.Services.Post.Processors;

public class PostReadProcessor : IPostReadProcessor
{
    private readonly ILogger _logger;
    private readonly IPostActions _postActions;

    public PostReadProcessor(ILogger<IPostReadProcessor> logger, IPostActions postActions)
    {
        _logger = logger;
        _postActions = postActions;
    }

    public async Task<PostReadResponse> ReadSingleton(IUserContext requestContext, string tenantId, string postId)
    {
        _logger.LogInformation("Executing Post Read by postId");

        var result = await _postActions.ReadByPostId(tenantId, postId);

        if (result.Item1.Success)
        {
            var response = new PostReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, "Post Found");
            response.post = getPostItemFromPostModel(tenantId, result.Item2);

            return response;
        }
        else
        {
            return new PostReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, $"Post not found.  PostId: {postId}");
        }
    }

    public async Task<PostRepliesReadResponse> ReadReplies(IUserContext requestContext, string tenantId, string? postId)
    {
        _logger.LogInformation("Executing Post Replies Read by postId");

        var searchResult = await _postActions.ReadPostReplies(requestContext.tenantId, postId);

        if (searchResult.Item1.Success)
        {
            var responseList = new List<PostReply>();
            foreach (var item in searchResult.Item2.PostReplies)
            {
                var postReply = new PostReply(
                    tenantId,
                    postId,
                    item.timeSent,
                    item.employeeUID,
                    item.message
                );
                responseList.Add(getPostRepliesFromPostModel(tenantId, postReply));
            }
            var response = new PostRepliesReadResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"{responseList.Count} Post(s) Found");
            response.replies = responseList;
            return response;
        }

        return new PostRepliesReadResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "Post search returned no results");
    }

    public async Task<PostSearchResponse> SearchPost(IUserContext requestContext, string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message)
    {
        var searchResult = await _postActions.Search(requestContext.tenantId, postId, timeSent, status, sendTo, employeeUID, message);

        if (searchResult.Item1.Success)
        {
            var postList = new List<PostItem>();
            foreach (var item in searchResult.Item2)
            {
                postList.Add(getPostItemFromPostModel(tenantId, item));
            }
            var response = new PostSearchResponse(Common.ServiceResponseBase.ResponseStatus.Ok, $"{postList.Count} Post(s) Found");
            response.posts = postList;
            return response;
        }

        return new PostSearchResponse(Common.ServiceResponseBase.ResponseStatus.NotFound, "Post search returned no results");
    }

    private PostItem getPostItemFromPostModel(string tenantId, PostModel postModel)
    {
        return new PostItem(
            postModel.tenantId,
            postModel.postId,
            postModel.timeSent,
            postModel.status,
            postModel.sendTo,
            postModel.employeeUID,
            postModel.message,
            postModel.imageURL
        );
    }

    private PostReply getPostRepliesFromPostModel(string tenantId, PostReply postReply)
    {
        return new PostReply(
            postReply.tenantId,
            postReply.postId,
            postReply.timeSent,
            postReply.employeeUID,
            postReply.message
        );
    }



}