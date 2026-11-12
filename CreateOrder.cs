using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using static DnD_Trading.WstGrp22DataSet;

namespace DnD_Trading
{
    public partial class CreateOrder : Form
    {
        private Parent mainForm;
        private CreateOrderRequest createOrderRequestForm;

        public Label Label5;

        public CreateOrder(Parent p)
        {
            mainForm = p;
            InitializeComponent();
            Label5 = this.label5;
        }

        public CreateOrder(CreateOrderRequest c)
        {
            createOrderRequestForm = c;
            InitializeComponent();
            Label5 = this.label5;
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {

            // orderSupplierProductTableAdapter.Insert(globalvar.orderID, orderSupplierProductTableAdapter.);

            if (dataGridView1.Rows.Count == 1)
            {
                MessageBox.Show("No products in the order to create.", "No Products", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            viewOrderProduct1TableAdapter.FillByOrderID(wstGrp22DataSet3.ViewOrderProduct1, globalvar.orderID);
            productTableAdapter.Fill(wstGrp22DataSet2.Product);

            decimal price = 0;

            Dictionary<int, List<DataGridViewRow>> supplierRows = new Dictionary<int, List<DataGridViewRow>>();

            DataRow dr;
            dr = wstGrp22DataSet.OrderSupplierProduct.NewRow();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                dr = wstGrp22DataSet.OrderSupplierProduct.NewRow();

                dr["OrderID"] = globalvar.orderID;
                dr["ProductID"] = Convert.ToInt32(row.Cells[1].Value);
                dr["SupplierID"] = Convert.ToInt32(row.Cells[2].Value);
                dr["OrderSupplierProductQuantity"] = Convert.ToInt32(row.Cells[3].Value);
                dr["OrderSupplierProductPrice"] = Convert.ToDecimal(row.Cells[7].Value);
                dr["OrderSupplierProductStatus"] = false;

                price += Convert.ToDecimal(row.Cells[7].Value);

                int supplierID = Convert.ToInt32(row.Cells[2].Value);
                if (!supplierRows.ContainsKey(supplierID))
                {
                    supplierRows[supplierID] = new List<DataGridViewRow>();
                }
                supplierRows[supplierID].Add(row);

                // Add the row to the dataset
                // wstGrp22DataSet.OrderSupplierProduct.Rows.Add(dr);
            }

            orderTableAdapter.UpdateOrderAmount(price, globalvar.orderID);

            orderTableAdapter.UpdateStatus(true, globalvar.orderID);

            // wstGrp22DataSet.OrderSupplierProduct.Rows.Add(dr);
            orderSupplierProductTableAdapter.Update(wstGrp22DataSet.OrderSupplierProduct);
            MessageBox.Show("Order created successfully.", "Order Created", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            paymentTableAdapter.UpdatePaymentTotalAndPaymentDue(
                price, price,
                globalvar.paymentID // Use the global variable for payment ID
            );

            // Send email notification to client

            globalvar.orderMessage += "</table>";
            globalvar.orderMessage +=
                "<br/><br/>" + "<b>Total Price:</b> " + price.ToString("C2") + "<br/><br/>" +
                "Please send proof of payment to the following email address: " + "<b>DNDTrading22@gmail.com</b>";

            MessageBox.Show("Disable Global Protect", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show("Press OK and wait for email to send. A message box will appear soon.", "Wait",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            try
            {
                using (MailMessage msg = new MailMessage()) using (SmtpClient smtp = EmailConfiguration.CreateClient())
                {
                    msg.To.Add(globalvar.clientEmail);
                    msg.From = new MailAddress(EmailConfiguration.SenderAddress);
                    msg.Body = globalvar.orderMessage;
                    msg.Subject = "Requesting Proof of Payment from " + globalvar.clientName + " for Order " +
                                  globalvar.orderID.ToString();
                    msg.IsBodyHtml = true;
                    smtp.Send(msg);
                }
                MessageBox.Show("Email requesting proof of payment from client sent successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending email requesting proof of payment from client: " + ex.Message,
                                "Email Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            /*
            MessageBox.Show("Now sending order emails to suppliers...", "Supplier Emails", MessageBoxButtons.OK,
            MessageBoxIcon.Information);

            foreach (var entry in supplierRows)
            {
                int supplierID = entry.Key;
                List<DataGridViewRow> rowsForSupplier = entry.Value;

                DataRow supplierDataRow = null;
                try
                {
                    supplierDataRow = wstGrp22DataSet2.Supplier.AsEnumerable()
                                        .FirstOrDefault(s => s.Field<int>("SupplierID") == supplierID);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error finding supplier data for ID {supplierID}: {ex.Message}", "Data Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                this.productTableAdapter.Fill(this.wstGrp22DataSet.Product);

                if (supplierDataRow == null || supplierDataRow.IsNull("SupplierEmail"))
                {
                    MessageBox.Show($"Could not find email for Supplier ID: {supplierID}. Skipping email.", "Supplier
            Email Error", MessageBoxButtons.OK, MessageBoxIcon.Warning); continue;
                }

                string supplierEmail = supplierDataRow.Field<string>("SupplierEmail");
                string supplierName = supplierDataRow.Field<string>("SupplierName");

                StringBuilder supplierBody = new StringBuilder();
                supplierBody.Append($"<p>Hello {supplierName},</p>");
                supplierBody.Append($"<p>We would like to place an order for the following products (Our Order Ref:
            {globalvar.orderID}):</p>"); supplierBody.Append("<table border='1' cellpadding='5'
            style='border-collapse:collapse; font-family: Arial, sans-serif;'>"); supplierBody.Append("<tr
            style='background-color: #f2f2f2;'><th>Product Name</th><th>Quantity</th></tr>");

                foreach (DataGridViewRow productRow in rowsForSupplier)
                {
                    int productID = Convert.ToInt32(productRow.Cells[1].Value);
                    int quantity = Convert.ToInt32(productRow.Cells[3].Value);

                    string productName = $"Product ID: {productID}";
                    try
                    {
                        var productInfoRow = wstGrp22DataSet.Product.AsEnumerable()
                                                .FirstOrDefault(p => p.Field<int>("ProductID") == productID);

                        if (productInfoRow != null)
                        {
                            productName = productInfoRow.Field<string>("ProductName");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error finding product name for ID {productID}: {ex.Message}", "Data Error",
            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    supplierBody.Append("<tr>");
                    supplierBody.Append($"<td>{productName}</td>");
                    supplierBody.Append($"<td style='text-align:center;'>{quantity}</td>");
                    supplierBody.Append("</tr>");
                }

                supplierBody.Append("</table>");
                supplierBody.Append("<br/><p>Please confirm receipt of this order and provide an estimated delivery
            date.</p>"); supplierBody.Append("<p>Thank you,<br/>DnD Trading</p>");

                MailMessage supplierMsg = new MailMessage();
                supplierMsg.To.Add(supplierEmail);
                supplierMsg.From = new MailAddress(from);
                supplierMsg.Body = supplierBody.ToString();
                supplierMsg.Subject = $"DnD Trading - New Product Order (Ref: {globalvar.orderID})";
                supplierMsg.IsBodyHtml = true;

                try
                {
                    smtp.Send(supplierMsg);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error sending email to {supplierName} ({supplierEmail}): " + ex.Message, "Email
            Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            MessageBox.Show("Supplier email requests sent.", "Supplier Emails Complete", MessageBoxButtons.OK,
            MessageBoxIcon.Information);
            */
            MessageBox.Show("Enable Global Protect", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            dataGridView1.DataSource = null;                                 // Clear the DataGridView
            dataGridView1.DataSource = wstGrp22DataSet.OrderSupplierProduct; // Rebind to the updated dataset
            dataGridView1.Refresh();
            globalvar.priceTotal = 0; // Reset the total price
            MessageBox.Show("Order sent successfully.", "Order Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
            globalvar.orderID = 0;                                               // Reset the order ID
            globalvar.clientID = 0;                                              // Reset the client ID
            globalvar.clientName = string.Empty;                                 // Reset the client name
            globalvar.clientEmail = string.Empty;                                // Reset the client email
            label4.Text = "Total Price: " + globalvar.priceTotal.ToString("C2"); // Update the total price label

            createOrderRequestForm.Show();
            // mainForm.Panel1.Visible = true;
            this.Hide();
            createOrderRequestForm.WindowState = FormWindowState.Maximized; // Maximize the CreateOrderRequest form
        }

        void smtp_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Email sending was canceled.");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("Error sending email: " + e.Error.Message);
            }
            else
            {
                MessageBox.Show("Message sent successfully!");
            }
        }

        private void CreateOrder_Load(object sender, EventArgs e)
        {
            // this.viewOrderProduct1TableAdapter.Fill(this.wstGrp22DataSet3.ViewOrderProduct1);
            this.orderTableAdapter.Fill(this.wstGrp22DataSet3.Order);
            this.paymentTableAdapter.Fill(this.wstGrp22DataSet.Payment);
            this.createOrderSupplierProductTableAdapter.Fill(this.wstGrp22DataSet.CreateOrderSupplierProduct);
            this.productTableAdapter.Fill(this.wstGrp22DataSet.Product);
            this.supplierTableAdapter.Fill(this.wstGrp22DataSet2.Supplier);

            this.clientTableAdapter.Fill(this.wstGrp22DataSet2.Client);

            this.clientOrderProductTableAdapter.FillByOrderID(this.wstGrp22DataSet1.ClientOrderProduct,
                                                              globalvar.orderID);

            // this.orderSupplierProductTableAdapter.Fill(this.wstGrp22DataSet.OrderSupplierProduct);

            this.supplierProductTableAdapter.Fill(this.wstGrp22DataSet.SupplierProduct);
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the order?", "Cancel Order", MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dataGridView1.Rows.Count == 1)
                {
                    // MessageBox.Show("No products in the order to cancel.", "No Products", MessageBoxButtons.OK,
                    // MessageBoxIcon.Information); return;
                }
                orderTableAdapter.DeleteQuery(globalvar.orderID);
                clientOrderProductTableAdapter.DeleteQuery(globalvar.orderID);
                orderSupplierProductTableAdapter.DeleteQuery(globalvar.orderID);
                paymentTableAdapter.DeleteQuery(globalvar.orderID);

                dataGridView1.DataSource = null;                                 // Clear the DataGridView
                dataGridView1.DataSource = wstGrp22DataSet.OrderSupplierProduct; // Rebind to the updated dataset
                dataGridView1.Refresh();
                globalvar.priceTotal = 0; // Reset the total price
                MessageBox.Show("Order cancelled successfully.", "Order Cancelled", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                globalvar.orderID = 0;                                               // Reset the order ID
                globalvar.clientID = 0;                                              // Reset the client ID
                globalvar.clientName = string.Empty;                                 // Reset the client name
                globalvar.clientEmail = string.Empty;                                // Reset the client email
                label4.Text = "Total Price: " + globalvar.priceTotal.ToString("C2"); // Update the total price label

                createOrderRequestForm.Show();
                createOrderRequestForm.WindowState = FormWindowState.Maximized;
                // mainForm.Panel1.Visible = true; // Show the main form's panel
                // mainForm.MenuStrip1.Items[8].Visible = true; // Show the "Create Order" menu item
                this.Hide(); // Hide the CreateOrder form
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                if (mainForm == null)
                {
                    this.Hide();
                    createOrderRequestForm.Show();
                    label5.Text = "";
                    createOrderRequestForm.WindowState =
                        FormWindowState.Maximized; // Maximize the CreateOrderRequest form
                }
                else
                {
                    mainForm.Show();
                    mainForm.Panel1.Visible = true;
                    mainForm.MenuStrip1.Items[8].Visible = true;
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show("An error occurred while navigating back: " + ex.Message, "Error",
                // MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView3_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // add supplier product to order

            if (!int.TryParse(numericUpDown1.Text.Trim(), out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Product quantity must be a positive integer.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DataRow dr;
            dr = wstGrp22DataSet.OrderSupplierProduct.NewRow();

            int index = dataGridView3.CurrentRow.Index;

            int productID = Convert.ToInt32(wstGrp22DataSet.CreateOrderSupplierProduct.Rows[index]["ProductID"]);
            int supplierID = Convert.ToInt32(wstGrp22DataSet.CreateOrderSupplierProduct.Rows[index]["SupplierID"]);
            int price = Convert.ToInt32(wstGrp22DataSet.CreateOrderSupplierProduct.Rows[index]["SupplierProductPrice"]);

            dr["OrderID"] = globalvar.orderID;
            dr["ProductID"] = productID;
            dr["SupplierID"] = supplierID;
            dr["OrderSupplierProductQuantity"] = int.Parse(numericUpDown1.Text.Trim());

            productTableAdapter.FillByProductSurcharge(wstGrp22DataSet.Product, productID);

            // decimal surcharge = (decimal)Convert.ToDecimal(dataGridView6.CurrentRow.Cells[2].Value)/100

            decimal surcharge = 0;
            try
            {
                var productInfoRow =
                    wstGrp22DataSet.Product.AsEnumerable().FirstOrDefault(p => p.Field<int>("ProductID") == productID);

                if (productInfoRow != null)
                {
                    surcharge = productInfoRow.Field<decimal>("ProductSurcharge") / 100;
                }
                else
                {
                    MessageBox.Show($"Could not find product details for ID {productID} to calculate surcharge.",
                                    "Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finding product surcharge for ID {productID}: {ex.Message}", "Data Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dr["OrderSupplierProductPrice"] = ((decimal)dataGridView3.CurrentRow.Cells[2].Value +
                                               (surcharge * (decimal)dataGridView3.CurrentRow.Cells[2].Value)) *
                                              int.Parse(numericUpDown1.Text.Trim());
            globalvar.priceTotal += ((decimal)dataGridView3.CurrentRow.Cells[2].Value +
                                     (surcharge * (decimal)dataGridView3.CurrentRow.Cells[2].Value)) *
                                    int.Parse(numericUpDown1.Text.Trim());
            label4.Text = "Total Price: " + globalvar.priceTotal.ToString("C2");
            dr["OrderSupplierProductStatus"] = false; // Default value for OrderSupplierProductStatus

            globalvar.orderMessage += "<tr>" + "<td>" + dataGridView3.CurrentRow.Cells[0].Value.ToString() + "</td>" +
                                      "<td>" + numericUpDown1.Text.Trim() + "</td>" + "<td>" +
                                      (((decimal)dataGridView3.CurrentRow.Cells[2].Value +
                                        (surcharge * (decimal)dataGridView3.CurrentRow.Cells[2].Value)) *
                                       int.Parse(numericUpDown1.Text.Trim()))
                                          .ToString("C2") +
                                      "</td>" + "</tr>";

            wstGrp22DataSet.OrderSupplierProduct.Rows.Add(dr);
            orderSupplierProductTableAdapter.Update(wstGrp22DataSet.OrderSupplierProduct);
            numericUpDown1.Value = 0; // Reset the numericUpDown control after adding the product

            txtProductSearch.Clear(); // Clear the search box after adding the product

            viewOrderProduct1TableAdapter.FillByOrderID(wstGrp22DataSet3.ViewOrderProduct1, globalvar.orderID);
            dataGridView1.Refresh();
        }

        private void txtProductSearch_TextChanged(object sender, EventArgs e)
        {
            createOrderSupplierProductTableAdapter.FillByProductName(wstGrp22DataSet.CreateOrderSupplierProduct,
                                                                     txtProductSearch.Text.Trim());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Product product = new Product(this);
            product.Show();
            product.WindowState = FormWindowState.Maximized;
            product.TabControl1.SelectedIndex = 1; // Switch to the second tab
            this.Hide();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.Rows.Count > 0 && e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int index = e.RowIndex;

                // Get IDs from the dataset
                int clientOrderProductID =
                    Convert.ToInt32(wstGrp22DataSet1.ClientOrderProduct.Rows[index]["ClientOrderProductID"]);
                int orderID = Convert.ToInt32(wstGrp22DataSet1.ClientOrderProduct.Rows[index]["orderID"]);

                // Read the updated checkbox value
                bool isAdded = (bool)dataGridView2.Rows[index].Cells[2].Value;

                // Call update method with correct values
                clientOrderProductTableAdapter.UpdateStatus(isAdded, clientOrderProductID, orderID);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void CreateOrder_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void CreateOrder_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to exit? Any order in progress will be cancelled and the application will close.",
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                orderTableAdapter.DeleteQuery(globalvar.orderID);
                clientOrderProductTableAdapter.DeleteQuery(globalvar.orderID);
                orderSupplierProductTableAdapter.DeleteQuery(globalvar.orderID);
                paymentTableAdapter.DeleteQuery(globalvar.orderID);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
