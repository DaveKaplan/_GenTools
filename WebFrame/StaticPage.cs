using Microsoft.AspNetCore.Http;

namespace WebFrame;

/// <summary>
/// A page that serves fixed, pre-authored HTML content.
/// Derive and implement <see cref="HtmlContent"/> to provide the markup.
/// </summary>
public abstract class StaticPage : WebPage
{
    /// <summary>The raw HTML fragment to render inside the page layout.</summary>
    protected abstract string HtmlContent { get; }

    protected override Task<string> RenderAsync(HttpContext context)
        => Task.FromResult(HtmlContent);
}
