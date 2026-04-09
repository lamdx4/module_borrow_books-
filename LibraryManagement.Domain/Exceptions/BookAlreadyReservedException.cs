namespace LibraryManagement.Domain.Exceptions;

public class BookAlreadyReservedException : LibraryManagementException
{
    public BookAlreadyReservedException(string message = "Sách đã được người khác đặt trước.") : base(message) { }
}
