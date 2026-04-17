namespace WebFrame;

/// <summary>
/// A page that presents the user with a set of navigable links.
/// Derive and implement <see cref="MenuLinks"/> to define the menu.
/// </summary>
public abstract class MenuPage : WebPage
{
    /// <summary>The links to display in the menu.</summary>
    public abstract IEnumerable<MenuLink> MenuLinks { get; }

    protected override Task<string> RenderAsync(Microsoft.AspNetCore.Http.HttpContext context)
    {
        var items = string.Join("\n", MenuLinks.Select(link =>
            $"""
            <li class="menu-item">
              <a href="{HtmlEncode(link.Url)}">{HtmlEncode(link.Text)}</a>
              {(string.IsNullOrWhiteSpace(link.Description) ? "" : $"<p class=\"menu-desc\">{HtmlEncode(link.Description)}</p>")}
            </li>
            """));

        var html = $$"""
            <style>
              ul.menu { list-style: none; padding: 0; display: grid; gap: 1rem; }
              .menu-item { background: #fff; border: 1px solid #dde3ea; border-radius: 8px; padding: 1.25rem 1.5rem; }
              .menu-item a { font-size: 1.1rem; font-weight: 600; color: #1a3c5e; text-decoration: none; }
              .menu-item a:hover { text-decoration: underline; }
              .menu-desc { margin: .4rem 0 0; color: #555; font-size: .9rem; }
            </style>
            <ul class="menu">
              {{items}}
            </ul>
            """;

        return Task.FromResult(html);
    }
}

/// <summary>Represents a single navigation link in a <see cref="MenuPage"/>.</summary>
public record MenuLink(string Text, string Url, string Description = "");
