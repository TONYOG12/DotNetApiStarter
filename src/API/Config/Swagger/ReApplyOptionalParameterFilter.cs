using System.Text.RegularExpressions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Config.Swagger;

public partial class ReApplyOptionalParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // Continue to apply optional settings to path parameters
        var routeTemplate = context.ApiDescription.RelativePath;
        var matches = OptionalParamRegex.Matches(routeTemplate);

        foreach (Match match in matches)
        {
            var paramName = match.Groups["paramName"].Value;

            // Only apply changes to path parameters
            var parameter = operation.Parameters.FirstOrDefault(p => p.Name == paramName && p.In == ParameterLocation.Path);
            if (parameter != null)
            {
                parameter.Required = false;
                parameter.AllowEmptyValue = true;
                parameter.Description = "Tick checkbox 'Send empty value' if you need to send an empty value";
                parameter.Schema.Default = new OpenApiString(string.Empty);
                parameter.Schema.Nullable = true;
            }
        }
    }
    
    private static readonly Regex OptionalParamRegex = MyRegex();

    [GeneratedRegex(@"\{(?<paramName>\w+)\?\}", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}