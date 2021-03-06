﻿//// The MIT License (MIT)
//// 
//// Copyright (c) 2014 Tesseris Pro LLC
////     
//// Permission is hereby granted, free of charge, to any person obtaining a copy
//// of this software and associated documentation files (the "Software"), to deal
//// in the Software without restriction, including without limitation the rights
//// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//// copies of the Software, and to permit persons to whom the Software is
//// furnished to do so, subject to the following conditions:
////     
//// The above copyright notice and this permission notice shall be included in
//// all copies or substantial portions of the Software.
////     
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//// THE SOFTWARE.

using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Tesseris.Web.SimpleSecurity
{
    /// <summary>
    /// Simple form based security provider
    /// </summary>
    /// <remarks>
    /// SimpleSecurityProvider automatically will create table [user] in default schema. 
    /// Provider requires only one table but supports roles. Roles are stored in role column as coma separated list.
    /// SimpleSecurityProvider adds cookies for authentication ticket and for user (auth.user). User cookies can be 
    /// used for JS client side code to determine user name.
    /// </remarks>
    public class SimpleSecurityProvider : ISecurityProvider
    {
        /// <summary>
        /// The connection string name
        /// </summary>
        private string connectionString;

        /// <summary>
        /// The ticket cookie name
        /// </summary>
        private static readonly string ticketCookieName = "auth.ticket";

        /// <summary>
        /// The user cookie name
        /// </summary>
        private static readonly string userCookieName = "auth.user";

        /// <summary>
        /// 
        /// </summary>
        private static readonly string userIdCookieName = "auth.id";

        /// <summary>
        /// The current provider can be replaced for tests or other purposes
        /// </summary>
        private static SimpleSecurityProvider current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleSecurityProvider"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="adminPassword">Default admin password.</param>
        /// <remarks>
        /// SimpleSecurityProvider automatically will create table [user] in default schema in case if this table is not existing.
        /// Provider requires only one table but supports roles. Roles are stored in role column as coma separated list.
        /// During initialization provider also creates admin user with specified password. Default password is "pass2app". />
        /// </remarks>
        public SimpleSecurityProvider(string connectionStringName)
        {
            connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        /// <summary>
        /// Gets or sets the current provider.
        /// </summary>
        /// <value>
        /// The current provider.
        /// </value>
        public static SimpleSecurityProvider Current
        {
            get
            {
                if (current == null)
                {
                    current = SecurityContext.Provider as SimpleSecurityProvider;
                }

                return current;
            }

            set
            {
                current = value;
            }
        }

        /// <summary>
        /// Gets the current user principal from HTTP request.
        /// </summary>
        /// <returns>
        /// Current user principal with all roles. If user is not logged in the method will return principal with not authenticated (IsAuthenticated = false) identity. 
        /// </returns>
        public GenericPrincipal GetPrincipalFromRequest()
        {
            VerifyInitialized();
            try
            {
                var ticketCoockie = HttpContext.Current.Request.Cookies[ticketCookieName];
                if (ticketCoockie != null && !string.IsNullOrEmpty(ticketCoockie.Value))
                {
                    var ticket = FormsAuthentication.Decrypt(ticketCoockie.Value);
                    if (!ticket.Expired)
                    {
                        var roles = GetUserRoles(ticket.Name);
                        if (roles != null)
                        {
                            // Update expiration period of cookies
                            if (ticket.Expiration != DateTime.MinValue)
                            {
                                var authCookie = BuildAuthCookie(ticket.Name, false, (int)(ticket.Expiration - ticket.IssueDate).TotalMinutes);
                                HttpContext.Current.Response.AppendCookie(authCookie);
                            }

                            return BuildPrincipal(ticket.Name, roles);
                        }
                    }
                    else
                    {
                        // Remove cookies
                        HttpContext.Current.Response.SetCookie(BuildAuthCookie());
                        HttpContext.Current.Response.SetCookie(new HttpCookie(userCookieName, "")
                        {
                            Expires = DateTime.MinValue,
                            HttpOnly = false,
                            Shareable = false
                        });
                    }
                }
            }
            catch (CryptographicException ex)
            {
                return new GenericPrincipal(new EmptyIdentity(), new string[] { });
            }

            return new GenericPrincipal(new EmptyIdentity(), new string[] { });
        }

        /// <summary>
        /// Logins the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="keep">if set to <c>true</c> will keep authentication cookies between browser sessions.</param>
        /// <param name="timeout">The expiration timeout in minutes.</param>
        /// <returns><c>true</c> if user logged in successfully (found in DB)</returns>
        public bool Login(string user, string password, bool keep = false, int timeout = 60)
        {
            VerifyInitialized();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(password))
            {
                return false;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                var foundUser = connection
                    .Query(
                        "select id, emailaddress, password, displayname role from [user] where emailaddress = @user and password = @password",
                        new { user = user.ToLower(), password = GetHash(password) })
                    .SingleOrDefault();
                if (foundUser == null)
                {
                    return false;
                }

                var userId = foundUser.id;
                //Create user id cookies
                HttpContext.Current.Response.SetCookie(new HttpCookie(userIdCookieName, Convert.ToString(userId))
                {
                    Expires = DateTime.Now.AddDays(1),
                    HttpOnly = false,
                    Shareable = false
                });

                var authCookie = BuildAuthCookie(user, keep, timeout);
                HttpContext.Current.Response.AppendCookie(authCookie);

                HttpContext.Current.Response.AppendCookie(
                    new HttpCookie(userCookieName, user)
                    {
                        Expires = authCookie.Expires,
                        HttpOnly = false,
                        Shareable = false
                    });

                var role = GetUserRoles(user);//(string)(foundUser.role ?? string.Empty);

                HttpContext.Current.User = BuildPrincipal(user, role);
            }

            return true;
        }

        /// <summary>
        /// Logout the user.
        /// </summary>
        public void Logout()
        {
            HttpContext.Current.Response.SetCookie(BuildAuthCookie());
            HttpContext.Current.Response.SetCookie(new HttpCookie(userCookieName, "")
            {
                Expires = DateTime.MinValue,
                HttpOnly = false,
                Shareable = false
            });

            HttpContext.Current.Response.SetCookie(new HttpCookie(userIdCookieName, "")
            {
                Expires = DateTime.MinValue,
                HttpOnly = false,
                Shareable = false
            });

            HttpContext.Current.User = new GenericPrincipal(new EmptyIdentity(), new string[] { });
        }

        /// <summary>
        /// Builds the authentication cookie.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="keep">if set to <c>true</c> will keep authentication cookies between browser sessions.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>Cookie with encoded ticket</returns>
        private static HttpCookie BuildAuthCookie(string user, bool keep, int timeout)
        {
            var ticket = new FormsAuthenticationTicket(user, keep, timeout);
            var authCookie = new HttpCookie(ticketCookieName, FormsAuthentication.Encrypt(ticket));
            authCookie.Shareable = false;
            authCookie.Expires = keep ? ticket.Expiration : DateTime.MinValue;
            authCookie.HttpOnly = true;
            return authCookie;
        }

        /// <summary>
        /// Builds empty authentication cookie.
        /// </summary>
        /// <returns>Empty cookie</returns>
        private static HttpCookie BuildAuthCookie()
        {
            var ticket = new FormsAuthenticationTicket("", false, 0);
            var authCookie = new HttpCookie(ticketCookieName, FormsAuthentication.Encrypt(ticket));
            authCookie.Shareable = false;
            authCookie.Expires = DateTime.MinValue;
            authCookie.HttpOnly = true;
            return authCookie;
        }

        /// <summary>
        /// Registers the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="roles">The roles.</param>
        public bool Register(string user, string password, string roles, string createdBy, bool isActive)
        {
            VerifyInitialized();

            using (var connection = new SqlConnection(connectionString))
            {
                if (connection.Query("select * from [user] where emailaddress = @user", new { user = user }).Any())
                {
                    // User already exists
                    return false;
                }
                var displayName = user.Split('@')[0];
                connection.Execute(
                    "insert into [user](emailaddress, password, role, displayname, createdby, createddate, isactive) values(@user,@password,@role,@displayName,@createdBy, @createdDate, @isActive)",
                    new { user = user, password = GetHash(password), role = roles ?? "1", displayName = displayName ?? string.Empty, createdBy = createdBy ?? "1", createdDate = DateTime.Now, isActive = isActive });
            }

            return true;
        }

        /// <summary>
        /// Unregisters the specified user.
        /// </summary>
        /// <param name="user">The user name.</param>
        public void Unregister(string user)
        {
            VerifyInitialized();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(
                    "delete [user] where emailaddress = @user",
                    new { user = user });
            }
        }

        /// <summary>
        /// Returns list of registered users
        /// </summary>
        /// <returns>List of GnericPrincipals with GenericIdentity</returns>
        public IEnumerable<GenericPrincipal> GetRegisteredUsers()
        {
            VerifyInitialized();

            using (var connection = new SqlConnection(connectionString))
            {
                var users = connection.Query("select emailaddress, role from [user]");
                var userRole = users.Select(x => x.role);
                var roles = connection.Query("select name from [role] where id = @id",
                    new { id = userRole }).FirstOrDefault();
                return users.Select(x => new GenericPrincipal(new GenericIdentity(x.name), SplitRoles(roles.name))).ToList();
            }
        }

        /// <summary>
        /// Updates user roles
        /// </summary>
        /// <param name="user">User to update</param>
        /// <param name="password">New roles</param>
        public void SetUserRoles(string user, string roles)
        {
            VerifyInitialized();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Execute(
                    "update [user] set role=@role where emailaddress = @user",
                    new { user = user, role = roles });
            }
        }

        /// <summary>
        /// Updates user password
        /// </summary>
        /// <param name="user">User to update</param>
        /// <param name="password">New password</param>
        public bool SetUserPassword(string user, string password, string newPassword)
        {
            VerifyInitialized();

            using (var connection = new SqlConnection(connectionString))
            {
                var foundUser = connection
                    .Query(
                        "select * from [user] where emailaddress = @user and password = @password",
                        new { user = user.ToLower(), password = GetHash(password) })
                    .SingleOrDefault();
                if (foundUser == null)
                {
                    return false;
                }

                connection.Execute(
                    "update [user] set password=@password where emailaddress = @user",
                    new { user = user, password = GetHash(newPassword) });

                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int ChangePassword(string email, string password)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var foundUser = connection
                        .Query(
                            "select * from [user] where emailaddress = @user",
                            new { user = email.ToLower() })
                        .SingleOrDefault();
                    if (foundUser == null)
                    {
                        return 1;
                    }

                    connection.Execute(
                        "update [user] set password=@password where emailaddress = @user",
                        new { user = email, password = GetHash(password) });

                    return 0;
                }
            }
            catch(Exception ex)
            {
                return 2;
            }
        }

        /// <summary>
        /// Builds the principal
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="roles">The roles.</param>
        /// <returns>Principal with specified name and roles</returns>
        private static GenericPrincipal BuildPrincipal(string name, string[] roles)
        {
            return new GenericPrincipal(new GenericIdentity(name), roles);
        }

        /// <summary>
        /// Gets the user roles from DB for specified user.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Array of user roles or null if user does not exist.</returns>
        private string[] GetUserRoles(string name)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var foundUser = connection
                    .Query(
                        "select id, emailaddress, password, role from [user] where emailaddress = @user",
                        new { user = name.ToLower() })
                    .SingleOrDefault();

                if (foundUser != null)
                {
                    var roles = connection
                    .Query(
                        "select name from [role] where id = @id",
                        new { id = foundUser.role })
                    .SingleOrDefault();//((string)foundUser.roles) ?? string.Empty;
                    return SplitRoles(roles.name);
                }
            }

            return null;
        }

        /// <summary>
        /// Splits roles
        /// </summary>
        /// <param name="roles">Coma separated string with roles</param>
        /// <returns>Array of roles</returns>
        private static string[] SplitRoles(string roles)
        {
            return roles.Split(',').Select(x => x.Trim()).ToArray();
        }

        /// <summary>
        /// Verifies that provider is initialized.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">SimpleSecurityProvider is not initialized.</exception>
        private void VerifyInitialized()
        {
            if (connectionString == null)
            {
                throw new InvalidOperationException("SimpleSecurityProvider is not initialized.");
            }
        }

        /// <summary>
        /// Gets password hash.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <returns>Password hash code</returns>
        private string GetHash(string password)
        {
            return string.Join("", MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X")).ToArray());
        }

    }
}
