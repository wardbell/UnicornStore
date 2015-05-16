using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;

/// <summary>
/// Example middleware
/// Add elapsed processing time to body (HTML only) and header
/// </summary>
public static class MyMiddleware {

    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder app)
    {
        return app.Use(next => context => TimeItAsync(context, next));
    }

    private static async Task TimeItAsync(HttpContext context, RequestDelegate next) { 
        var body = context.Response.Body;
        var buffer = new MemoryStream();
        context.Response.Body = buffer;

        var sw = new Stopwatch();
        sw.Start();

        try
        {
            // await context.Response.WriteAsync("Before\r\n"); // can prepend text too
            await next(context);

            sw.Stop();
            var elapsed = sw.ElapsedMilliseconds;

            context.Response.Headers.Add("X-ElapsedTime", new[] { elapsed.ToString() });

            var isHtml = context.Response.ContentType?.ToLower().Contains("text/html");
            if (context.Response.StatusCode == 200 && isHtml.GetValueOrDefault())
            {
                var text = $"\r\n<p style=\"margin-left:1em\">Page processed in {elapsed:n0}ms.</p>\r\n";
                // appended after </html> but most browsers will show it anyway
                await context.Response.WriteAsync(text);
            }
            buffer.Position = 0;
            await buffer.CopyToAsync(body);
        }
        finally
        {
            context.Response.Body = body;
        }
    }
}
