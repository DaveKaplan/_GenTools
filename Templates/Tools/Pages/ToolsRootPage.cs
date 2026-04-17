using WebFrame;

namespace Tools.Pages;

public class ToolsRootPage : MenuPage
{
    public override string Title => "Tools";
    public override string Route => "/Tools";

    public override IEnumerable<MenuLink> MenuLinks =>
    [
        new MenuLink(
            "Spanish",
            "/Tools/Spanish",
            "Browse common Spanish nouns, verbs, and random vocabulary with English translations."),
    ];
}
