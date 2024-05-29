using Cassandra;
using ISession = Cassandra.ISession;

public class CassandraDAO
{
    private readonly Cluster _cluster;
    private readonly ISession _session;

    public CassandraDAO()
    {
        var contactPoints = new[] { "localhost" };
        var port = 9042;
        var keyspace = "all_one_db_cass_for_mssg";

        var cluster = Cluster.Builder()
            .AddContactPoints(contactPoints)
            .WithPort(port)
            .WithCredentials("user_alldmin", "passwade")
            .Build();

        _session = cluster.Connect(keyspace);
    }

    public RowSet ExecuteQuery(string cql)
    {
        return _session.Execute(cql);
    }

    public void Dispose()
    {
        _session.Dispose();
        _cluster.Dispose();
    }
    public async Task<List<string>> GetTableNamesAsync()
{
    var tableNames = new List<string>();

    var resultSet = await _session.ExecuteAsync(new SimpleStatement("DESCRIBE TABLES;"));

    foreach (var row in resultSet)
    {
        var tableName = row.GetValue<string>(2);
        tableNames.Add(tableName);
    }

    return tableNames;
}


}
