namespace LibraryManagement.Domain.Exceptions;

public class LibraryManagementException : Exception
{
    public LibraryManagementException(string message) : base(message) { }
}
