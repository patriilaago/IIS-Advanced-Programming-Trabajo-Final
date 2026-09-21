using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Configuration;
using Models;

namespace DAL
{
    /// <summary>
    /// Class for managing user authentication and roles in the database.
    /// </summary>
    public class UserManagement
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Ticket2HelpConnection"].ConnectionString;
        /// <summary>
        /// Retrieves a user from the database based on username and password.
        /// </summary>
        /// <param name="username">Name of the user</param>
        /// <param name="password">Password of the user</param>
        /// <returns>Return the user that haves the username and the password and in the case that not have a user with the user and password returns null.</returns>
        public User? GetUser(string username, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Username, Role FROM Users WHERE Username = @username AND Password = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return new User
                    {
                        Username = reader["Username"].ToString(),
                        Role = reader["Role"].ToString()
                    };
                }
                return null;
            }
        }
    }           
}
