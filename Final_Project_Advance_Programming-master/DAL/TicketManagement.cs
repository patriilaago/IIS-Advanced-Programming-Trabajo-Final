using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Models;

namespace DAL;
    /// <summary>
    /// Class for the creation of tickets using the Factory pattern.
    /// </summary>
    //crear tickets con Factory pattern
    public static class TicketFactory{
        /// <summary>
        /// Creates a ticket based on the type provided using the Factory pattern.
        /// </summary>
        /// <param name="type">Type it's a string that contains the type of the ticket to create.</param>
        /// <returns>Return the ticket that creates</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Ticket CreateTicket(string type)
        {
            return type switch
            {
                "Hardware" => new HardwareTicket { TicketType = "Hardware" },
                "Software" => new SoftwareTicket { TicketType = "Software" },
                _ => throw new ArgumentException("Tipo inválido de ticket"),
            };
        }
    }

    /// <summary>
    /// Class for managing tickets in the database.
    /// </summary>
    public class TicketManagement {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["Ticket2HelpConnection"].ConnectionString;

        public List<Ticket> GetTicketsByUser(string username)
        {
            var tickets = new List<Ticket>();
            string query = @"
            SELECT * 
            FROM Tickets 
            WHERE Username = @username";


            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string type = reader["TicketType"].ToString();

                    try
                    {
                        Ticket ticket = TicketFactory.CreateTicket(type);

                        // Propiedades comunes
                        ticket.Id = Convert.ToInt32(reader["TicketId"]);
                        ticket.Username = reader["Username"].ToString();
                        ticket.TicketType = type;
                        ticket.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
                        ticket.Status = reader["Status"].ToString();
                        ticket.ServiceStatus = reader["ServiceStatus"].ToString();
                        ticket.Response = reader["Response"].ToString();

                    // Propiedades específicas
                    if (ticket is HardwareTicket hw)
                        {
                            hw.Equipment = reader["Equipment"]?.ToString();
                            hw.Malfunction = reader["Malfunction"]?.ToString();
                        }
                        else if (ticket is SoftwareTicket sw)
                        {
                            sw.Software = reader["Software"]?.ToString();
                            sw.Description = reader["Description"]?.ToString();
                        }

                        tickets.Add(ticket);
                    }
                    catch (ArgumentException)
                    {
                        // Si el tipo es inválido, lo ignoramos
                        continue;
                    }
                }
            }
            return tickets;
        }

        /// <summary>
        /// Inserts a ticket into the database.
        /// </summary>
        /// <param name="ticket">It'a the (harware of software) ticket to insert.</param>
        /// <returns>The number of the tickets inserted in the databse</returns>
        public bool InsertTicket(Ticket ticket)
        {
            string query = @"INSERT INTO Tickets 
            (Username, TicketType, CreatedAt, Status, ServiceStatus, Description, Equipment, Malfunction, Software) 
            VALUES 
            (@username, @ticketType, @createdAt, @status, @serviceStatus, @description, @equipment, @malfunction, @software)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", ticket.Username);
                cmd.Parameters.AddWithValue("@ticketType", ticket.TicketType);
                cmd.Parameters.AddWithValue("@createdAt", ticket.CreatedAt);
                cmd.Parameters.AddWithValue("@status", ticket.Status);
                cmd.Parameters.AddWithValue("@serviceStatus", ticket.ServiceStatus ?? "");

                // Inicializar campos específicos
                if (ticket is HardwareTicket hw)
                {
                    cmd.Parameters.AddWithValue("@description", DBNull.Value);
                    cmd.Parameters.AddWithValue("@equipment", hw.Equipment ?? "");
                    cmd.Parameters.AddWithValue("@malfunction", hw.Malfunction ?? "");
                    cmd.Parameters.AddWithValue("@software", DBNull.Value);
                }
                else if (ticket is SoftwareTicket sw)
                {
                    cmd.Parameters.AddWithValue("@description", sw.Description ?? "");
                    cmd.Parameters.AddWithValue("@equipment", DBNull.Value);
                    cmd.Parameters.AddWithValue("@malfunction", DBNull.Value);
                    cmd.Parameters.AddWithValue("@software", sw.Software ?? "");
                }
                else
                {
                    // fallback
                    cmd.Parameters.AddWithValue("@description", ticket is SoftwareTicket ? ((SoftwareTicket)ticket).Description : "");
                    cmd.Parameters.AddWithValue("@equipment", "");
                    cmd.Parameters.AddWithValue("@malfunction", "");
                    cmd.Parameters.AddWithValue("@software", "");
                }

                conn.Open();
                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        #region TechnicianTicketManagementDAL

        /// <summary>  
        /// Manages ticket operations for technicians in the database.
        /// </summary>
        public bool UpdateTicketResponse(int ticketId, string response) {
        using (var conn = new SqlConnection(connectionString))
        {
            string query = @"
            UPDATE Tickets 
            SET Response = @Response, 
                Status = 'Fulfilled', 
                AnsweredAt = @AnsweredAt, 
                ServiceStatus = 'Resolved'
            WHERE TicketId = @TicketId";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Response", response);
            cmd.Parameters.AddWithValue("@AnsweredAt", DateTime.Now);
            cmd.Parameters.AddWithValue("@TicketId", ticketId);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }

        /// <summary>
        /// Updates the status and service status of a ticket.
        /// </summary>
        public bool UpdateTicketStatus(int ticketId, string status, string serviceStatus) {
            using (var conn = new SqlConnection(connectionString))
            {
                string query = "UPDATE Tickets SET Status = @Status, ServiceStatus = @ServiceStatus WHERE TicketId = @TicketId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@ServiceStatus", serviceStatus);
                cmd.Parameters.AddWithValue("@TicketId", ticketId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }


    /// <summary>
    /// Maps the data from a SqlDataReader row to a Ticket object.
    /// Uses the TicketFactory to create the appropriate subclass (Hardware or Software).
    /// </summary>
    /// <param name="reader">The SqlDataReader containing ticket data from the database.</param>
    /// <returns>A Ticket object (either HardwareTicket or SoftwareTicket) populated with data.</returns>
    private Ticket MapTicket(SqlDataReader reader)
    {
        string type = reader["TicketType"].ToString();
        Ticket ticket = TicketFactory.CreateTicket(type);

        ticket.Id = Convert.ToInt32(reader["TicketId"]);
        ticket.Username = reader["Username"].ToString();
        ticket.CreatedAt = Convert.ToDateTime(reader["CreatedAt"]);
        ticket.Status = reader["Status"]?.ToString() ?? "";
        ticket.ServiceStatus = reader["ServiceStatus"]?.ToString() ?? "";
        ticket.Response = reader["Response"].ToString();


        if (ticket is HardwareTicket hw)
        {
            hw.Equipment = reader["Equipment"]?.ToString() ?? "";
            hw.Malfunction = reader["Malfunction"]?.ToString() ?? "";
        }
        else if (ticket is SoftwareTicket sw)
        {
            sw.Software = reader["Software"]?.ToString() ?? "";
            sw.Description = reader["Description"]?.ToString() ?? "";
        }

        return ticket;
    }

    /// <summary>
    /// Gets all tickets from the database.
    /// </summary>
    public List<Ticket> GetAllTickets() {
            var tickets = new List<Ticket>();
            using (var conn = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Tickets";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ticket = MapTicket(reader);
                        tickets.Add(ticket);
                    }
                }
            }
            return tickets;
        }
        #endregion
    }
    


