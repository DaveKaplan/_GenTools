using WebFrame;

namespace GTSample.Pages;

/// <summary>
/// The home/root page. Presents the top-level menu linking to all sample pages.
/// </summary>
public class RootPage : MenuPage
{
    public override string Title => "GTSample — Home";
    public override string Route => "/Sample";

    public override IEnumerable<MenuLink> MenuLinks =>
    [
        new MenuLink(
            "Static Page Sample",
            "/Sample/static",
            "Displays a pre-authored HTML article — no server-side logic required."),

        new MenuLink(
            "Dynamic Page Sample",
            "/Sample/dynamic",
            "Enter any question and let Claude search the web for an answer in real time."),
    ];
}
