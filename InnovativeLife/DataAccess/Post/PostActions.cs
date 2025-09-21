using Google.Cloud.Firestore;
using InnovativeLife.DataAccess.Common;
using Microsoft.Extensions.Logging;

namespace InnovativeLife.DataAccess.Post;

public class PostActions : IPostActions
{
    private ILogger<PostActions> _logger;
    public PostActions(ILogger<PostActions> logger)
    {
        _logger = logger;
    }

    public async Task<Tuple<DalResponse, PostModel?>> ReadByPostId(string tenantId, string postId)
    {
        try
        {
            var db = Utilities.connectToFirestore();
            Query PostQuery = db.Collection(PostConstants.PostCollectionName).WhereEqualTo(PostConstants.postId, postId);
            QuerySnapshot PostQuerySnapshot = await PostQuery.GetSnapshotAsync();

            if (PostQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, PostModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new PostModel());
            }

            var value = PostQuerySnapshot[0].ConvertTo<PostModel>();

            _logger.LogInformation("PostActions.Read: GetPost Read Complete");

            return new Tuple<DalResponse, PostModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.Read: Exception {ex.Message}");
            return new Tuple<DalResponse, PostModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new PostModel());
        }
    }

    public async Task<Tuple<DalResponse, PostRepliesModel?>> ReadPostReplies(string tenantId, string postId)
    {
        try
        {
            var db = Utilities.connectToFirestore();
            Query PostQuery = db.Collection(PostConstants.PostCollectionName).WhereEqualTo(PostConstants.postId, postId);
            QuerySnapshot PostQuerySnapshot = await PostQuery.GetSnapshotAsync();

            if (PostQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, PostRepliesModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new PostRepliesModel());
            }

            var postRef = PostQuerySnapshot[0];
            var replyResult = await postRef.Reference.Collection(PostConstants.PostRepliesCollectionName).GetSnapshotAsync();
            var replyDocuments = replyResult.Documents;

            var response = new PostRepliesModel();
            response.tenantId = tenantId;
            response.postId = postId;

            foreach (DocumentSnapshot documentSnapshot in replyDocuments)
            {
                response.PostReplies.Add(documentSnapshot.ConvertTo<PostReplyModel>());
            }

            _logger.LogInformation("ReadByPostReplies.Read: GetPost Read Complete");

            return new Tuple<DalResponse, PostRepliesModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.Read: Exception {ex.Message}");
            return new Tuple<DalResponse, PostRepliesModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new PostRepliesModel());
        }
    }

    public async Task<Tuple<DalResponse, PostReactionsModel?>> ReadPostReactions(string tenantId, string postId)
    {
        try
        {
            var db = Utilities.connectToFirestore();
            Query PostQuery = db.Collection(PostConstants.PostCollectionName).WhereEqualTo(PostConstants.postId, postId);
            QuerySnapshot PostQuerySnapshot = await PostQuery.GetSnapshotAsync();

            if (PostQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, PostReactionsModel?>(new DalResponse(DalResponse.ResponseStatus.NotFound), new PostReactionsModel());
            }

            var postRef = PostQuerySnapshot[0];
            var reactionResult = await postRef.Reference.Collection(PostConstants.PostReactionsCollectionName).GetSnapshotAsync();
            var reactionsDocuments = reactionResult.Documents;

            var response = new PostReactionsModel();
            response.tenantId = tenantId;
            response.postId = postId;

            foreach (DocumentSnapshot documentSnapshot in reactionsDocuments)
            {
                response.PostReactionss.Add(documentSnapshot.ConvertTo<PostReactionModel>());
            }

            _logger.LogInformation("PostActions.ReadPostReactions: GetPost Reactions Read Complete");

            return new Tuple<DalResponse, PostReactionsModel?>(new DalResponse(DalResponse.ResponseStatus.Ok), response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.ReadPostReactions: Exception {ex.Message}");
            return new Tuple<DalResponse, PostReactionsModel?>(new DalResponse(DalResponse.ResponseStatus.Exception), new PostReactionsModel());
        }
    }

    async Task<Tuple<DalResponse, List<PostModel>>> IPostActions.Search(string tenantId, string? postId, string? timeSent, string? status, string? sendTo, string? employeeUID, string? message)
    {
        try
        {
            _logger.LogInformation($"EmployeeActions.Search starting with parameters: tenantId: {tenantId} | postId: {postId} | timeSent: {timeSent} | status: {status} | sendTo: {sendTo} | employeeUID: {employeeUID} | message: {message}");

            var db = Utilities.connectToFirestore();
            Query employeeQuery = db.Collection(PostConstants.PostCollectionName);

            employeeQuery = employeeQuery.WhereEqualTo(PostConstants.tenantId, tenantId);

            employeeQuery = employeeQuery.WhereEqualTo(PostConstants.status, "sent");

            if (!string.IsNullOrEmpty(postId))
            {
                employeeQuery = employeeQuery.WhereEqualTo(PostConstants.postId, postId);
            }

            if (!string.IsNullOrEmpty(timeSent))
            {
                employeeQuery = employeeQuery.WhereEqualTo(PostConstants.timeSent, timeSent);
            }

            if (!string.IsNullOrEmpty(sendTo))
            {
                employeeQuery = employeeQuery.WhereEqualTo(PostConstants.sendTo, status);
            }

            if (!string.IsNullOrEmpty(employeeUID))
            {
                employeeQuery = employeeQuery.WhereEqualTo(PostConstants.employeeUID, employeeUID);
            }

            if (!string.IsNullOrEmpty(message))
            {
                employeeQuery = employeeQuery.WhereEqualTo(PostConstants.message, message);
            }

            QuerySnapshot employeeQuerySnapshot = await employeeQuery.GetSnapshotAsync();

            if (employeeQuerySnapshot.Count == 0)
            {
                return new Tuple<DalResponse, List<PostModel>>(new DalResponse(DalResponse.ResponseStatus.NotFound), new List<PostModel>());
            }

            var employees = new List<PostModel>();
            foreach (DocumentSnapshot documentSnapshot in employeeQuerySnapshot.Documents)
            {
                employees.Add(documentSnapshot.ConvertTo<PostModel>());
            }

            _logger.LogInformation($"EmployeeActions.Search: Complete with {employees.Count} employees returned");

            return new Tuple<DalResponse, List<PostModel>>(new DalResponse(DalResponse.ResponseStatus.Ok), employees);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"EmployeeActions.Search: Exception {ex.Message}");

            return new Tuple<DalResponse, List<PostModel>>(new DalResponse(DalResponse.ResponseStatus.Ok), new List<PostModel>());
        }
    }

    public async Task<DalResponse> AddPostReply(string tenantId, string postId, PostReplyModel postReply)
    {
        try
        {
            _logger.LogInformation("PostReplyActions.AddPostReply: Adding reply to post {0}", postId);

            var db = Utilities.connectToFirestore();
            Query PostQuery = db.Collection(PostConstants.PostCollectionName).WhereEqualTo(PostConstants.postId, postId);
            QuerySnapshot PostQuerySnapshot = await PostQuery.GetSnapshotAsync();

            if (PostQuerySnapshot.Count == 0)
            {
                return new DalResponse(DalResponse.ResponseStatus.NotFound);
            }

            var postRef = PostQuerySnapshot[0].Reference;
            DocumentReference postReplyDocument = postRef.Collection(PostConstants.PostRepliesCollectionName).Document(postReply.postReplyId);

            var result = await postReplyDocument.SetAsync(postReply);

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.AddPostReply: Exception - {ex.Message}");
            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }

    public async Task<DalResponse> AddPostReaction(string tenantId, string postId, PostReactionModel postReaction)
    {
        try
        {
            _logger.LogInformation("PostReactActions.AddPostReaction: Adding reaction to post {0}", postId);

            var db = Utilities.connectToFirestore();
            Query PostQuery = db.Collection(PostConstants.PostCollectionName).WhereEqualTo(PostConstants.postId, postId);
            QuerySnapshot PostQuerySnapshot = await PostQuery.GetSnapshotAsync();

            if (PostQuerySnapshot.Count == 0)
            {
                return new DalResponse(DalResponse.ResponseStatus.NotFound);
            }

            var postRef = PostQuerySnapshot[0].Reference;
            DocumentReference postReactionDocument = postRef.Collection(PostConstants.PostReactionsCollectionName).Document(postReaction.postReactionId);

            var result = await postReactionDocument.SetAsync(postReaction);

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.AddPostReaction: Exception - {ex.Message}");
            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }

    public async Task<DalResponse> Save(string tenantId, PostModel postModel)
    {
        try
        {
            _logger.LogInformation("PostActions.Save: Saving post {0}", postModel.employeeUID);

            var db = Utilities.connectToFirestore();
            CollectionReference collection = db.Collection(PostConstants.PostCollectionName);
            DocumentReference postRef = db.Collection(PostConstants.PostCollectionName).Document(postModel.postId);

            var result = await postRef.SetAsync(postModel);

            return new DalResponse(DalResponse.ResponseStatus.Ok);
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostActions.Save: Exception - {ex.Message}");
            return new DalResponse(DalResponse.ResponseStatus.Exception);
        }
    }
}