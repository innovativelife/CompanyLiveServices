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
    public string searchPromptText { get; set; }
    [FirestoreProperty]
    public string homeSvg { get; set; }
    [FirestoreProperty]
    public string peopleSvg { get; set; }
    [FirestoreProperty]
    public string calendarSvg { get; set; }
    [FirestoreProperty]
    public string policySvg { get; set; }
    [FirestoreProperty]
    public string moreSvg { get; set; }
    [FirestoreProperty]
    public string loginTopBarColor { get; set; }
    [FirestoreProperty]
    public string loginContainerColor { get; set; }
    [FirestoreProperty]
    public string loginBackgroundColor { get; set; }
    [FirestoreProperty]
    public string loginButtonsColor { get; set; }
    [FirestoreProperty]
    public string loginTextFieldColor { get; set; }
    [FirestoreProperty]
    public string backgroundColor { get; set; }
    [FirestoreProperty]
    public string headingColor { get; set; }
    [FirestoreProperty]
    public string textColor { get; set; }
    [FirestoreProperty]
    public string textFieldColor { get; set; }
    [FirestoreProperty]
    public string topSearchColor { get; set; }
    [FirestoreProperty]
    public string topSearchBoarderColor { get; set; }
    [FirestoreProperty]
    public string topBarColor { get; set; }
    [FirestoreProperty]
    public string breadCrumbBarColor { get; set; }
    [FirestoreProperty]
    public string breadCrumbColorRgb { get; set; }
    [FirestoreProperty]
    public string bottomBarColor { get; set; }
    [FirestoreProperty]
    public string widgets1Color { get; set; }
    [FirestoreProperty]
    public string widgets2Color { get; set; }
    [FirestoreProperty]
    public string bottomButttonSelectedColor { get; set; }
    [FirestoreProperty]
    public string bottomButttonUnselectedColor { get; set; }
    [FirestoreProperty]
    public string buttonColor { get; set; }
    [FirestoreProperty]
    public string buttonTextColor { get; set; }
    [FirestoreProperty]
    public string heading1FontSize { get; set; }
    [FirestoreProperty]
    public string heading2FontSize { get; set; }
    [FirestoreProperty]
    public string heading3FontSize { get; set; }
    [FirestoreProperty]
    public string footerFontSize { get; set; }
    [FirestoreProperty]
    public string normalTextFontSize { get; set; }
    [FirestoreProperty]
    public string avatarTextFontSize { get; set; }
    [FirestoreProperty]
    public string smallSpacing { get; set; }
    [FirestoreProperty]
    public string mediumSpacing { get; set; }
    [FirestoreProperty]
    public string largeSpacing { get; set; }
    [FirestoreProperty]
    public string searchSvg { get; set; }
    [FirestoreProperty]
    public string backSvg { get; set; }
    [FirestoreProperty]
    public string favouriteSvg { get; set; }
    [FirestoreProperty]
    public string messageSvg { get; set; }
    [FirestoreProperty]
    public string phoneCallSvg { get; set; }
}

//  public string primaryColorHex { get; set; }
//     [FirestoreProperty]
//     public string highlightColorHex { get; set; }
//     [FirestoreProperty]
//     public string backgroundColorHex { get; set; }
//     [FirestoreProperty]
//     public string homePageSafeAreaColorHex { get; set; }
//     [FirestoreProperty]
//     public string appTitleColorHex { get; set; }
//     [FirestoreProperty]
//     public string appTitleBackgroundColorHex { get; set; }

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