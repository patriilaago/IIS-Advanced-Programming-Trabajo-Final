using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    /// Base class for tickets in the Ticket2Help system.
    /// </summary>
    public abstract class Ticket
    {
        /// <summary>
        /// Gets or sets the unique identifier for the ticket.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the ticket.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the type of the ticket, which can be 'Hardware' or 'Software'.
        /// </summary>
        public string TicketType { get; set; } // 'Hardware', 'Software'

        /// <summary>
        /// Gets or sets the date and time when the ticket was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the ticket was answered.
        /// </summary>
        public DateTime AnsweredAt { get; set; }

        /// <summary>
        /// Gets or sets the status of the ticket, which can be 'Unanswered', 'In Service', or 'Fulfilled'.
        /// </summary>
        public string Status { get; set; } // 'Unanswered', 'In Service', 'Fulfilled'

        /// <summary>
        /// Gets or sets the service status of the ticket, which can be 'Open', 'Resolved', or 'Not Resolved'.
        /// </summary>
        public string ServiceStatus { get; set; } // 'Open', 'Resolved', 'Not Resolved'

        /// <summary>
        /// Response for the ticket
        /// </summary>
        public string Response { get; set; }

        public abstract string GetSummary(); 
    }
    /// <summary>
    /// Represents a hardware-related ticket in the Ticket2Help system.
    /// </summary>
    public class HardwareTicket : Ticket
    {

        /// <summary>
        /// Gets or sets the equipment involved in the hardware ticket and the malfunction description.
        /// </summary>
        public string Equipment { get; set; }

        /// <summary>
        /// Gets or sets the description of the malfunction in the hardware ticket.
        /// </summary>
        public string Malfunction { get; set; }

        /// <summary>
        /// Gets a summary of the hardware ticket.
        /// </summary>
        /// <returns>The sumary of the software tickets</returns>
        public override string GetSummary()
        {
            return $"[HW] {Equipment} - {Malfunction}";
        }
    }

    /// <summary>
    /// Represents a software-related ticket in the Ticket2Help system.
    /// </summary>
    public class SoftwareTicket : Ticket
    {
        /// <summary>
        /// Gets or sets the software involved in the software ticket and the description of the issue.
        /// </summary>
        public string Software { get; set; }

        /// <summary>
        /// Gets or sets the description of the issue in the software ticket.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets a summary of the software ticket.
        /// </summary>
        /// <returns>The sumary of the hardware tickets</returns>
        public override string GetSummary()
        {
            return $"[SW] {Software} - {Description}";
        }
    }
}
