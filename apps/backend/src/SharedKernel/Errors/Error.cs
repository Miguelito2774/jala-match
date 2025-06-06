namespace SharedKernel.Errors;

public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static readonly Error NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure
    );

    public static Error Failure(string code, string description)
    {
        return new Error(code, description, ErrorType.Failure);
    }

    public static Error NotFound(string code, string description)
    {
        return new Error(code, description, ErrorType.NotFound);
    }

    public static Error Problem(string code, string description)
    {
        return new Error(code, description, ErrorType.Problem);
    }

    public static Error Conflict(string code, string description)
    {
        return new Error(code, description, ErrorType.Conflict);
    }

    public static Error Unauthorized(string code, string description)
    {
        return new Error(code, description, ErrorType.Unauthorized);
    }

    public static Error Forbidden(string code, string description)
    {
        return new Error(code, description, ErrorType.Forbidden);
    }
    
    public static Error Validation(string code, string description)
    {
        return new Error(code, description, ErrorType.Validation);
    }
}
