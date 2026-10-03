using Gadema.Api.CoreServices.Interfaces;
using Gadema.Api.Services.Access;
using Gadema.Core.Interfaces;

namespace Gadema.Api.CoreServices;



public class CoreServicesProvider : ICoreServicesProvider
{
    private IAuditService _auditService;
    private IMetadataService _metadataService;
    private IPermissionEngine _permissionEngine;

    private IUserContext? _userContext;

    public CoreServicesProvider(IAuditService auditService,
                                IMetadataService metadataService,
                                IPermissionEngine permissionEngine,
                                IUserContext? userContext)
    {
        _auditService = auditService;
        _metadataService = metadataService;
        _permissionEngine = permissionEngine;
        _userContext = userContext;
    }

    public IAuditService AuditService { get => _auditService; }
    public IMetadataService MetadataService { get => _metadataService; }
    public IPermissionEngine PermissionEngine { get => _permissionEngine; }
    public IUserContext? UserContext { get => _userContext; set => _userContext = value; }
}