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
    public string tribesTitle { get; set; }
    [FirestoreProperty]
    public string moreTitle { get; set; }
    [FirestoreProperty]
    public string titleFontSize { get; set; }
    [FirestoreProperty]
    public string headingFontSize { get; set; }
    [FirestoreProperty]
    public string textFontSize { get; set; }
    [FirestoreProperty]
    public string subTextFontSize { get; set; }
    [FirestoreProperty]
    public string smallSpacing { get; set; }
    [FirestoreProperty]
    public string mediumSpacing { get; set; }
    [FirestoreProperty]
    public string largeSpacing { get; set; }
    [FirestoreProperty]
    public string primaryColor { get; set; }
    [FirestoreProperty]
    public string secondaryColor { get; set; }
    [FirestoreProperty]
    public string tertiaryColor { get; set; }
    [FirestoreProperty]
    public string backgroundColor { get; set; }
    [FirestoreProperty]
    public string textColor { get; set; }
    [FirestoreProperty]
    public string inputsColor { get; set; }
}
//phoneCallSvg

