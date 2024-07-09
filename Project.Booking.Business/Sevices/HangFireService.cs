using Project.Booking.Constants;
using Project.Booking.Extensions;
using Project.Booking.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Project.Booking.Business.Sevices
{
    public class HangFireService
    {
        public HangFireService()
        {

        }
        public List<Unit> UpdateBookingPaymentOverdue(int overDueInterval)
        {
            SqlParameter[] parameters =
            {
                 new SqlParameter("@overDueInterval", SqlDbType.Int) { Value = overDueInterval}
                  };

            DataTable dt = ExecuteDataTable(Constant.ONLINE_BOOKING_CONN, "sp_Service_UpdateBookingPaymentOverdue", parameters);
            var units = dt.ConvertDataTable<Unit>();
            return units;
        }
        #region Private Function
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
                    cmd.CommandTimeout = 3600;
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
        #endregion
    }
}