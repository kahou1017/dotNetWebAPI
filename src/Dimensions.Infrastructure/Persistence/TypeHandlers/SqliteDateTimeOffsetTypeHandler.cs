using System.Data;
using Dapper;

namespace Dimensions.Infrastructure.Persistence.TypeHandlers;

internal sealed class SqliteDateTimeOffsetTypeHandler : SqlMapper.TypeHandler<DateTimeOffset>
{
    public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
    {
        parameter.Value = value.UtcDateTime.ToString("O");
        parameter.DbType = DbType.String;
    }

    public override DateTimeOffset Parse(object value)
    {
        return value switch
        {
            DateTimeOffset dto => dto,
            DateTime dt => new DateTimeOffset(dt, TimeSpan.Zero),
            string s when DateTimeOffset.TryParse(s, out var parsed) => parsed,
            _ => throw new DataException($"Unable to parse DateTimeOffset from value '{value}'.")
        };
    }
}
