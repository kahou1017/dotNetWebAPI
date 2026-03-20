using System.Data;

namespace Dimensions.Application.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
