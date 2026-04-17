using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Tools.Pages;
using WebFrame;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app
    .MapWebPage<ToolsRootPage>()
    .MapWebPage<SpanishPage>();

// Azure Neural TTS proxy — returns MP3 audio for es-MX-DaliaNeural
app.MapGet("/Tools/Spanish/tts", async (string text, string? voice, HttpContext ctx) =>
{
    var key    = Environment.GetEnvironmentVariable("AZURE_SPEECH_KEY")    ?? "";
    var region = Environment.GetEnvironmentVariable("AZURE_SPEECH_REGION") ?? "eastus";

    if (string.IsNullOrWhiteSpace(key))
    {
        ctx.Response.StatusCode = 503;
        await ctx.Response.WriteAsync("AZURE_SPEECH_KEY not configured.");
        return;
    }

    // Only allow known safe voice names
    var voiceName = voice == "m" ? "es-MX-JorgeNeural" : "es-MX-DaliaNeural";

    // Sanitise input — strip anything that could break SSML
    var safe = System.Net.WebUtility.HtmlEncode(text.Trim());
    if (safe.Length > 200) safe = safe[..200];

    var ssml = $"""
        <speak version='1.0' xml:lang='es-MX'>
          <voice xml:lang='es-MX' name='{voiceName}'>
            <prosody rate='0.9'>{safe}</prosody>
          </voice>
        </speak>
        """;

    using var http = new HttpClient();
    using var req  = new HttpRequestMessage(
        HttpMethod.Post,
        $"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1");

    req.Headers.Add("Ocp-Apim-Subscription-Key", key);
    req.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-128kbitrate-mono-mp3");
    req.Headers.Add("User-Agent", "GTSample-Tools");
    req.Content = new StringContent(ssml, Encoding.UTF8, "application/ssml+xml");

    using var res = await http.SendAsync(req);

    if (!res.IsSuccessStatusCode)
    {
        ctx.Response.StatusCode = (int)res.StatusCode;
        await ctx.Response.WriteAsync($"Azure TTS error: {res.StatusCode}");
        return;
    }

    var audio = await res.Content.ReadAsByteArrayAsync();
    ctx.Response.ContentType = "audio/mpeg";
    await ctx.Response.Body.WriteAsync(audio);
});

// Translation endpoint — Claude Haiku, EN↔ES
app.MapPost("/Tools/Spanish/translate", async (HttpContext ctx) =>
{
    var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY") ?? "";
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        ctx.Response.StatusCode = 503;
        await ctx.Response.WriteAsync("ANTHROPIC_API_KEY not configured.");
        return;
    }

    var form = await ctx.Request.ReadFormAsync();
    var text = (form["text"].ToString() ?? "").Trim();
    var dir  = form["dir"].ToString() == "es2en" ? "es2en" : "en2es";
    if (text.Length > 100) text = text[..100];
    if (string.IsNullOrWhiteSpace(text)) { await ctx.Response.WriteAsync(""); return; }

    var prompt = dir == "en2es"
        ? $"You are a translator. The user has typed English text. Translate it into Mexican Spanish. Output ONLY the Spanish translation — no explanation, no original text, no labels:\n\n{text}"
        : $"You are a translator. The user has typed Spanish text. Translate it into English. Output ONLY the English translation — no explanation, no original text, no labels:\n\n{text}";

    var body = JsonSerializer.Serialize(new
    {
        model = "claude-haiku-4-5-20251001",
        max_tokens = 256,
        messages = new[] { new { role = "user", content = prompt } }
    });

    using var http = new HttpClient();
    using var req  = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
    req.Headers.Add("x-api-key", apiKey);
    req.Headers.Add("anthropic-version", "2023-06-01");
    req.Content = new StringContent(body, Encoding.UTF8, "application/json");

    using var res = await http.SendAsync(req);
    var json = await res.Content.ReadAsStringAsync();
    if (!res.IsSuccessStatusCode) { ctx.Response.StatusCode = 500; await ctx.Response.WriteAsync("API error."); return; }

    var text2 = JsonNode.Parse(json)?["content"]?[0]?["text"]?.GetValue<string>() ?? "";
    ctx.Response.ContentType = "text/plain; charset=utf-8";
    await ctx.Response.WriteAsync(text2.Trim());
});

app.Run();
