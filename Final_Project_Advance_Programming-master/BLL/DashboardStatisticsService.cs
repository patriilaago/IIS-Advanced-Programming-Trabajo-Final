using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace BLL
{
    public class DashboardStatisticsService
    {
        private readonly string connectionString;

        public DashboardStatisticsService(string connectionString)
        {
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Calculates the percentage of fulfilled tickets within the given date range.
        /// </summary>
        public double GetPercentageFulfilledWithinDateRange(DateTime start, DateTime end)
        {
            DateTime sqlMinDate = new DateTime(1753, 1, 1);
            if (start < sqlMinDate) start = sqlMinDate;
            if (end < sqlMinDate) end = sqlMinDate;

            const string query = @"
                SELECT COUNT(*) AS Total,
                       SUM(CASE WHEN Status = 'Fulfilled' THEN 1 ELSE 0 END) AS Fulfilled
                FROM Tickets
                WHERE CreatedAt BETWEEN @start AND @end";

            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@start", start);
            cmd.Parameters.AddWithValue("@end", end);
            conn.Open();

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int total = reader["Total"] != DBNull.Value ? Convert.ToInt32(reader["Total"]) : 0;
                int fulfilled = reader["Fulfilled"] != DBNull.Value ? Convert.ToInt32(reader["Fulfilled"]) : 0;

                return total > 0 ? (double)fulfilled / total * 100 : 0;
            }
            return 0;
        }

        /// <summary>
        /// Returns the number of tickets grouped by service status.
        /// Optionally filters by date.
        /// </summary>
        public Dictionary<string, int> GetServiceStatusDistribution(DateTime? start = null, DateTime? end = null)
        {
            DateTime sqlMinDate = new DateTime(1753, 1, 1);
            if (start < sqlMinDate) start = sqlMinDate;
            if (end < sqlMinDate) end = sqlMinDate;

            var result = new Dictionary<string, int>();

            string query = "SELECT ServiceStatus, COUNT(*) AS Count FROM Tickets";
            if (start.HasValue && end.HasValue)
            {
                query += " WHERE CreatedAt BETWEEN @start AND @end";
            }
            query += " GROUP BY ServiceStatus";

            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, conn);

            if (start.HasValue && end.HasValue)
            {
                cmd.Parameters.AddWithValue("@start", start.Value);
                cmd.Parameters.AddWithValue("@end", end.Value);
            }

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string status = reader["ServiceStatus"].ToString();
                int count = Convert.ToInt32(reader["Count"]);
                result[status] = count;
            }

            return result;
        }

        /// <summary>
        /// Calculates the average service time (in minutes) for each ticket type.
        /// Optionally filters by date.
        /// </summary>
        public Dictionary<string, double> GetAverageServiceTimePerType(DateTime? start = null, DateTime? end = null)
        {
            DateTime sqlMinDate = new DateTime(1753, 1, 1);
            if (start < sqlMinDate) start = sqlMinDate;
            if (end < sqlMinDate) end = sqlMinDate;

            var result = new Dictionary<string, double>();

            string query = @"
                SELECT TicketType,
                       AVG(DATEDIFF(MINUTE, CreatedAt, AnsweredAt)) AS AvgMinutes
                FROM Tickets
                WHERE AnsweredAt IS NOT NULL";

            if (start.HasValue && end.HasValue)
            {
                query += " AND CreatedAt BETWEEN @start AND @end";
            }

            query += " GROUP BY TicketType";

            using var conn = new SqlConnection(connectionString);
            using var cmd = new SqlCommand(query, conn);

            if (start.HasValue && end.HasValue)
            {
                cmd.Parameters.AddWithValue("@start", start.Value);
                cmd.Parameters.AddWithValue("@end", end.Value);
            }

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string type = reader["TicketType"].ToString();
                if (double.TryParse(reader["AvgMinutes"]?.ToString(), out double avg))
                {
                    result[type] = avg;
                }
            }

            return result;
        }
    }
}

