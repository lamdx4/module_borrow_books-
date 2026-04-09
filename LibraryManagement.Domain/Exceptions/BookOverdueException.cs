namespace LibraryManagement.Domain.Exceptions;

public class BookOverdueException : LibraryManagementException
{
    public BookOverdueException(string message = "Sách đã quá hạn, không thể gia hạn thêm.") : base(message) { }
}
