using System.Data;
using Dapper;

namespace Dimensions.Infrastructure.Persistence.TypeHandlers;

internal sealed class NullableSqliteDateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset?>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset? value)
    {
        parameter.Value = value?.UtcDateTime.ToString("O") ?? (object)DBNull.Value;
        parameter.DbType = DbType.String;
    }

    public override DateTimeOffset? Parse(object value)
    {
        if (value is null || value is DBNull)
        {
            return null;
        }

        return value switch
        {
            DateTimeOffset dto => dto,
            DateTime dt => new DateTimeOffset(dt, TimeSpan.Zero),
            string s when string.IsNullOrWhiteSpace(s) => null,
            string s when DateTimeOffset.TryParse(s, out var parsed) => parsed,
            _ => throw new DataException($"Unable to parse nullable DateTimeOffset from value '{value}'.")
        };
    }
}
