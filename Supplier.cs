using DnD_Trading.WstGrp22DataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DnD_Trading
{
    public partial class Supplier : Form
    {
        private Parent mainForm;

        private Product productForm;

        private bool isMainForm = false;
        private bool isProductForm = false;

        public Supplier(Parent p)
        {
            InitializeComponent();
            mainForm = p;
            isMainForm = true;
        }

        public Supplier(Product p)
        {
            InitializeComponent();
            productForm = p;
            isProductForm = true;
        }

        private void Supplier_Load(object sender, EventArgs e)
        {
            this.supplierTableAdapter.Fill(this.wstGrp22DataSet.Supplier);
        }

        private void txtSupplier_TextChanged(object sender, EventArgs e)
        {
            supplierTableAdapter.FillBySupplierName(this.wstGrp22DataSet.Supplier, txtSupplier.Text.Trim());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string supplierName = txtSupplierName.Text.Trim();
            string phoneNumber = txtSupplierPhoneNumber.Text.Trim();
            string email = txtSupplierEmail.Text.Trim();

            // Validate Supplier Name
            if (string.IsNullOrEmpty(supplierName))
            {
                MessageBox.Show("Supplier name is required.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Validate Phone Number, only digits
            if (!IsValidPhoneNumber(phoneNumber))
            {
                MessageBox.Show("Phone number must be 10 digits", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Validate Email using MailAddress
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email address.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert the supplier data
            supplierTableAdapter.Insert(supplierName, // SupplierName
                                        phoneNumber,  // SupplierPhoneNumber
                                        email,        // SupplierEmail
                                        false         // SupplierOptOut
            );

            MessageBox.Show("Supplier added successfully!", "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            this.supplierTableAdapter.Fill(this.wstGrp22DataSet.Supplier);

            if (isProductForm)
            {
                productForm.Show();
                this.Hide();
                productForm.WindowState = FormWindowState.Maximized;
            }

            if (productForm != null)
            {
                productForm.ComboBox1.DataSource = supplierTableAdapter.GetData();
                productForm.ComboBox1.DisplayMember = "SupplierName";
            }

            // Clear input fields
            txtSupplierName.Clear();
            txtSupplierPhoneNumber.Clear();
            txtSupplierEmail.Clear();
        }

        bool IsValidPhoneNumber(string number)
        {
            if (number.Length != 10)
                return false;

            foreach (char c in number)
            {
                if (!char.IsDigit(c))
                    return false;
            }

            return true;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (isMainForm)
            {
                mainForm.Show();
                this.Hide();
                mainForm.Panel1.Visible = true;
                mainForm.MenuStrip1.Items[8].Visible = true;
            }
            else if (isProductForm)
            {
                productForm.Show();
                this.Hide();
                productForm.WindowState = FormWindowState.Maximized;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Product productForm = new Product(this);
            productForm.Show();
            productForm.WindowState = FormWindowState.Maximized;
            productForm.TabControl1.SelectedIndex = 1; // Switch to the first tab
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is the checkbox column (index 4)
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                // Force the current edit (checkbox) to commit
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int supplierID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                bool isOptOut = (bool)dataGridView1.Rows[e.RowIndex].Cells[4].Value;

                supplierTableAdapter.UpdateStatus(isOptOut, supplierID);
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
    }
}
