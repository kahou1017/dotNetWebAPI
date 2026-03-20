namespace Dimensions.Infrastructure.Options;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "Sqlite";

    public string ConnectionStringName { get; set; } = "DefaultConnection";

    public int CommandTimeoutSeconds { get; set; } = 30;
}
