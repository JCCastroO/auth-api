using Newtonsoft.Json;

namespace Auth.Api.View.Handlers;

public static class ExceptionHandler
{
    public static IResult Error(Exception exception)
        => exception switch
        {
            ApplicationException e => Results.UnprocessableEntity(e.Message),
            UnauthorizedAccessException e => Results.Unauthorized(),
            _ => Results.InternalServerError(exception.Message)
        };
}