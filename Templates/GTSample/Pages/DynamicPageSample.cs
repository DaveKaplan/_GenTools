using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using WebFrame;

namespace GTSample.Pages;

/// <summary>
/// Demonstrates <see cref="DynamicPage"/>: accepts a user query, calls the
/// Claude API with the built-in web_search tool, and renders the response.
/// </summary>
public class DynamicPageSample : DynamicPage
{
    public override string Title => "Dynamic Page Sample — Claude Web Search";
    public override string Route => "/Sample/dynamic";

    protected override string AdditionalHead() => """
        <style>
          .search-box { display:flex; gap:.5rem; margin-bottom:1.5rem; }
          .search-box input  { flex:1; padding:.6rem 1rem; font-size:1rem;
                               border:1px solid #ccd3db; border-radius:6px; }
          .search-box button { padding:.6rem 1.4rem; background:#1a3c5e; color:#fff;
                               border:none; border-radius:6px; font-size:1rem; cursor:pointer; }
          .search-box button:hover { background:#265a8a; }
          .result-card { background:#fff; border:1px solid #dde3ea; border-radius:8px;
                         padding:1.5rem 2rem; line-height:1.7; }
          .result-card h3 { margin-top:1.2rem; color:#1a3c5e; }
          .result-card a  { color:#1a6ab0; }
          .spinner { display:none; }
          .error-box { background:#fff0f0; border:1px solid #f5c6c6; border-radius:6px;
                       padding:1rem 1.5rem; color:#b00; }
          .hint { color:#777; font-size:.875rem; margin-bottom:1rem; }
        </style>
        """;

    protected override async Task<string> GetDynamicContentAsync(HttpContext context)
    {
        var query = string.Empty;
        var resultHtml = string.Empty;

        if (context.Request.Method == "POST")
        {
            var form = await context.Request.ReadFormAsync();
            query = form["query"].ToString().Trim();

            if (!string.IsNullOrEmpty(query))
                resultHtml = await RunClaudeSearchAsync(query);
        }

        var encodedQuery = HtmlEncode(query);
        var resultSection = resultHtml.Length > 0
            ? $"<div class=\"result-card\">{resultHtml}</div>"
            : string.Empty;

        return $"""
            <form method="post" action="/Sample/dynamic">
              <p class="hint">Enter any question — Claude will search the web and summarise the answer.</p>
              <div class="search-box">
                <input type="text" name="query" value="{encodedQuery}"
                       placeholder="e.g. Latest news on fusion energy" autofocus />
                <button type="submit">Search</button>
              </div>
            </form>
            {resultSection}
            <p><a href="/Sample">&larr; Back to Home</a></p>
            """;
    }

    // -------------------------------------------------------------------------
    // Claude API — web_search_20250305 built-in tool
    // -------------------------------------------------------------------------

    private static readonly HttpClient Http = new();

    private static async Task<string> RunClaudeSearchAsync(string query)
    {
        var apiKey = Environment.GetEnvironmentVariable("ANTHROPIC_API_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
            return ErrorBox("ANTHROPIC_API_KEY environment variable is not set. " +
                            "Set it and restart the application.");

        var requestBody = new
        {
            model = "claude-haiku-4-5-20251001",
            max_tokens = 2048,
            tools = new[]
            {
                new { type = "web_search_20250305", name = "web_search" }
            },
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = $"Please search the web and give me a clear, well-structured answer to: {query}"
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var request = new HttpRequestMessage(HttpMethod.Post,
            "https://api.anthropic.com/v1/messages");
        request.Headers.Add("x-api-key", apiKey);
        request.Headers.Add("anthropic-version", "2023-06-01");
        request.Headers.Add("anthropic-beta", "web-search-2025-03-05");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using var response = await Http.SendAsync(request);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return ErrorBox($"API error {(int)response.StatusCode}: {HtmlEncodeStatic(responseJson)}");

            return ParseResponse(responseJson);
        }
        catch (Exception ex)
        {
            return ErrorBox($"Request failed: {HtmlEncodeStatic(ex.Message)}");
        }
    }

    private static string ParseResponse(string json)
    {
        try
        {
            var doc = JsonNode.Parse(json)!;
            var content = doc["content"]?.AsArray();
            if (content is null) return ErrorBox("Unexpected API response format.");

            var sb = new StringBuilder();
            foreach (var block in content)
            {
                var type = block?["type"]?.GetValue<string>();
                if (type == "text")
                {
                    var text = block!["text"]?.GetValue<string>() ?? string.Empty;
                    sb.Append(MarkdownToHtml(text));
                }
            }
            return sb.Length > 0 ? sb.ToString() : "<p><em>No text response received.</em></p>";
        }
        catch (Exception ex)
        {
            return ErrorBox($"Failed to parse response: {HtmlEncodeStatic(ex.Message)}");
        }
    }

    /// <summary>
    /// Very lightweight Markdown → HTML pass (headers, bold, italic, code, links).
    /// For full Markdown support, swap in a library like Markdig.
    /// </summary>
    private static string MarkdownToHtml(string md)
    {
        if (string.IsNullOrEmpty(md)) return string.Empty;

        var lines = md.Split('\n');
        var sb = new StringBuilder();
        bool inList = false;

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd();

            if (line.StartsWith("### "))
            { CloseList(sb, ref inList); sb.Append($"<h3>{InlineFormat(line[4..])}</h3>\n"); continue; }
            if (line.StartsWith("## "))
            { CloseList(sb, ref inList); sb.Append($"<h2>{InlineFormat(line[3..])}</h2>\n"); continue; }
            if (line.StartsWith("# "))
            { CloseList(sb, ref inList); sb.Append($"<h2>{InlineFormat(line[2..])}</h2>\n"); continue; }

            if (line.StartsWith("- ") || line.StartsWith("* "))
            {
                if (!inList) { sb.Append("<ul>\n"); inList = true; }
                sb.Append($"<li>{InlineFormat(line[2..])}</li>\n");
                continue;
            }

            CloseList(sb, ref inList);

            if (string.IsNullOrWhiteSpace(line))
                sb.Append("<br/>\n");
            else
                sb.Append($"<p>{InlineFormat(line)}</p>\n");
        }

        CloseList(sb, ref inList);
        return sb.ToString();
    }

    private static void CloseList(StringBuilder sb, ref bool inList)
    {
        if (inList) { sb.Append("</ul>\n"); inList = false; }
    }

    private static string InlineFormat(string s)
    {
        // Bold, italic, inline code, links — order matters
        s = System.Text.RegularExpressions.Regex.Replace(s,
            @"\*\*(.+?)\*\*", "<strong>$1</strong>");
        s = System.Text.RegularExpressions.Regex.Replace(s,
            @"\*(.+?)\*", "<em>$1</em>");
        s = System.Text.RegularExpressions.Regex.Replace(s,
            @"`(.+?)`", "<code>$1</code>");
        s = System.Text.RegularExpressions.Regex.Replace(s,
            @"\[(.+?)\]\((https?://[^\)]+)\)", "<a href=\"$2\" target=\"_blank\">$1</a>");
        return s;
    }

    private static string ErrorBox(string msg) =>
        $"<div class=\"error-box\"><strong>Error:</strong> {msg}</div>";

    private static string HtmlEncodeStatic(string s) =>
        System.Net.WebUtility.HtmlEncode(s ?? string.Empty);
}
