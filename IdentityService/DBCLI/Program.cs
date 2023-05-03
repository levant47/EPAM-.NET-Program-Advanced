using System.Data.SQLite;

if (args.Length != 1)
{
    Console.WriteLine("Usage: dbcli <connection string>");
    return 1;
}
var connectionString = args[0];

await using (var connection = new SQLiteConnection(connectionString))
{
    await Schema.Create(connection);
}

Console.WriteLine("Done");
return 0;
