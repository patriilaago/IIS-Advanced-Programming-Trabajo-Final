using BLL;
using System.Windows;
using DAL;

namespace PA_WPF_DB
{
    /// <summary>
    /// Interaction logic for TechnicianResponseWindow.xaml
    /// </summary>
    public partial class TechnicianResponseWindow : Window
    {
        private readonly int _ticketId;
        private readonly TicketManagementBLL _ticketManagerBLL = new TicketManagementBLL();

        /// <summary>
        /// Constructor for TechnicianResponseWindow that initializes the window with the ticket ID to respond to.
        /// </summary>
        /// <param name="ticketId">The ID of the ticket to build</param>
        public TechnicianResponseWindow(int ticketId)
        {
            InitializeComponent();
            _ticketId = ticketId;
        }

        /// <summary>
        /// Handles the click event for the Submit button to submit the technician's response to the ticket.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string response = txtResponse.Text.Trim();

            if (string.IsNullOrWhiteSpace(response))
            {
                MessageBox.Show("Response cannot be empty.");
                return;
            }

            bool result = _ticketManagerBLL.RespondToTicket(_ticketId, response);
            
            if (result)
            {
                MessageBox.Show("Response submitted.");
                this._ticketManagerBLL.CloseTicket(_ticketId);
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to submit response.");
            }
        }
    }
}
