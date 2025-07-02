using ProductSalesReporter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

public static class DataAccess
{
    private static readonly string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Leeseangbok.JUSTCALLMET\OneDrive\Documents\SaleDB.mdf;Integrated Security=True;Connect Timeout=30";

    public static List<SaleDto> GetSalesData(DateTime startDate, DateTime endDate, string productNameFilter)
    {
        var salesList = new List<SaleDto>();
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("sp_GetProductSales", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@StartDate", SqlDbType.Date).Value = startDate;
                    command.Parameters.Add("@EndDate", SqlDbType.Date).Value = endDate;
                    if (string.IsNullOrWhiteSpace(productNameFilter))
                    {
                        command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 100).Value = DBNull.Value;
                    }
                    else
                    {
                        command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 100).Value = productNameFilter;
                    }
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var sale = new SaleDto
                            {
                                ProductCode = reader["PRODUCTCODE"].ToString(),
                                ProductName = reader["PRODUCTNAME"].ToString(),
                                Quantity = Convert.ToInt32(reader["QUANTITY"]),
                                UnitPrice = Convert.ToDecimal(reader["UNITPRICE"]),
                                Total = Convert.ToDecimal(reader["TOTAL"]),
                                SaleDate = Convert.ToDateTime(reader["SALEDATE"])
                            };
                            salesList.Add(sale);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LogError(ex);
            throw;
        }
        return salesList;
    }
    public static void LogError(Exception ex)
    {
        try
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            string logFile = Path.Combine(logDirectory, "errors.txt");
            string errorMessage = $"{DateTime.Now}: {ex.Message}\nStack Trace: {ex.StackTrace}\n\n";
            File.AppendAllText(logFile, errorMessage);
        }
        catch
        {
        }
    }
}