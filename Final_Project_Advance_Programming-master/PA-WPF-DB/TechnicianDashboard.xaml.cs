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
using Models;
using DAL;

namespace PA_WPF_DB
{
    /// <summary>
    /// Interaction logic for TechnicianDashboard.xaml
    /// </summary>
    public partial class TechnicianDashboard : Window
    {
        private readonly TicketManagementBLL _ticketManager = new TicketManagementBLL();

        public TechnicianDashboard()
        {
            InitializeComponent();
            LoadAllTickets();
        }

        private void LoadAllTickets()
        {
            List<Ticket> tickets = _ticketManager.GetAllTickets();
            dgAllTickets.ItemsSource = tickets;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAllTickets();
        }

        private void Respond_Click(object sender, RoutedEventArgs e)
        {
            Ticket selected = dgAllTickets.SelectedItem as Ticket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to respond.");
                return;
            }

            if (selected.ServiceStatus == "Resolved")
            {
                MessageBox.Show("This ticket has already been resolved and cannot be updated.");
                return;
            }

            var responseWindow = new TechnicianResponseWindow(selected.Id);
            bool? result = responseWindow.ShowDialog();

            if (result == true)
            {
                LoadAllTickets(); // vuelve a cargar los datos con la nueva respuesta
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Ticket selected = dgAllTickets.SelectedItem as Ticket;
            if (selected == null)
            {
                MessageBox.Show("Select a ticket to close.");
                return;
            }

            bool result = _ticketManager.CloseTicket(selected.Id);
            MessageBox.Show(result ? "Ticket closed successfully." : "Failed to close ticket.");

            LoadAllTickets();
        }

        /// <summary>
        /// Opens the dashboard statistics window.
        /// </summary>
        private void ViewStats_Click(object sender, RoutedEventArgs e)
        {
            var statsWindow = new DashboardWindow();
            statsWindow.ShowDialog();
        }
    }
}

