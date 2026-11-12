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

namespace DnD_Trading
{
    public partial class AddClient : Form
    {
        private Parent mainForm;

        public AddClient(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Add new client record?", "New Client", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                string firstName = txtFirstName.Text.Trim();
                string surname = txtSurname.Text.Trim();
                string phoneNumber = txtPhoneNumber.Text.Trim();
                string email = txtEmail.Text.Trim();

                // Validate required fields
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname))
                {
                    MessageBox.Show("First Name and Surname are required.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Validate phone number: only digits
                if (!IsValidPhoneNumber(phoneNumber))
                {
                    MessageBox.Show("Phone number must be 10 digits.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Validate email using System.Net.Mail.MailAddress
                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Invalid email address.", "Input Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                // Proceed with adding the client
                clientTableAdapter.Insert(firstName + " " + surname, // ClientName
                                          phoneNumber,               // ClientPhoneNumber
                                          email,                     // ClientEmail
                                          false                      // ClientOptOut
                );

                MessageBox.Show("Client added successfully!", "Success", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                // Clear input fields after successful addition
                txtFirstName.Clear();
                txtSurname.Clear();
                txtPhoneNumber.Clear();
                txtEmail.Clear();
            }

            this.clientTableAdapter.Fill(this.wstGrp22DataSet.Client);
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
            if (string.IsNullOrWhiteSpace(email))
                return false;
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void AddClient_Load(object sender, EventArgs e)
        {
            this.clientTableAdapter.Fill(this.wstGrp22DataSet.Client);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            mainForm.Show();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }
    }
}
