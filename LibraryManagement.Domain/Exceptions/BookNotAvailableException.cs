namespace LibraryManagement.Domain.Exceptions;

public class BookNotAvailableException : LibraryManagementException
{
    public BookNotAvailableException(string message = "Sách hiện không sẵn sàng để mượn.") : base(message) { }
}
