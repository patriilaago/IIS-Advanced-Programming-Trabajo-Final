using BLL;
using DAL;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;


namespace PA_WPF_DB
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        //Botón enter para ver si es un empleado normal o un tecnico
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            var userService = new UserService();
            var user = userService.ValidateLogin(username, password);

            if (user != null) //si el usuario existe
            {
                if (user.Role == "Technician") //si es técnico
                {
                    new TechnicianDashboard().Show();
                }
                else //si es usuario normal
                {
                    new UserDashboard(user.Username).Show();
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect Credentials.");
            }
        }

    }
}