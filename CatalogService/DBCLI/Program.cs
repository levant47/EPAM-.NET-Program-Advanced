using Dapper;
using MySqlConnector;

if (args.Length != 1)
{
    Console.WriteLine("Usage: dbcli <connection string>");
    return 1;
}
var connectionString = args[0];
var connectionStringParameters = connectionString.Split(';').Select(parameterString =>
{
    var words = parameterString.Split('=');
    return new { Name = words[0], Value = words[1] };
}).ToArray();
var databaseName = connectionStringParameters.First(parameter => parameter.Name == "database").Value;

var sysConnectionString = string.Join(";",
    connectionStringParameters.Select(parameter => parameter.Name == "database" ? "database=sys" : $"{parameter.Name}={parameter.Value}")
);
await using (var sysConnection = new MySqlConnection(sysConnectionString))
{
    await sysConnection.ExecuteAsync(@$"
        DROP DATABASE IF EXISTS {databaseName};
        CREATE DATABASE {databaseName};
    ");
}

await using (var targetDbConnection = new MySqlConnection(connectionString))
{
    await Schema.Create(targetDbConnection);
}

Console.WriteLine("Done");
return 0;
