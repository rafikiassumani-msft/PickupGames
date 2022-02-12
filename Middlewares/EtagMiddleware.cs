
using System.Security.Cryptography;

namespace PickUpGames.Middlewares;
public class EtagMiddleware {

    private RequestDelegate _next;

    public EtagMiddleware(RequestDelegate requestDelegate) {
        _next = requestDelegate;
    }

    public async Task InvokeAsync(HttpContext httpContext) {

        using var memoryStream = new MemoryStream();
        var originalStream = httpContext.Response.Body;
        httpContext.Response.Body = memoryStream;

        await _next(httpContext);

        if (httpContext.Request.Method == "GET" && httpContext.Response.StatusCode == StatusCodes.Status200OK ) {
            //Check if headers contain If-None-Match and compare with the computed etag
             // Compare the etagHeader with the computed value from the response result, 
             // If match, set back the header and set response status to 304
             // If they do not match, create new computed etag value, set it in the headers.
             // How do I access the response result?  

             //Create the etag value from the original stream
             using var sha256Algo = SHA256.Create();
             byte[] data = sha256Algo.ComputeHash(memoryStream);
             string computedEtagValue = Convert.ToBase64String(data);
             
             var etagHeader = httpContext.Request.Headers["If-None-Match"].FirstOrDefault();
             if (etagHeader is not null && etagHeader == computedEtagValue) {
                 httpContext.Response.StatusCode =  StatusCodes.Status304NotModified;
                 return;
             }
             
             //Reset the current position in the stream
             httpContext.Response.Headers.Add("Etag", computedEtagValue);
             memoryStream.Position = 0;
             await memoryStream.CopyToAsync(originalStream);
        }
   
    }

}

public static class EtagMiddlewareExtensions {

    public static IApplicationBuilder UseEtagResponseCaching(this IApplicationBuilder applicationBuilder) {

        return applicationBuilder.UseMiddleware<EtagMiddleware>();
    }
}