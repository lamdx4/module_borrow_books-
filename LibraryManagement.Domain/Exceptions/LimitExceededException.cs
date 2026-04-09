namespace LibraryManagement.Domain.Exceptions;

public class LimitExceededException : LibraryManagementException
{
    public LimitExceededException(string message = "Vượt quá hạn mức cho phép.") : base(message) { }
}
