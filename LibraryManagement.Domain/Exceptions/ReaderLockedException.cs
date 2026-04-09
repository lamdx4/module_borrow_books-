namespace LibraryManagement.Domain.Exceptions;

public class ReaderLockedException : LibraryManagementException
{
    public ReaderLockedException(string message = "Tài khoản độc giả đang bị khóa.") : base(message) { }
}
