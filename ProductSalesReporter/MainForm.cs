using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductSalesReporter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            SetDefaultDates();
        }

        private void SetDefaultDates()
        {
            var today = DateTime.Today;
            dtpStartDate.Value = new DateTime(today.Year, today.Month, 1);
            dtpEndDate.Value = today;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date;
                string productName = txtProductNameFilter.Text;
                var salesData = DataAccess.GetSalesData(startDate, endDate, productName);
                if (salesData == null || salesData.Count == 0)
                {
                    MessageBox.Show("No sales data found for the selected criteria.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var report = new ProductSalesReport();
                report.DataSource = salesData;
                report.Parameters["paramStartDate"].Value = startDate;
                report.Parameters["paramEndDate"].Value = endDate;
                ReportPrintTool printTool = new ReportPrintTool(report);
                printTool.ShowPreviewDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while generating the report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = dtpStartDate.Value.Date;
                DateTime endDate = dtpEndDate.Value.Date;
                string productName = txtProductNameFilter.Text;
                var salesData = DataAccess.GetSalesData(startDate, endDate, productName);
                if (salesData == null || salesData.Count == 0)
                {
                    MessageBox.Show("No data to export.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var report = new ProductSalesReport();
                report.DataSource = salesData;
                report.Parameters["paramStartDate"].Value = startDate;
                report.Parameters["paramEndDate"].Value = endDate;
                using (SaveFileDialog saveDialog = new SaveFileDialog())
                {
                    saveDialog.Filter = "PDF file (*.pdf)|*.pdf";
                    saveDialog.Title = "Save Report as PDF";
                    saveDialog.FileName = $"ProductSalesReport_{DateTime.Now:yyyyMMdd}.pdf";
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        report.ExportToPdf(saveDialog.FileName);
                        MessageBox.Show($"Report successfully saved to:\n{saveDialog.FileName}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during export: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
