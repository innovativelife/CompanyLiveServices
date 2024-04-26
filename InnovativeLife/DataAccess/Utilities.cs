using Google.Cloud.Firestore;
using Google.Api.Gax;
using InnovativeLife.Common;

namespace InnovativeLife.DataAccess;

public static class Utilities
{
    public static FirestoreDb connectToFirestore()
    {
        return new FirestoreDbBuilder
        {
            ProjectId = GcpConstants.ProjectId,
            EmulatorDetection = EmulatorDetection.EmulatorOrProduction
        }.Build();
    }

    public static Query appendCriteria(CollectionReference collection, Query? query, string column, string? value)
    {
        // parameter not provided
        if (String.IsNullOrEmpty(value))
        {
            return query;
        }

        if (query == null)
        {
            return collection.WhereEqualTo(column, value);
        }
        else 
        {
            return query.WhereEqualTo(column, value);
        }

    }
}
