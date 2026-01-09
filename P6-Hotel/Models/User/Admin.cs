namespace P6_Hotel;

public record Admin (string Username, string Password) : AUser(Username, Password, UserRole.Admin);
