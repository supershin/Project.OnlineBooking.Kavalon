using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Booking.Services
{
    public class DatabaseContext
    {
        private readonly string connectionString = ConfigurationManager.AppSettings["ConnectionString"].ToString();
        private DataTable ExecuteDataTable(string connectionString, string storedProcedureName, params SqlParameter[] arrParam)
        {

            DataTable dt = new DataTable();
            // Open the connection  
            using (SqlConnection cnn = new SqlConnection(connectionString))
            {
                cnn.Open();

                // Define the command 
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = storedProcedureName;
                    cmd.CommandTimeout = 30;
                    // Handle the parameters 
                    if (arrParam != null)
                    {
                        foreach (SqlParameter param in arrParam)
                            cmd.Parameters.Add(param);
                    }

                    // Define the data adapter and fill the dataset 
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        public void UpdateBookingOverduePayment() {
            ExecuteDataTable(connectionString, "sp_Service_UpdateBookingPaymentOverdue");
        }
    }
}
