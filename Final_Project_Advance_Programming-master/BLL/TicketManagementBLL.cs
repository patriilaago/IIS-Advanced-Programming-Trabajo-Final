using System;
using System.Collections.Generic;
using DAL;
using Models;

/// <summary>
/// Class for managing ticket responses and statuses in the system.
/// </summary>
public class TicketManagementBLL
{
    private TicketManagement _ticketManag = new TicketManagement();
    
    #region TechnicianTicketManagementBLL

    /// <summary>
    /// Constructor for TicketManagementBLL that initializes the ticket repository.
    /// </summary>
    /// <param name="ticketId">Id of the ticket to response</param>
    /// <param name="response">Response for the ticket</param>
    /// <returns> Returns true if ticket have response and false in the other case</returns>
    public bool RespondToTicket(int ticketId, string response)
    {
        return _ticketManag.UpdateTicketResponse(ticketId, response);
    }

    /// <summary>
    /// Closes a ticket by updating its status to "Closed".
    /// </summary>
    /// <param name="ticketId"></param>
    /// <returns>Returns true if ticket can stay close and false in the other case</returns>
    public bool CloseTicket(int ticketId)
    {
        return _ticketManag.UpdateTicketStatus(ticketId, "Fulfilled", "Resolved");
    }

    /// <summary>
    /// Retrieves all tickets from the database.
    /// </summary>
    /// <returns>A list with all the tickets</returns>
    public List<Ticket> GetAllTickets()
    {
        return _ticketManag.GetAllTickets();
    }

    #endregion
}
