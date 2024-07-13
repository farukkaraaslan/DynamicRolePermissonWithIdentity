namespace WebApp.Helpers;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Kullanıcı login değilse, login sayfasına yönlendir
        if (!context.User.Identity.IsAuthenticated)
        {
            if (context.Request.Path != "/Auth/Login")
            {
                context.Response.Redirect("/Auth/Login");
                return;
            }
        }

        await _next(context);
    }
}
