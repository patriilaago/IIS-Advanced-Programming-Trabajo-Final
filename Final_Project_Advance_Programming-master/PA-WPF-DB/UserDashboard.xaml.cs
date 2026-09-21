using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BLL;
using DAL;

namespace PA_WPF_DB
{
    /// <summary>
    /// Lógica de interacción para UserDashboard.xaml
    /// </summary>
    public partial class UserDashboard : Window
    {
        public UserDashboard()
        {
            InitializeComponent();
        }

        private readonly string _username;
        private readonly TicketManagement _ticketManager = new TicketManagement();

        /// <summary>
        /// Constructor for UserDashboard that initializes the window with the username of the logged-in user.
        /// </summary>
        /// <param name="username"></param>
        public UserDashboard(string username)
        {
            InitializeComponent();
            _username = username;
            LoadTickets();
        }

        /// <summary>
        /// Loads the tickets for the current user into the DataGrid.
        /// </summary>
        private void LoadTickets()
        {
            List<Ticket> tickets = _ticketManager.GetTicketsByUser(_username);
            dgTickets.ItemsSource = tickets;
        }


        /// <summary>
        /// Handles the click event for the RefreshTickets button to reload the tickets.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshTickets_Click(object sender, RoutedEventArgs e)
        {
            LoadTickets();
        }

        /// <summary>
        /// Handles the click event for the AddTicket button to open a new ticket window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddTicket_Click(object sender, RoutedEventArgs e)
        {
            var newTicketWindow = new NewTicketWindow(_username);
            newTicketWindow.ShowDialog();

            // Recargar tickets después de cerrar
            LoadTickets();
        }

    }
}
