namespace LibraryManagement.Domain.Exceptions;

public class ReaderExpiredException : LibraryManagementException
{
    public ReaderExpiredException(string message = "Thẻ độc giả đã hết hạn.") : base(message) { }
}
