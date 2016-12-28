// ==========================================================================
// Copyright (c) 2016 http://www.bndy.net.
// Created by Bndy at 12/28/2016 08:30:17 PM
// --------------------------------------------------------------------------
// AspNet Identify
// ==========================================================================

using System;
using System.Security.Claims;
using System.Web;
using System.Linq;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

namespace Net.Bndy.Web
{
    public class AspNetAuthentication
    {
        private static bool _enable;
        public HttpContextBase HttpContext { private set; get; }

        public static void Enable(IAppBuilder app, string loginUrl = null)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(loginUrl)
            });
            _enable = true;
        }

        public AspNetAuthentication(HttpContextBase httpContext)
        {
            HttpContext = httpContext;

            if (!_enable)
            {
                throw new Exception($"Please use {nameof(AspNetAuthentication)}.{ nameof(AspNetAuthentication.Enable)} method to enable {nameof(AspNetAuthentication)}.");
            }
        }

        public void CreateUser(IdentityUser user, string password)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            user.PasswordHash = userManager.PasswordHasher.HashPassword(password);

            var createdResult = userManager.Create(user);

            if (!createdResult.Succeeded)
            {
                throw new Exception(string.Join(",", createdResult.Errors));
            }
        }

        public IQueryable<IdentityUser> GetUsers()
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);
            return userStore.Users;
        }

        public ClaimsIdentity SignIn(string username, string password)
        {
            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            var u = userManager.Find(username, password);
            if (u != null)
            {
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = userManager.CreateIdentity(u, DefaultAuthenticationTypes.ApplicationCookie);

                authManager.SignIn(new Microsoft.Owin.Security.AuthenticationProperties
                {
                    IsPersistent = false
                }, identity);

                return identity;
            }
            return null;
        }

        public void SignOut()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();

        }
    }
}
