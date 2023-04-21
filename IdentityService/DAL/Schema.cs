public static class Schema
{
    public static Task Create(SQLiteConnection connection) => connection.ExecuteAsync("""
        CREATE TABLE Roles (
            Id INTEGER PRIMARY KEY,
            Name TEXT NOT NULL UNIQUE
        );

        CREATE TABLE Permissions (
            RoleId INTEGER,
            Permission INTEGER,
            PRIMARY KEY (RoleId, Permission),
            FOREIGN KEY (RoleId) REFERENCES Roles (Id)
        );

        CREATE TABLE Users (
            Id INTEGER PRIMARY KEY,
            Username TEXT NOT NULL UNIQUE,
            Password TEXT NOT NULL,
            RoleId INTEGER NOT NULL,
            FOREIGN KEY (RoleId) REFERENCES Roles (Id)
        );
    """);
}
