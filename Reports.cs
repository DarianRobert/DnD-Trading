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
    public partial class Reports : Form
    {
        public bool backButton = false;

        private Parent mainForm;

        public Reports(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (backButton == false)
            {
                mainForm.Show();
                this.Hide();
                mainForm.Panel1.Visible = true;
                mainForm.MenuStrip1.Items[8].Visible = true;
            }
            else if (backButton)
            {
                crystalReportViewer1.Visible = false;
                crystalReportViewer2.Visible = false;
                crystalReportViewer3.Visible = false;
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                backButton = false;
            }
        }

        private void Reports_Load(object sender, EventArgs e)
        {
            this.clientTableAdapter.Fill(this.wstGrp22DataSet.Client);
            this.orderTableAdapter.Fill(this.wstGrp22DataSet.Order);
            this.orderSupplierProductTableAdapter.Fill(this.wstGrp22DataSet.OrderSupplierProduct);
            this.productTableAdapter.Fill(this.wstGrp22DataSet.Product);
            this.paymentTableAdapter.Fill(this.wstGrp22DataSet.Payment);

            crystalReport11.SetDataSource(this.wstGrp22DataSet);
            CrystalReport21.SetDataSource(this.wstGrp22DataSet);
            CrystalReport41.SetDataSource(this.wstGrp22DataSet);
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backButton = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            crystalReportViewer1.Visible = true;
            crystalReportViewer1.Zoom(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            backButton = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            crystalReportViewer2.Visible = true;
            crystalReportViewer2.Zoom(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            backButton = true;
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            crystalReportViewer3.Visible = true;
            crystalReportViewer3.Zoom(1);
        }
    }
}
