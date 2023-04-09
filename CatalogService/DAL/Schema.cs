public static class Schema
{
    public static Task Create(MySqlConnection connection) => connection.ExecuteAsync(@$"
        CREATE TABLE Categories (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR({CategoryEntity.NameMaxLength}) NOT NULL,
            ImageUrl TEXT,
            ParentCategoryId INT,
            FOREIGN KEY (ParentCategoryId)
                REFERENCES Categories (Id)
                ON UPDATE RESTRICT
                ON DELETE RESTRICT
        );

        CREATE TABLE Items (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR({ItemEntity.NameMaxLength}) NOT NULL,
            Description TEXT,
            ImageUrl TEXT,
            CategoryId INT NOT NULL,
            Price DECIMAL,
            Amount INT NOT NULL,
            FOREIGN KEY (CategoryId)
                REFERENCES Categories (Id)
                ON UPDATE RESTRICT
                ON DELETE CASCADE
        );
    ");
}
