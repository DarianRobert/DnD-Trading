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
    public partial class Order : Form
    {
        private Parent mainForm;

        public Order(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Hide();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void Order_Load(object sender, EventArgs e)
        {
            // this.clientClientOrderProductOrder1TableAdapter.Fill(this.wstGrp22DataSet1.ClientClientOrderProductOrder1);
            this.viewOrderProduct1TableAdapter.Fill(this.wstGrp22DataSet3.ViewOrderProduct1);
            this.viewOrderProductTableAdapter.Fill(this.wstGrp22DataSet1.ViewOrderProduct);
            this.clientClientOrderProductOrderTableAdapter.Fill(this.wstGrp22DataSet1.ClientClientOrderProductOrder);
            this.userTableAdapter.Fill(this.wstGrp22DataSet2.User);
            this.orderTableAdapter.Fill(this.wstGrp22DataSet2.Order);
            this.clientOrderTableAdapter.Fill(this.wstGrp22DataSet1.ClientOrder);
            this.orderTableAdapter.Fill(this.wstGrp22DataSet.Order);
            comboBox2.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            clientClientOrderProductOrderTableAdapter.FillByClientName(
                this.wstGrp22DataSet1.ClientClientOrderProductOrder, textBox1.Text.Trim());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            clientClientOrderProductOrderTableAdapter.FillBySalesRep(
                this.wstGrp22DataSet1.ClientClientOrderProductOrder, comboBox2.Text.Trim());
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.clientOrderTableAdapter.Fill(this.wstGrp22DataSet1.ClientOrder);
            clientClientOrderProductOrderTableAdapter.Fill(this.wstGrp22DataSet1.ClientClientOrderProductOrder);
            textBox1.Clear();
            comboBox2.Text = "";
            dataGridView3.Visible = false;
            dataGridView1.Refresh();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is the checkbox column (index 4)
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                // Force the current edit (checkbox) to commit
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);

                int orderID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value);
                bool isOptOut = (bool)dataGridView1.Rows[e.RowIndex].Cells[5].Value;

                orderTableAdapter.UpdateStatus(isOptOut, orderID);

                MessageBox.Show("Order status updated successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void dataGridView1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView3.Visible = true;

            // wstGrp22DataSet1.EnforceConstraints = false;

            // viewOrderProductTableAdapter.FillByOrderID(this.wstGrp22DataSet1.ViewOrderProduct,
            // Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));

            // MessageBox.Show("Order ID: " + dataGridView1.Rows[e.RowIndex].Cells[0].Value);

            // viewOrderProduct1TableAdapter.Fill(this.wstGrp22DataSet1.ViewOrderProduct1);

            viewOrderProduct1TableAdapter.FillByOrderID(this.wstGrp22DataSet3.ViewOrderProduct1,
                                                        Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value));

            // wstGrp22DataSet1.EnforceConstraints = true;
        }
    }
}
