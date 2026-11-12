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
    public partial class AddUser : Form
    {
        private Parent mainForm;

        public AddUser(Parent p)
        {
            InitializeComponent();
            mainForm = p;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Add new user record?", "New User", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
            {
                return; // User cancelled the operation
            }

            // Validate required fields
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("User Name, First Name, and Last Name are required.", "Input Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            userTableAdapter.Insert(textBox1.Text.Trim(),                     // UserName
                                    textBox2.Text.Trim(),                     // UserFirstName
                                    textBox3.Text.Trim(),                     // UserLastName
                                    false,                                    // UserType
                                    "12345",                                  // UserPassword
                                    false,                                    // UserOptOut
                                    comboBox1.SelectedItem.ToString().Trim(), // UserQuestion
                                    textBox4.Text.Trim()                      // UserAnswer
            );

            MessageBox.Show("New user record added successfully.", "Success", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

            // Clear input fields after successful insertion
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            comboBox1.SelectedIndex = -1;                          // Reset the combo box selection
            textBox1.Focus();                                      // Set focus back to the first input field
            this.userTableAdapter.Fill(this.wstGrp22DataSet.User); // Refresh the user data
        }

        private void AddUser_Load(object sender, EventArgs e)
        {
            this.userTableAdapter.Fill(this.wstGrp22DataSet.User);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            mainForm.Show();
            mainForm.Panel1.Visible = true;
            mainForm.MenuStrip1.Items[8].Visible = true;
        }
    }
}
