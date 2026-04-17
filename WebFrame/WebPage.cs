using Microsoft.AspNetCore.Http;

namespace WebFrame;

/// <summary>
/// Abstract base class for all WebFrame pages.
/// Handles layout wrapping and HTTP response writing.
/// </summary>
public abstract class WebPage
{
    public abstract string Title { get; }
    public abstract string Route { get; }

    /// <summary>Entry point called by ASP.NET Core routing.</summary>
    public async Task HandleAsync(HttpContext context)
    {
        var body = await RenderAsync(context);
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(WrapInLayout(body));
    }

    /// <summary>Subclasses return the inner HTML body content.</summary>
    protected abstract Task<string> RenderAsync(HttpContext context);

    /// <summary>Wraps body content in a full HTML page with shared styles.</summary>
    protected virtual string WrapInLayout(string body) => $$"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
          <meta charset="UTF-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1.0" />
          <title>{{HtmlEncode(Title)}}</title>
          <style>
            *, *::before, *::after { box-sizing: border-box; }
            body  { font-family: Segoe UI, sans-serif; margin: 0; background: #f5f7fa; color: #222; }
            header { background: #1a3c5e; color: #fff; padding: 1rem 2rem; }
            header h1 { margin: 0; font-size: 1.5rem; }
            nav.breadcrumb { background: #e8edf2; padding: .4rem 2rem; font-size: .875rem; }
            nav.breadcrumb a { color: #1a3c5e; text-decoration: none; }
            main  { padding: 2rem; max-width: 900px; margin: 0 auto; }
            footer { text-align: center; padding: 1rem; color: #888; font-size: .8rem; }
          </style>
          {{AdditionalHead()}}
        </head>
        <body>
          <header><h1>{{HtmlEncode(Title)}}</h1></header>
          <nav class="breadcrumb"><a href="/Sample">&#8962; Home</a></nav>
          <main>{{body}}</main>
          <footer>WebFrame &mdash; GTSample</footer>
        </body>
        </html>
        """;

    /// <summary>Override to inject extra &lt;head&gt; elements (styles, scripts).</summary>
    protected virtual string AdditionalHead() => string.Empty;

    protected static string HtmlEncode(string s) =>
        System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
}
