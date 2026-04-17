using Microsoft.AspNetCore.Builder;

namespace WebFrame;

/// <summary>
/// Extension methods for registering <see cref="WebPage"/> instances with an
/// ASP.NET Core <see cref="WebApplication"/>.
/// </summary>
public static class WebFrameExtensions
{
    /// <summary>
    /// Creates an instance of <typeparamref name="T"/> and maps its
    /// <see cref="WebPage.Route"/> to its <see cref="WebPage.HandleAsync"/> handler.
    /// </summary>
    public static WebApplication MapWebPage<T>(this WebApplication app)
        where T : WebPage, new()
    {
        var page = new T();
        app.Map(page.Route, page.HandleAsync);
        return app;
    }
}
