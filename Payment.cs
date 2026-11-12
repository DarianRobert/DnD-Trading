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
using DnD_Trading.WstGrp22DataSetTableAdapters;

namespace DnD_Trading
{
    public partial class Payment : Form
    {
        private Parent mainForm;

        public Payment(Parent p)
        {
            mainForm = p;
            InitializeComponent();
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            this.paymentFormTableAdapter.Fill(this.wstGrp22DataSet.PaymentForm);
            this.paymentTableAdapter.Fill(this.wstGrp22DataSet.Payment);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Hide();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                paymentFormTableAdapter.FillByStatusTrue(this.wstGrp22DataSet.PaymentForm);
            }
            else
            {
                paymentFormTableAdapter.FillByStatusFalse(this.wstGrp22DataSet.PaymentForm);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.paymentFormTableAdapter.Fill(this.wstGrp22DataSet.PaymentForm);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate that the payment amount is a positive number
                if (Convert.ToDecimal(textBox1.Text.Trim()) <= 0)
                {
                    MessageBox.Show("Payment amount must be a positive number.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DialogResult result =
                MessageBox.Show("Update payment status?", "Update Payment", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
            {
                MessageBox.Show("Payment update cancelled.", "Cancelled", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                return;
            }

            // Validate that a row is selected
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Please select a payment to update.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (globalvar.paymentID == 0)
            {
                MessageBox.Show("Please select a payment to update.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // Validate that the status is not already true
            if (Convert.ToBoolean(dataGridView1.CurrentRow.Cells[6].Value))
            {
                MessageBox.Show("This payment is already marked as paid.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (Convert.ToDecimal(textBox1.Text.Trim()) >= Convert.ToDecimal(dataGridView1.CurrentRow.Cells[4].Value))
            {
                decimal surplus = Convert.ToDecimal(textBox1.Text.Trim()) -
                                  Convert.ToDecimal(dataGridView1.CurrentRow.Cells[4].Value);

                paymentTableAdapter.UpdatePayment(true, 0, surplus, globalvar.paymentID);

                MessageBox.Show("Payment status updated successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
            else
            {
                // Update the payment status
                paymentTableAdapter.UpdatePayment(false,
                                                  Convert.ToDecimal(dataGridView1.CurrentRow.Cells[4].Value) -
                                                      Convert.ToDecimal(textBox1.Text.Trim()),
                                                  0, globalvar.paymentID);

                MessageBox.Show("Payment status updated successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

            paymentFormTableAdapter.Fill(this.wstGrp22DataSet.PaymentForm);

            textBox1.Clear();
            textBox2.Clear();

            globalvar.paymentID = 0;
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            globalvar.paymentID = Convert.ToInt32(dataGridView1.CurrentRow.Cells[2].Value);

            // MessageBox.Show("Payment ID: " + globalvar.paymentID, "Payment Selected", MessageBoxButtons.OK,
            // MessageBoxIcon.Information);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Validate that the input is not empty
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                paymentFormTableAdapter.Fill(this.wstGrp22DataSet.PaymentForm);
                return;
            }

            // Fill the PaymentForm table with the filtered data based on the client name

            paymentFormTableAdapter.FillByClientName(this.wstGrp22DataSet.PaymentForm, textBox2.Text.Trim());
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }
    }
}
