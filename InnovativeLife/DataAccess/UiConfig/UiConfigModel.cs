using Google.Cloud.Firestore;

namespace InnovativeLife.DataAccess.UiConfig;

[FirestoreData]
public class UiConfigModel
{
    [FirestoreProperty]
    public string tenantId { get; set; } = "";
    [FirestoreProperty]
    public string configId { get; set; } = "";
    [FirestoreProperty]
    public string configName { get; set; } = "";
    [FirestoreProperty]
    public string googleFont { get; set; } = "";
    [FirestoreProperty]
    public string appBannerUrl { get; set; } = "";
    [FirestoreProperty]
    public string appTitle { get; set; } = "";
    [FirestoreProperty]
    public string homeTitle { get; set; } = "";
    [FirestoreProperty]
    public string peopleTitle { get; set; } = "";
    [FirestoreProperty]
    public string calendarTitle { get; set; } = "";
    [FirestoreProperty]
    public string policyTitle { get; set; } = "";
    [FirestoreProperty]
    public string moreTitle { get; set; } = "";
    [FirestoreProperty]
    public string primaryColorHex { get; set; } = "";
    [FirestoreProperty]
    public string headerColorHex { get; set; } = "";
    [FirestoreProperty]
    public string headerLoggedOutColorHex { get; set; } = "";
    [FirestoreProperty]
    public string footerColorHex { get; set; } = "";
    [FirestoreProperty]
    public string footerContentColorHex { get; set; } = "";
    [FirestoreProperty]
    public string footerCurrentPageColorHex { get; set; } = "";
    [FirestoreProperty]
    public string cardLightColorHex { get; set; } = "";
    [FirestoreProperty]
    public string cardDarkColorHex { get; set; } = "";
    [FirestoreProperty]
    public string buttonColorHex { get; set; } = "";
    [FirestoreProperty]
    public string buttonTextColorHex { get; set; } = "";
    [FirestoreProperty]
    public string headingColorHex { get; set; } = "";
    [FirestoreProperty]
    public string subHeadingColorHex { get; set; } = "";
    [FirestoreProperty]
    public string backgroundColorHex { get; set; } = "";
    [FirestoreProperty]
    public string textColorHex { get; set; } = "";
    [FirestoreProperty]
    public string calendarMeetingColorHex { get; set; } = "";
    [FirestoreProperty]
    public string calendarTaskColorHex { get; set; } = "";
    [FirestoreProperty]
    public string calendarEventColorHex { get; set; } = "";
    [FirestoreProperty]
    public string heading1FontSize { get; set; } = "";
    [FirestoreProperty]
    public string heading2FontSize { get; set; } = "";
    [FirestoreProperty]
    public string heading3FontSize { get; set; } = "";
    [FirestoreProperty]
    public string footerFontSize { get; set; } = "";
    [FirestoreProperty]
    public string normalTextFontSize { get; set; } = "";
    [FirestoreProperty]
    public string avatarTextFontSize { get; set; } = "";
    [FirestoreProperty]
    public string smallSpacing { get; set; } = "";
    [FirestoreProperty]
    public string mediumSpacing { get; set; } = "";
    [FirestoreProperty]
    public string largeSpacing { get; set; } = "";
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