// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Integrador.Views.MisDatos
{
    public static class ManageNavPages
    {
        public static string Index
        {
            get
            {
                // Create an instance of HttpContextAccessor
                var httpContextAccessor = new HttpContextAccessor();

                // Access the HttpContext property using the instance
                var httpContext = httpContextAccessor.HttpContext;
                // Obtener el User Manager
                var userManager = (UserManager<IdentityUser>)httpContext.RequestServices.GetService(typeof(UserManager<IdentityUser>));

                // Obtener el usuario actual

                var user = userManager.GetUserAsync(httpContextAccessor.HttpContext.User).Result;

                // Comprobar los roles del usuario
                if (userManager.IsInRoleAsync(user, "Cliente").Result)
                {
                    // Cliente
                    return "EditCli";
                }
                else
                {
                    // Proveedor
                    return "EditPro";
                }
            }
        }

        public static string ChangePassword => "ChangePassword";

        public static string DeletePersonalData => "DeletePersonalData";

        public static string PersonalData => "PersonalData";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

        public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

        public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
