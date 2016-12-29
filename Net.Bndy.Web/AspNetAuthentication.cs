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
        public static bool Enabled { private set; get; }
        public static void Enable(IAppBuilder app, string loginUrl = null)
        {
            app.UseCookieAuthentication(new Microsoft.Owin.Security.Cookies.CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(loginUrl)
            });
            Enabled = true;
        }
    }
    public class AspNetAuthentication<TUser, TRole>
        where TUser : IdentityUser
        where TRole : IdentityRole, new()
    {

        public HttpContextBase HttpContext { private set; get; }
        public IdentityDbContext DbContext { private set; get; }

        public UserStore<TUser> UserStore
        {
            get
            {
                return DbContext == null ? new UserStore<TUser>() : new UserStore<TUser>(DbContext);
            }
        }
        public UserManager<TUser> UserManager
        {
            get
            {
                return new UserManager<TUser>(UserStore);
            }
        }
        public RoleStore<TRole> RoleStore
        {
            get
            {
                return DbContext == null ? new RoleStore<TRole>() : new RoleStore<TRole>(DbContext);
            }
        }
        public RoleManager<TRole> RoleManager
        {
            get
            {
                return new RoleManager<TRole>(RoleStore);
            }
        }


        public AspNetAuthentication(HttpContextBase httpContext, IdentityDbContext dbContext = null)
        {
            if (!AspNetAuthentication.Enabled)
            {
                throw new Exception($"Please use `AspNetAuthentication.Enable` method to enable AspNetAuthentication.");
            }

            HttpContext = httpContext;
            DbContext = dbContext;
        }

        public void CreateRole(TRole role)
        {
            var createdResult = RoleManager.Create(role);
            if (!createdResult.Succeeded)
            {
                throw new Exception(string.Join(",", createdResult.Errors));
            }
        }

        public void CreateUser(TUser user, string password)
        {
            var createdResult = UserManager.Create(user, password);

            if (!createdResult.Succeeded)
            {
                throw new Exception(string.Join(",", createdResult.Errors));
            }
        }

        public IQueryable<TUser> GetUsers()
        {
            return UserStore.Users;
        }

        public ClaimsIdentity SignIn(string username, string password)
        {
            var u = UserManager.Find(username, password);
            if (u != null)
            {
                var authManager = HttpContext.GetOwinContext().Authentication;
                var identity = UserManager.CreateIdentity(u, DefaultAuthenticationTypes.ApplicationCookie);

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
