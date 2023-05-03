using System.Data.SQLite;

if (args.Length != 1)
{
    Console.WriteLine("Usage: dbcli <connection string>");
    return 1;
}
var connectionString = args[0];

var parameters = connectionString.Split(';').Select(parameterString =>
{
    var parts = parameterString.Split('=');
    return new { Name = parts[0], Value = parts[1] };
}).ToArray();
var dbPath = parameters.First(parameter => parameter.Name == "Data Source").Value;
if (File.Exists(dbPath)) { File.Delete(dbPath); }

await using (var connection = new SQLiteConnection(connectionString))
{
    await Schema.Create(connection);
}

Console.WriteLine("Done");
return 0;
