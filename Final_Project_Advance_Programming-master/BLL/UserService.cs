using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UserService
    {
        private UserManagement userManag = new UserManagement();

        /// <summary>
        /// Validates the login credentials of a user.
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Return the user that haves the username and the password and in the case that not have a user with the user and password returns null.</returns>
        public User? ValidateLogin(string username, string password)
        {
            return userManag.GetUser(username, password);
        }
    }
}


