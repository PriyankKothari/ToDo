using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using ToDo.Persistent.DbEnums;

namespace ToDo.WebApi.Constraints
{
    public class ItemStatusEnumConstraint : IRouteConstraint
    {
        /// <summary>
        /// Determines whether the URL parameter contains a valid enum value for InvoiceStatuses constraint.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="route"></param>
        /// <param name="parameterName"></param>
        /// <param name="values"></param>
        /// <param name="routeDirection"></param>
        /// <returns></returns>
        public bool Match(HttpContext httpContext, IRouter route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return Enum.TryParse(values[parameterName]?.ToString(), true, out ToDoStatuses result);
        }
    }
}