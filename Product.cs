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
    public partial class Product : Form
    {
        public TabControl TabControl1 { get; set; }
        public ComboBox ComboBox1 { get; set; }

        private Parent mainForm;

        public Product(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private Supplier supplierForm;

        public Product(Supplier p)
        {
            InitializeComponent();
            supplierForm = p;
        }

        private CreateOrder createOrderForm;
        private bool isCreateOrderForm = false;

        public Product(CreateOrder p)
        {
            InitializeComponent();
            createOrderForm = p;
            isCreateOrderForm = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Product_Load(object sender, EventArgs e)
        {
            this.searchAddProductTableAdapter.Fill(this.wstGrp22DataSet.SearchAddProduct);

            this.supplierProductTableAdapter.Fill(this.wstGrp22DataSet.SupplierProduct);

            this.supplierTableAdapter.Fill(this.wstGrp22DataSet.Supplier);

            this.productTableAdapter.Fill(this.wstGrp22DataSet.Product);

            TabControl1 = this.tabControl1;
            ComboBox1 = this.comboBox1;

            comboBox1.SelectedIndex = -1;
            cmbSupplier.SelectedIndex = -1;

            if (globalvar.loggedInAs == 0)
            {
                this.groupBox3.Visible = false;
            }
            else
            {
                this.groupBox3.Visible = true;
            }
        }

        private void txtProduct_TextChanged(object sender, EventArgs e)
        {
            cmbSupplier.Text = "";
            // Filter the Product table based on the text entered in txtProduct
            if (string.IsNullOrWhiteSpace(txtProduct.Text))
            {
                // If the text box is empty, reload all products
                searchAddProductTableAdapter.Fill(this.wstGrp22DataSet.SearchAddProduct);
                return;
            }

            searchAddProductTableAdapter.FillByProductLIKE(this.wstGrp22DataSet.SearchAddProduct,
                                                           txtProduct.Text.Trim());
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(txtSurcharge.Text, out decimal surcharge))
            {
                productTableAdapter.UpdateQuery(surcharge, globalvar.productID);
                MessageBox.Show("Surcharge updated successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                txtSurcharge.Clear();
            }
            else
            {
                MessageBox.Show("Please enter a valid decimal value for the surcharge.", "Invalid Input",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            productTableAdapter.Fill(this.wstGrp22DataSet.Product);
        }

        private void cmbSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtProduct.Text = "";
            searchAddProductTableAdapter.FillBySupplier(this.wstGrp22DataSet.SearchAddProduct, cmbSupplier.Text.Trim());
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isCreateOrderForm)
            {
                createOrderForm.Show();
                this.Hide();
                createOrderForm.WindowState = FormWindowState.Maximized;
                return;
            }
            this.Hide();
            mainForm.Show();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.Rows[e.RowIndex].IsNewRow)
                return;
            globalvar.productID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            cmbSupplier.DataSource = null;
            cmbSupplier.DisplayMember = "";

            string productName = textBox1.Text.Trim();
            string supplierName = comboBox1.Text.Trim();
            decimal surcharge;
            decimal price;

            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(supplierName))
            {
                MessageBox.Show("Product name and supplier name cannot be empty.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            searchAddProductTableAdapter.FillByProductAndSupplier(this.wstGrp22DataSet.SearchAddProduct, productName,
                                                                  supplierName);

            if (wstGrp22DataSet.SearchAddProduct.Rows.Count > 0)
            {
                MessageBox.Show("This product already exists in the database.", "Duplicate Product",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            productTableAdapter.FillByProduct(wstGrp22DataSet.Product, productName);

            if (wstGrp22DataSet.Product.Rows.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(textBox4.Text.Trim()))
                {
                    MessageBox.Show(
                        "This product already exists in the database. Do not enter a value in the surcharge field.",
                        "Duplicate Product", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(textBox3.Text.Trim(), out price))
                {
                    MessageBox.Show("Please enter a valid decimal value for the price.", "Invalid Input",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productTableAdapter.FillByProduct(wstGrp22DataSet.Product, productName);
                int productID = Convert.ToInt32(wstGrp22DataSet.Product.Rows[0]["ProductID"]);

                supplierTableAdapter.FillBySupplierName(this.wstGrp22DataSet.Supplier, supplierName);
                // Get the supplier ID based on the supplier name
                int supplierID = Convert.ToInt32(wstGrp22DataSet.Supplier.Rows[0]["SupplierID"]);

                supplierProductTableAdapter.Insert(productID,  // find matching product ID
                                                   supplierID, // find matching supplier ID
                                                   price);
            }
            else
            {
                if (!decimal.TryParse(textBox4.Text.Trim(), out surcharge))
                {
                    MessageBox.Show("Please enter a valid decimal value for the surcharge.", "Invalid Input",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!decimal.TryParse(textBox3.Text.Trim(), out price))
                {
                    MessageBox.Show("Please enter a valid decimal value for the price.", "Invalid Input",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                productTableAdapter.Insert(productName, surcharge);
                productTableAdapter.FillByOrderIDDESC(wstGrp22DataSet.Product);
                // Get the last inserted product ID
                int lastInsertedProductID = Convert.ToInt32(wstGrp22DataSet.Product.Rows[0]["ProductID"]);
                supplierTableAdapter.FillBySupplierName(this.wstGrp22DataSet.Supplier, supplierName);
                // Get the supplier ID based on the supplier name
                int supplierID = Convert.ToInt32(wstGrp22DataSet.Supplier.Rows[0]["SupplierID"]);

                supplierProductTableAdapter.Insert(lastInsertedProductID, // get last inserted product ID
                                                   supplierID,            // find matching supplier ID
                                                   price);
            }

            MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Optionally, you can clear the input fields after adding
            textBox1.Clear();
            comboBox1.SelectedIndex = -1;
            textBox3.Clear();
            textBox4.Clear();

            if (isCreateOrderForm)
            {
                createOrderForm.Show();
                this.Hide();
                createOrderForm.WindowState = FormWindowState.Maximized;
            }

            supplierTableAdapter.Fill(this.wstGrp22DataSet.Supplier);
            productTableAdapter.Fill(this.wstGrp22DataSet.Product);
            searchAddProductTableAdapter.Fill(this.wstGrp22DataSet.SearchAddProduct);

            cmbSupplier.DataSource = supplierBindingSource;
            cmbSupplier.DisplayMember = "SupplierName";
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (isCreateOrderForm)
            {
                createOrderForm.Show();
                this.Hide();
                createOrderForm.WindowState = FormWindowState.Maximized;
                return;
            }
            this.Hide();
            mainForm.Show();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            productTableAdapter.FillByProduct(this.wstGrp22DataSet.Product, textBox2.Text.Trim());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            txtProduct.Text = "";
            cmbSupplier.SelectedIndex = -1;
            this.searchAddProductTableAdapter.Fill(this.wstGrp22DataSet.SearchAddProduct);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Supplier supplier = new Supplier(this);
            supplier.Show();
            this.Hide();
            supplier.WindowState = FormWindowState.Maximized;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                searchAddProductTableAdapter.Fill(this.wstGrp22DataSet.SearchAddProduct);
                return;
            }
            string productName = textBox1.Text.Trim();
            searchAddProductTableAdapter.FillByProduct(this.wstGrp22DataSet.SearchAddProduct, productName);
        }

        private void cmbSupplier_TextChanged(object sender, EventArgs e)
        {
            txtProduct.Text = "";
            searchAddProductTableAdapter.FillBySupplier(this.wstGrp22DataSet.SearchAddProduct, cmbSupplier.Text.Trim());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            productTableAdapter.Fill(this.wstGrp22DataSet.Product);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cmbSupplier.DataSource = null;
            cmbSupplier.DisplayMember = "";
            if (dataGridView2.CurrentRow == null || dataGridView2.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Please select a supplier product to update.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }
            string productName = dataGridView2.CurrentRow.Cells[0].Value.ToString();
            string supplierName = dataGridView2.CurrentRow.Cells[1].Value.ToString();

            productTableAdapter.FillByProduct(wstGrp22DataSet.Product, productName);
            // Get the last inserted product ID
            int productID = Convert.ToInt32(wstGrp22DataSet.Product.Rows[0]["ProductID"]);

            supplierTableAdapter.FillBySupplierName(this.wstGrp22DataSet.Supplier, supplierName);
            // Get the supplier ID based on the supplier name
            int supplierID = Convert.ToInt32(wstGrp22DataSet.Supplier.Rows[0]["SupplierID"]);

            if (decimal.TryParse(textBox5.Text.Trim(), out decimal price))
            {
                supplierProductTableAdapter.UpdatePrice(price, productID, supplierID);
                MessageBox.Show("Price updated successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                textBox5.Clear();
            }
            else
            {
                MessageBox.Show("Invalid price value. Please enter a valid decimal value.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            searchAddProductTableAdapter.Fill(wstGrp22DataSet.SearchAddProduct);
            supplierTableAdapter.Fill(this.wstGrp22DataSet.Supplier);
            productTableAdapter.Fill(this.wstGrp22DataSet.Product);

            cmbSupplier.DataSource = supplierBindingSource;
            cmbSupplier.DisplayMember = "SupplierName";
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void dataGridView2_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView2.Rows[e.RowIndex].IsNewRow)
                return;
            textBox1.Text = Convert.ToString(dataGridView2.Rows[e.RowIndex].Cells[0].Value);
        }

        private void lblProduct_Click(object sender, EventArgs e)
        {
        }
    }
}
