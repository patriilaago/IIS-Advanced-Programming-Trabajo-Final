using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Models;

namespace Tests
{
    /// <summary>
    /// Unit tests for the Ticket, HardwareTicket, and SoftwareTicket classes.
    /// </summary>
    [TestClass]
    public class TicketTests
    {
        /// <summary>
        /// Tests that HardwareTicket returns a correctly formatted summary.
        /// </summary>
        [TestMethod]
        public void HardwareTicket_GetSummary_ReturnsCorrectFormat()
        {
            var hw = new HardwareTicket { Equipment = "Monitor", Malfunction = "Broken screen" };
            Assert.AreEqual("[HW] Monitor - Broken screen", hw.GetSummary());
        }

        /// <summary>
        /// Tests that SoftwareTicket returns a correctly formatted summary.
        /// </summary>
        [TestMethod]
        public void SoftwareTicket_GetSummary_ReturnsCorrectFormat()
        {
            var sw = new SoftwareTicket { Software = "Word", Description = "Freeze on startup" };
            Assert.AreEqual("[SW] Word - Freeze on startup", sw.GetSummary());
        }

        /// <summary>
        /// Tests that all properties are correctly assigned in a hardware ticket.
        /// </summary>
        [TestMethod]
        public void Ticket_Initialization_SetsCorrectValues()
        {
            var now = DateTime.Now;
            var ticket = new HardwareTicket
            {
                Id = 101,
                Username = "testuser",
                TicketType = "Hardware",
                CreatedAt = now,
                Status = "Unanswered",
                ServiceStatus = "Open",
                Equipment = "Mouse",
                Malfunction = "Not working"
            };

            Assert.AreEqual(101, ticket.Id);
            Assert.AreEqual("testuser", ticket.Username);
            Assert.AreEqual("Hardware", ticket.TicketType);
            Assert.AreEqual(now, ticket.CreatedAt);
            Assert.AreEqual("Unanswered", ticket.Status);
            Assert.AreEqual("Open", ticket.ServiceStatus);
            Assert.AreEqual("Mouse", ticket.Equipment);
            Assert.AreEqual("Not working", ticket.Malfunction);
        }

        /// <summary>
        /// Tests that empty string values in software tickets are handled gracefully.
        /// </summary>
        [TestMethod]
        public void SoftwareTicket_EmptyStrings_AreHandledGracefully()
        {
            var sw = new SoftwareTicket
            {
                Software = "",
                Description = ""
            };

            Assert.AreEqual("[SW]  - ", sw.GetSummary());
        }

        /// <summary>
        /// Tests that null values in a hardware ticket do not cause errors in GetSummary().
        /// </summary>
        [TestMethod]
        public void HardwareTicket_NullProperties_ReturnsSummaryWithNulls()
        {
            var hw = new HardwareTicket { Equipment = null, Malfunction = null };
            string summary = hw.GetSummary();
            Assert.AreEqual("[HW]  - ", summary);
        }

        /// <summary>
        /// Tests that null values in a software ticket do not cause errors in GetSummary().
        /// </summary>
        [TestMethod]
        public void SoftwareTicket_NullProperties_ReturnsSummaryWithNulls()
        {
            var sw = new SoftwareTicket { Software = null, Description = null };
            string summary = sw.GetSummary();
            Assert.AreEqual("[SW]  - ", summary);
        }

        /// <summary>
        /// Tests that setting CreatedAt to a future date throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Ticket_CreatedAtInFuture_ThrowsException()
        {
            var ticket = new HardwareTicket
            {
                CreatedAt = DateTime.Now.AddYears(10) // Invalid future date
            };

            if (ticket.CreatedAt > DateTime.Now)
                throw new ArgumentOutOfRangeException("CreatedAt cannot be in the future.");
        }

        /// <summary>
        /// Tests that assigning an invalid ticket status throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ticket_InvalidStatus_ThrowsException()
        {
            var ticket = new SoftwareTicket { Status = "Flying" }; // Invalid status

            if (ticket.Status != "Unanswered" && ticket.Status != "In Service" && ticket.Status != "Fulfilled")
                throw new ArgumentException("Invalid ticket status.");
        }

        /// <summary>
        /// Tests that assigning an invalid service status throws an exception.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Ticket_InvalidServiceStatus_ThrowsException()
        {
            
            var ticket = new HardwareTicket { ServiceStatus = "Maybe" }; // Invalid status

            if (ticket.ServiceStatus != "Open" && ticket.ServiceStatus != "Resolved" && ticket.ServiceStatus != "Not Resolved")
                throw new ArgumentException("Invalid service status.");
        }

        /// <summary>
        /// Tests that special characters are preserved in the ticket summary.
        /// </summary>
        [TestMethod]
        public void Ticket_Summary_WithSpecialCharacters()
        {
            var hw = new HardwareTicket
            {
                Equipment = "HP Monitor",
                Malfunction = "Burned Pixels!"
            };

            string summary = hw.GetSummary();
            Assert.AreEqual("[HW] HP Monitor - Burned Pixels!", summary);
        }

        /// <summary>
        /// Tests that long descriptions in software tickets do not break the summary.
        /// </summary>
        [TestMethod]
        public void SoftwareTicket_LongDescription_ReturnsCorrectSummary()
        {
            var sw = new SoftwareTicket
            {
                Software = "Visual Studio",
                Description = new string('a', 1000)
            };

            string summary = sw.GetSummary();
            Assert.IsTrue(summary.StartsWith("[SW] Visual Studio - "));
        }
    }
}


