public class ApiVersionAttribute : Attribute
{
    public readonly int VersionIntroduced;
    public readonly int? VersionDeprecated;

    public ApiVersionAttribute(int versionIntroduced) => (VersionIntroduced, VersionDeprecated) = (versionIntroduced, null);
    public ApiVersionAttribute(int versionIntroduced, int deprecatedInVersion) =>
        (VersionIntroduced, VersionDeprecated) = (versionIntroduced, deprecatedInVersion);

    public bool Match(int version) => version >= VersionIntroduced && (VersionDeprecated == null || version < VersionDeprecated);

    public static ApiVersionAttribute? GetFrom(ActionDescriptor actionDescriptor)
    {
        var methodInfo = (actionDescriptor as ControllerActionDescriptor)?.MethodInfo;
        return methodInfo?.GetCustomAttribute<ApiVersionAttribute>()
            ?? methodInfo?.DeclaringType?.GetCustomAttribute<ApiVersionAttribute>();
    }
}
