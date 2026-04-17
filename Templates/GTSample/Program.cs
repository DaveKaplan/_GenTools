using GTSample.Pages;
using WebFrame;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Register all pages via WebFrame's MapWebPage extension
app
    .MapWebPage<RootPage>()
    .MapWebPage<StaticPageSample>()
    .MapWebPage<DynamicPageSample>();

app.Run();
