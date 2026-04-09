namespace LibraryManagement.Domain.Exceptions;

public class AlreadyReservedException : LibraryManagementException
{
    public AlreadyReservedException(string message = "Bạn đã đặt trước cuốn sách này rồi.") : base(message) { }
}
