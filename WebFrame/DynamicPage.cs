using Microsoft.AspNetCore.Http;

namespace WebFrame;

/// <summary>
/// A page that generates its content at request time — e.g. from a database,
/// external API, or user input submitted via a form.
/// Derive and implement <see cref="GetDynamicContentAsync"/> to supply the HTML.
/// </summary>
public abstract class DynamicPage : WebPage
{
    protected override Task<string> RenderAsync(HttpContext context)
        => GetDynamicContentAsync(context);

    /// <summary>
    /// Called on every request. Receives the full <see cref="HttpContext"/>
    /// so the implementation can read query strings, form values, cookies, etc.
    /// Return an HTML fragment that will be wrapped in the shared layout.
    /// </summary>
    protected abstract Task<string> GetDynamicContentAsync(HttpContext context);
}
