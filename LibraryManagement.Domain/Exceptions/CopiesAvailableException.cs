namespace LibraryManagement.Domain.Exceptions;

public class CopiesAvailableException : LibraryManagementException
{
    public CopiesAvailableException(string message = "Sách hiện có sẵn trên kệ, vui lòng mượn trực tiếp (không cần đặt trước).") : base(message) { }
}
