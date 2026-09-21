using System;
using System.Configuration;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using BLL;
using System.Collections.Generic;
using System.Linq;

namespace PA_WPF_DB
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private DashboardStatisticsService statisticsService;

        // Properties bound to the charts
        public SeriesCollection ServiceStatusSeries { get; set; }
        public SeriesCollection AverageTimeSeries { get; set; }
        public string[] TimeLabels { get; set; }
        public string FulfilledPercentageText { get; set; }

        public DashboardWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager
                .ConnectionStrings["Ticket2HelpConnection"].ConnectionString;

            statisticsService = new DashboardStatisticsService(connectionString);

            LoadDashboardData();
            DataContext = this; // Bind data to XAML
        }

        private void LoadDashboardData()
        {
            // Convert nullable DatePickers to non-nullable with default values
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            // Evita errores con fechas nulas
            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = new DateTime(2000, 1, 1); // fecha por defecto
                endDate = DateTime.Now;
            }

            // 📊 Pie Chart - Distribución de estados del servicio
            var statusData = statisticsService.GetServiceStatusDistribution(startDate, endDate);
            ServiceStatusSeries = new SeriesCollection();
            foreach (var kvp in statusData)
            {
                ServiceStatusSeries.Add(new PieSeries
                {
                    Title = kvp.Key,
                    Values = new ChartValues<int> { kvp.Value },
                    DataLabels = true
                });
            }

            // 📉 Column Chart - Tiempo medio de servicio por tipo
            var avgTimes = statisticsService.GetAverageServiceTimePerType(startDate, endDate);
            AverageTimeSeries = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Average Time",
            Values = new ChartValues<double>(avgTimes.Values)
        }
    };
            TimeLabels = avgTimes.Keys.ToArray();

            // ✅ Porcentaje de tickets cumplidos
            double fulfilled = statisticsService.GetPercentageFulfilledWithinDateRange(startDate.Value, endDate.Value);
            FulfilledPercentageText = $"Fulfilled Tickets: {fulfilled:0.##}%";

            // 🔄 Refresca los gráficos en el XAML
            DataContext = null;
            DataContext = this;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            LoadDashboardData();
        }

        /// <summary>
        /// Handles the back button click to return to the technician dashboard.
        /// </summary>
        private void BackToTickets_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Simply closes the dashboard window
        }
    }
}

