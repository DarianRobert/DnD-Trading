using DnD_Trading.WstGrp22DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnD_Trading
{
    public partial class CreateOrderRequest : Form
    {
        private Parent mainForm;

        public CreateOrderRequest(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private void CreateOrderRequest_Load(object sender, EventArgs e)
        {
            this.productTableAdapter.Fill(this.wstGrp22DataSet.Product);
            this.paymentTableAdapter.FillByDESC(this.wstGrp22DataSet.Payment);
            this.orderTableAdapter.Fill(this.wstGrp22DataSet.Order);
            this.clientTableAdapter.FillByOptOutFalse(this.wstGrp22DataSet.Client);
            // this.clientOrderProductTableAdapter.Fill(this.wstGrp22DataSet.ClientOrderProduct);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show("Add new product to order?", "New Product", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                string desc = txtProdDesc.Text.Trim();

                // Validate Product Description
                if (string.IsNullOrWhiteSpace(desc))
                {
                    MessageBox.Show("Product description is required.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    txtProdDesc.Focus();
                    return;
                }

                // Validate Product Description Format
                if (!System.Text.RegularExpressions.Regex.IsMatch(desc, @"^(?=.*[A-Za-z])[A-Za-z0-9\s\-,.()'/&]+$"))
                {
                    MessageBox.Show(
                        "Product description must contain valid characters (letters, numbers, spaces, and basic punctuation).",
                        "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProdDesc.Focus();
                    return;
                }

                // Validate Product Description Length
                if (desc.Length < 3 || desc.Length > 70)
                {
                    MessageBox.Show("Product description should be between 3 and 70 characters.", "Invalid Length",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProdDesc.Focus();
                    return;
                }

                // Reject repetitive or vowel-less nonsense ("rrrrrr", "qwrty")
                // Check for 4+ repeated identical letters
                if (System.Text.RegularExpressions.Regex.IsMatch(desc, @"(.)\1{3,}"))
                {
                    MessageBox.Show(
                        "Product description appears invalid (repeated characters). Please enter a proper description.",
                        "Invalid Description", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtProdDesc.Focus();
                    return;
                }

                // Check that there's at least one vowel
                if (!System.Text.RegularExpressions.Regex.IsMatch(desc, "[aeiouAEIOU]"))
                {
                    DialogResult confirm = MessageBox.Show(
                        "The product description doesn't seem to contain any vowels. Are you sure this is correct?",
                        "Unusual Description", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.No)
                    {
                        txtProdDesc.Focus();
                        return;
                    }
                }

                // Validate Product Quantity
                if (!int.TryParse(numericUpDown1.Text.Trim(), out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Product quantity must be a positive integer.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    numericUpDown1.Focus();
                    return;
                }

                // Check if product description shares any word with known products
                var knownProducts = productTableAdapter.GetData();
                bool hasMatch = false;

                // Split the user's description into individual lowercase words
                var userWords = desc.ToLower().Split(new[] { ' ', '-', ',', '.', '(', ')', '/', '&' },
                                                     StringSplitOptions.RemoveEmptyEntries);

                // Loop through all products in the database
                foreach (var row in knownProducts)
                {
                    string productName = row.ProductName.ToString().ToLower();
                    // Split each product into words
                    var productWords = productName.Split(new[] { ' ', '-', ',', '.', '(', ')', '/', '&' },
                                                         StringSplitOptions.RemoveEmptyEntries);

                    // Check for overlap
                    if (userWords.Any(w => productWords.Contains(w)))
                    {
                        hasMatch = true;
                        break;
                    }
                }

                // If no match, ask the user for confirmation
                if (!hasMatch)
                {
                    DialogResult confirm = MessageBox.Show(
                        "No similar products were found in the database.\nAre you sure you want to add this product description?",
                        "Unrecognized Product", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirm == DialogResult.No)
                    {
                        txtProdDesc.Focus();
                        return;
                    }
                }

                // Insert the product into the order
                clientOrderProductTableAdapter.Insert(globalvar.orderID, globalvar.clientID, desc, quantity, false);

                MessageBox.Show("Product added to order successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                // Clear input fields after adding
                txtProdDesc.Clear();
                numericUpDown1.Value = 0;

                // Refresh data
                clientOrderProductTableAdapter.FillByOrderID(this.wstGrp22DataSet.ClientOrderProduct,
                                                             globalvar.orderID);
            }
            else
            {
                MessageBox.Show("Product addition cancelled.", "Cancelled", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void dataGridView2_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            globalvar.clientID = (int)dataGridView2.CurrentRow.Cells[0].Value;
            globalvar.clientName = dataGridView2.CurrentRow.Cells[1].Value.ToString();
            globalvar.clientEmail = dataGridView2.CurrentRow.Cells[3].Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (globalvar.clientID == 0)
            {
                MessageBox.Show("Please select a client before creating an order.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // globalvar.orderID = Convert.ToInt32(dataGridView3.Rows[0].Cells[0].Value) + 1;

            int paymentID;

            if (dataGridView3.Rows.Count > 0 && dataGridView3.Rows[0].Cells[0].Value != null)
            {
                paymentID = Convert.ToInt32(dataGridView4.Rows[0].Cells[0].Value) + 1;
            }
            else
            {
                paymentID = 1;
            }

            DialogResult result = MessageBox.Show("Create new order record?", "New Order", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                orderTableAdapter.Insert(globalvar.clientID, // ClientID
                                         globalvar.userName, // UserName
                                         DateTime.Now,       // OrderDate
                                         0,                  // OrderTotal
                                         false               // OrderStatus
                );

                MessageBox.Show("Order created successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                orderTableAdapter.FillByDESC(this.wstGrp22DataSet.Order);

                globalvar.orderID = dataGridView3.Rows[0].Cells[0].Value != null
                                        ? Convert.ToInt32(dataGridView3.Rows[0].Cells[0].Value)
                                        : 0;
                groupBox1.Visible = true;
                groupBox2.Visible = false;
                paymentTableAdapter.Insert(globalvar.orderID, false, 0, 0, 0);

                this.paymentTableAdapter.FillByDESC(this.wstGrp22DataSet.Payment);
                globalvar.paymentID = dataGridView4.Rows[0].Cells[0].Value != null
                                          ? Convert.ToInt32(dataGridView4.Rows[0].Cells[0].Value)
                                          : 0;
                globalvar.orderMessage = "Order ID: " + globalvar.orderID + "<br/>";

                globalvar.orderMessage +=
                    "<table border='1' cellspacing='0' cellpadding='6' style='border-collapse:collapse;font-family:Arial;font-size:12px;'>" +
                    "<tr>" + "<th>Product</th>" + "<th>Quantity</th>" + "<th>Price</th>" + "</tr>";
            }
            else
            {
                MessageBox.Show("Order creation cancelled.", "Cancelled", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

            // textBox1.Clear();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 1)
            {
                MessageBox.Show("Please add at least one product to the order before finalizing.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CreateOrder createOrder = new CreateOrder(this);
            createOrder.Show();
            this.Hide();
            createOrder.WindowState = FormWindowState.Maximized;
            createOrder.Label5.Text = "Client: " + globalvar.clientName;
            groupBox2.Visible = true;
            groupBox1.Visible = false;
            txtProdDesc.Clear();
            numericUpDown1.Value = 0;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Hide();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            clientTableAdapter.FillByName(this.wstGrp22DataSet.Client, textBox1.Text.Trim());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void CreateOrderRequest_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void CreateOrderRequest_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit? Any order in progress will be cancelled.",
                                         "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                orderTableAdapter.DeleteQuery(globalvar.orderID);
                clientOrderProductTableAdapter.DeleteQuery(globalvar.orderID);
                paymentTableAdapter.DeleteQuery(globalvar.orderID);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the order request?", "Cancel Order Request",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (dataGridView1.Rows.Count == 1)
                {
                    // MessageBox.Show("No products in the order to cancel.", "No Products", MessageBoxButtons.OK,
                    // MessageBoxIcon.Information); return;
                }
                orderTableAdapter.DeleteQuery(globalvar.orderID);
                clientOrderProductTableAdapter.DeleteQuery(globalvar.orderID);
                paymentTableAdapter.DeleteQuery(globalvar.orderID);

                dataGridView1.DataSource = null; // Clear the DataGridView
                // dataGridView1.DataSource = wstGrp22DataSet.ClientOrderProduct; // Rebind to the updated dataset
                dataGridView1.Rows.Clear(); // Clear all rows
                dataGridView1.Refresh();
                globalvar.priceTotal = 0; // Reset the total price
                MessageBox.Show("Order cancelled successfully.", "Order Cancelled", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                globalvar.orderID = 0;                // Reset the order ID
                globalvar.clientID = 0;               // Reset the client ID
                globalvar.clientName = string.Empty;  // Reset the client name
                globalvar.clientEmail = string.Empty; // Reset the client email

                txtProdDesc.Clear();
                numericUpDown1.Value = 0;
                groupBox1.Visible = false;
                groupBox2.Visible = true;
            }
        }
    }

    public static class globalvar
    {
        public static int loggedInAs = -1; // 0 = Sales Rep, 1 = Manager
        public static string userName = "";
        public static string userFirstName = "";
        public static string userSurname = "";
        public static string clientEmail = "";
        public static string clientName = "";
        public static string orderMessage = "";
        public static int orderID = 0;
        public static int clientID = 0;
        public static int productID = 0;
        public static int paymentID = 0;
        public static decimal priceTotal = 0;
    }
}
