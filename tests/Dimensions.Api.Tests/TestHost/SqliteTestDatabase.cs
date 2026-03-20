namespace Dimensions.Api.Tests.TestHost;

internal sealed class SqliteTestDatabase : IDisposable
{
    private readonly string _databasePath;

    public SqliteTestDatabase()
    {
        _databasePath = Path.Combine(
            Path.GetTempPath(),
            "Dimensions.Tests",
            $"{Guid.NewGuid():N}.db");

        Directory.CreateDirectory(Path.GetDirectoryName(_databasePath)!);
    }

    public string ConnectionString => $"Data Source={_databasePath};Cache=Shared;Foreign Keys=True";

    public string DatabasePath => _databasePath;

    public void Dispose()
    {
        try
        {
            if (File.Exists(_databasePath))
            {
                File.Delete(_databasePath);
            }
        }
        catch
        {
            // 測試結束後若檔案仍被占用，不影響測試結果。
        }
    }
}
