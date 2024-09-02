namespace APP.Utils;

public static class AppConstants
{
    public const string MyAllowedSpecificOrigins = "_myAllowedSpecificOrigins";

    public const int ErrorNumberForMysqlUniqueConstraintViolation = 1062;
    public const int ErrorNumberForForeignKeyConstraint = 1452;

    public const string AppName = "App-Name";
    
    //Tenant Ids
    public const string DefaultTenantId = "StartUp";
    
    //Domains
    public const string DomainType = "A";
    public const string SShPort = "22";
    
    //Permission
    public const string Permission = "permission";
    
    //Mapper Context
    public const string ModelType = "ModelType";
    public const string IsAdmin = "IsAdmin";
    public const string UserId = "UserId";
}