using Dimensions.Application.Interfaces;
using Dimensions.Application.Models;
using Dimensions.Contracts.Common;
using Dimensions.Contracts.Device;

namespace Dimensions.Infrastructure.Persistence.Repositories;

public sealed class DeviceRepository(DapperSqlExecutor sqlExecutor) : IDeviceRepository
{
    public Task<DeviceListItemResponse?> GetDeviceByIdAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                UserId,
                DeviceId,
                DeviceName,
                DeviceType,
                IsEnabled
            FROM Devices
            WHERE DeviceId = @DeviceId
            LIMIT 1;
            """;

        return sqlExecutor.QuerySingleOrDefaultAsync<DeviceListItemResponse>(
            sql,
            new { DeviceId = deviceId },
            cancellationToken: cancellationToken);
    }

    public async Task<PagedResult<DeviceListItemResponse>> GetDeviceListAsync(DeviceListRequest request, CancellationToken cancellationToken = default)
    {
        var pageNo = request.PageNo <= 0 ? 1 : request.PageNo;
        var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;
        var offset = (pageNo - 1) * pageSize;

        const string countSql = """
            SELECT COUNT(1)
            FROM Devices
            WHERE (@UserId IS NULL OR UserId = @UserId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@IsEnabled IS NULL OR IsEnabled = @IsEnabled);
            """;

        const string listSql = """
            SELECT
                UserId,
                DeviceId,
                DeviceName,
                DeviceType,
                IsEnabled
            FROM Devices
            WHERE (@UserId IS NULL OR UserId = @UserId)
              AND (@DeviceId IS NULL OR DeviceId = @DeviceId)
              AND (@IsEnabled IS NULL OR IsEnabled = @IsEnabled)
            ORDER BY UpdatedAt DESC, DeviceId
            LIMIT @PageSize OFFSET @Offset;
            """;

        var parameters = new
        {
            request.UserId,
            request.DeviceId,
            request.IsEnabled,
            PageSize = pageSize,
            Offset = offset
        };

        var totalCount = await sqlExecutor.QuerySingleOrDefaultAsync<int>(countSql, parameters, cancellationToken: cancellationToken);
        var items = await sqlExecutor.QueryAsync<DeviceListItemResponse>(listSql, parameters, cancellationToken: cancellationToken);

        return new PagedResult<DeviceListItemResponse>
        {
            PageNo = pageNo,
            PageSize = pageSize,
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<DeviceListItemResponse> UpsertDeviceAsync(DeviceWriteModel device, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO Devices (DeviceId, UserId, DeviceName, DeviceType, IsEnabled, Remark, CreatedAt, UpdatedAt)
            VALUES (@DeviceId, @UserId, @DeviceName, @DeviceType, @IsEnabled, @Remark, @UpdatedAt, @UpdatedAt)
            ON CONFLICT(DeviceId) DO UPDATE SET
                UserId = excluded.UserId,
                DeviceName = excluded.DeviceName,
                DeviceType = excluded.DeviceType,
                IsEnabled = excluded.IsEnabled,
                Remark = excluded.Remark,
                UpdatedAt = excluded.UpdatedAt;
            """;

        await sqlExecutor.ExecuteAsync(sql, device, cancellationToken: cancellationToken);

        return new DeviceListItemResponse
        {
            UserId = device.UserId,
            DeviceId = device.DeviceId,
            DeviceName = device.DeviceName,
            DeviceType = device.DeviceType,
            IsEnabled = device.IsEnabled
        };
    }

    public async Task DisableDeviceAsync(string deviceId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE Devices
            SET IsEnabled = 0,
                UpdatedAt = @UpdatedAt
            WHERE DeviceId = @DeviceId;
            """;

        await sqlExecutor.ExecuteAsync(
            sql,
            new { DeviceId = deviceId, UpdatedAt = DateTimeOffset.UtcNow },
            cancellationToken: cancellationToken);
    }

    public async Task<bool> HasEnabledDeviceAsync(string userId, string deviceId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COUNT(1)
            FROM Devices
            WHERE UserId = @UserId
              AND DeviceId = @DeviceId
              AND IsEnabled = 1;
            """;

        var count = await sqlExecutor.QuerySingleOrDefaultAsync<int>(
            sql,
            new { UserId = userId, DeviceId = deviceId },
            cancellationToken: cancellationToken);

        return count > 0;
    }
}
