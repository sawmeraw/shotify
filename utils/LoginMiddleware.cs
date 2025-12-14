public class LoginMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _password;

    public LoginMiddleware(RequestDelegate next)
    {
        _next = next;
        _password = Environment.GetEnvironmentVariable("LOGIN")!;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/login"))
        {
            await _next(context);
            return;
        }

        var authHeader = context.Request.Headers["Authorization"].ToString();
        if(string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            context.Response.Redirect("/login");
            return;
        }

        var token = authHeader.Substring("Bearer ".Length);

        //todo validate jwt
        // bool valid = ValidateJwt(token);
        await _next(context);
    }


}