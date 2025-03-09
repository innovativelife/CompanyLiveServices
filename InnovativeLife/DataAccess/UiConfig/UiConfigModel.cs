using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.UiConfig;

[FirestoreData]
public class UiConfigModel
{
    [FirestoreProperty]
    public string tenantId { get; set; }
    [FirestoreProperty]
    public string configId { get; set; }
    [FirestoreProperty]
    public string configName { get; set; }
    [FirestoreProperty]
    public string googleFont { get; set; }
    [FirestoreProperty]
    public string appBannerUrl { get; set; }
    [FirestoreProperty]
    public string appTitle { get; set; }
    [FirestoreProperty]
    public string homeTitle { get; set; }
    [FirestoreProperty]
    public string peopleTitle { get; set; }
    [FirestoreProperty]
    public string calendarTitle { get; set; }
    [FirestoreProperty]
    public string policyTitle { get; set; }
    [FirestoreProperty]
    public string moreTitle { get; set; }
    [FirestoreProperty]
    public string primaryColorHex { get; set; }
    [FirestoreProperty]
    public string highlightColorHex { get; set; }
    [FirestoreProperty]
    public string backgroundColorHex { get; set; }
    [FirestoreProperty]
    public string homePageSafeAreaColorHex { get; set; }
    [FirestoreProperty]
    public string appTitleColorHex { get; set; }
    [FirestoreProperty]
    public string appTitleBackgroundColorHex { get; set; }
}

// [FirestoreData]
// public class RgboColorModel
// {
//     [FirestoreProperty]
//     public int red { get; set; }
//     [FirestoreProperty]
//     public int green { get; set; }
//     [FirestoreProperty]
//     public int blue { get; set; }
//     [FirestoreProperty]
//     public float opacity { get; set; }
// }