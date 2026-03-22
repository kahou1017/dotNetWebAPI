using Dimensions.Admin.Web.Models;

namespace Dimensions.Admin.Web.Services;

public interface IAdminSessionAccessor
{
    AdminSessionState? GetCurrent();

    void SetCurrent(AdminSessionState session);

    void Clear();
}
