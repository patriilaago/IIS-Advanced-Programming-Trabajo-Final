using DAL;
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
using DAL;

namespace PA_WPF_DB
{
    /// <summary>
    /// Lógica de interacción para NewTicket.xaml
    /// </summary>
    public partial class NewTicketWindow : Window
    {
        private readonly string _username;
        private readonly TicketManagement _ticketManager = new TicketManagement();

        public NewTicketWindow(string username)
        {
            InitializeComponent();
            _username = username;
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            // obtener SW o HW del ComboBox
            var typeItem = cbType.SelectedItem as ComboBoxItem;
            string selectedType = typeItem?.Content?.ToString();

            if (string.IsNullOrWhiteSpace(selectedType))
            {
                MessageBox.Show("Please select a ticket type.");
                return;
            }

            // Crear instancia usando fabric pattern
            Ticket ticket;
            try
            {
                ticket = TicketFactory.CreateTicket(selectedType);
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Invalid ticket type.");
                return;
            }

            // Asignar propiedades en común
            ticket.Username = _username;
            ticket.CreatedAt = DateTime.Now;
            ticket.Status = "Unanswered";
            ticket.ServiceStatus = "Not Resolved";

            // Asignar propiedades específicas
            if (ticket is HardwareTicket hwTicket)
            {
                string equipment = txtEquipment.Text?.Trim();
                string malfunction = txtMalfunction.Text?.Trim();

                if (string.IsNullOrWhiteSpace(equipment) || string.IsNullOrWhiteSpace(malfunction))
                {
                    MessageBox.Show("Please enter both Equipment and Malfunction for hardware tickets.");
                    return;
                }

                hwTicket.Equipment = equipment;
                hwTicket.Malfunction = malfunction;
            }
            else if (ticket is SoftwareTicket swTicket)
            {
                string software = txtSoftware.Text?.Trim();
                string description = txtDescription.Text?.Trim();

                if (string.IsNullOrWhiteSpace(software) || string.IsNullOrWhiteSpace(description))
                {
                    MessageBox.Show("Please enter both Software and Description for software tickets.");
                    return;
                }

                swTicket.Software = software;
                swTicket.Description = description;
            }

            // Guardar en la base de datos
            bool success = _ticketManager.InsertTicket(ticket);

            if (success)
            {
                MessageBox.Show("Ticket created successfully!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to create ticket.");
            }
        }

        // Para mostrar los campos dependiendo de si es HW o SW issue
        private void cbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = cbType.SelectedItem as ComboBoxItem;
            string selectedType = selectedItem?.Content?.ToString();

            panelHardware.Visibility = selectedType == "Hardware" ? Visibility.Visible : Visibility.Collapsed;
            panelSoftware.Visibility = selectedType == "Software" ? Visibility.Visible : Visibility.Collapsed;
        }

    }
}
