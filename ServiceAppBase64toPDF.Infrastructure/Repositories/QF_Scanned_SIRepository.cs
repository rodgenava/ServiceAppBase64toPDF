using Microsoft.Extensions.Configuration;
using ServiceAppBase64toPDF.Application;
using ServiceAppBase64toPDF.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAppBase64toPDF.Infrastructure
{
    public class QF_Scanned_SIRepository : IQF_Scanned_SIRepository
    {
        //private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public QF_Scanned_SIRepository(IConfiguration configuration)
        {
            //_context = context;
            _configuration = configuration;
        }
        //public async Task<QF_Scanned_SI> GetScannedSIList()
        //{
        //    try
        //    {
        //        var result = await _context.QF_Scanned_SI
        //            //.Where(a => !string.IsNullOrEmpty(a.ImageScanned))
        //            .Select(a => new QF_Scanned_SI
        //            {
        //                RCRNumber = a.RCRNumber,
        //                SIpdf = a.SIpdf,
        //                //ImageScanned = a.ImageScanned,
        //                //ImageScanned2 = a.ImageScanned2,
        //                //ImageScanned3 = a.ImageScanned3,
        //                //ImageScanned4 = a.ImageScanned4
        //            }).FirstOrDefaultAsync();

        //        return result ?? new QF_Scanned_SI();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error during retrieving Scanned SI table: {ex.Message}");
        //        return new QF_Scanned_SI();
        //    }
        //}
        public async Task<QF_Scanned_SI> GetScannedSIList()
        {
            try
            {
                // Connection string to your SQL Server database
                string connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;

                // SQL select query
                string selectquery = String.Format("select RCRNumber,SIpdf,ImageScanned,ImageScanned2,ImageScanned3,ImageScanned4 from QF_Scanned_SI where CAST(ImageScanned AS VARCHAR(MAX)) <> ''");
                
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Create a SqlCommand
                    using (SqlCommand command = new SqlCommand(selectquery, connection))
                    {
                        // Execute the query and get a SqlDataReader
                        using (SqlDataReader sreader = command.ExecuteReader())
                        {
                            QF_Scanned_SI Scanned_SI = new QF_Scanned_SI();
                            // Loop through the rows
                            while (sreader.Read())
                            {
                                Scanned_SI.RCRNumber = sreader[0].ToString();
                                Scanned_SI.SIpdf = sreader[1].ToString();
                                Scanned_SI.ImageScanned = sreader[2].ToString();
                                Scanned_SI.ImageScanned2 = sreader[3].ToString();
                                Scanned_SI.ImageScanned3 = sreader[4].ToString();
                                Scanned_SI.ImageScanned4 = sreader[5].ToString();

                                return Scanned_SI;
                            }
                        }
                    }
                }

                return new QF_Scanned_SI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during retrieving Scanned SI table: {ex.Message}");
                return new QF_Scanned_SI();
            }
        }

        public async Task<bool> SetPDFtoScanned_SI(string SIpdf, string RCRNumber)
        {
            try
            {
                string connectionString = _configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string queryUpdate = "UPDATE QF_Scanned_SI SET [SIpdf] = @SIpdf,[ImageScanned] = '',[ImageScanned2] = '',[ImageScanned3] = '',[ImageScanned4] = ''  WHERE RCRNumber = @RCRNumber";

                    using (SqlCommand cmd = new SqlCommand(queryUpdate, connection))
                    {
                        cmd.Parameters.AddWithValue("@SIpdf", SIpdf);
                        cmd.Parameters.AddWithValue("@RCRNumber", RCRNumber);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return true;
                        }
                    }

                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during updating Scanned SI table: {ex.Message}");
                return false;
            }
        }
    }
}
