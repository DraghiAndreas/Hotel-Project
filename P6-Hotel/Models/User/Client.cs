namespace P6_Hotel;

public record Client (string Username, string Password) : AUser(Username, Password, UserRole.Client);
