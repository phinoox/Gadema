using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Interfaces;

public interface ICoreServicesProvider
{
    IAuditService AuditService { get; }
    IMetadataService MetadataService { get; }
    IPermissionEngine PermissionEngine { get; }
    IUserContext? UserContext { get; set; }
}