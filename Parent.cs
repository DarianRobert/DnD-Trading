using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net.Sockets;

namespace DnD_Trading
{
    public partial class Parent : Form
    {
        public Panel Panel1;
        public MenuStrip MenuStrip1;

        public Parent()
        {
            InitializeComponent();
            Panel1 = this.panel1;
            MenuStrip1 = this.menuStrip1;
        }

        private void pbShowPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '\0';
            pbShowPassword.Image = Image.FromFile("HidePassword.png");
        }

        private void pbShowPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtPassword.PasswordChar = '*';
            pbShowPassword.Image = System.Drawing.Image.FromFile("ShowPassword.png");
        }

        private void orderToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void inventoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Product(this));
        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Reports(this));
        }

        private void clientsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void addClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new AddClient(this));
        }

        private void searchForClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Client(this));
        }

        private void searchForOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Order(this));
        }

        private void createOrderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new CreateOrder(this));
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                userTableAdapter.FillByUsernameAndPassword(wstGrp22DataSet.User, txtUsername.Text.Trim(),
                                                           txtPassword.Text.Trim());

                if (wstGrp22DataSet.User.Rows.Count > 0)
                {

                    if (Convert.ToInt32(wstGrp22DataSet.User.Rows[0]["UserOptOut"]) == 1)
                    {
                        MessageBox.Show("This user has opted out of the system. Please contact support.",
                                        "User Opted Out", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return; // Exit the login process if the user has opted out
                    }

                    menuStrip1.Visible = true;
                    menuStrip1.Items[0].Visible = false;

                    globalvar.loggedInAs =
                        Convert.ToInt32(wstGrp22DataSet.User.Rows[0]["UserType"]); // 0 = Sales Rep, 1 = Manager

                    // Access the specific row and column instead of using ItemArray
                    DataRow userRow = wstGrp22DataSet.User.Rows[0];
                    if (userRow["UserType"].ToString() == "False")
                    {
                        menuStrip1.Items[6].Visible = false;
                        menuStrip1.Items[7].Visible = false;
                    }
                    else
                    {
                        menuStrip1.Items[6].Visible = true;
                        menuStrip1.Items[7].Visible = true;
                    }

                    // User exists, proceed to the main application
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    globalvar.userName = txtUsername.Text.Trim();
                    globalvar.userFirstName = userRow["UserFirstName"].ToString();
                    globalvar.userSurname = userRow["UserLastName"].ToString();
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtUsername.Visible = false;
                    txtPassword.Visible = false;
                    btnLogin.Visible = false;
                    lblForgotPassword.Visible = false;
                    lblReEnter.Visible = false;
                    txtReEnterPassword.Visible = false;
                    btnChangePassword.Visible = false;
                    pbBack.Visible = false;
                    pbReEnterPassword.Visible = false;
                    label1.Visible = false;
                    label2.Visible = false;
                    pbShowPassword.Visible = false;
                    lblLogin.Visible = true;
                    lblLogin.Location = new Point(12, 9); // Adjust the position as needed

                    lblLogin.Text = "Welcome, " + globalvar.userFirstName + " " + globalvar.userSurname + "!";
                }
                else
                {
                    // User does not exist, show error message
                    MessageBox.Show("Invalid username or password.", "Login Error", MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login: " + ex.Message, "Login Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void lblForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            btnLogin.Visible = false;
            lblForgotPassword.Visible = false;
            lblReEnter.Visible = true;
            txtReEnterPassword.Visible = true;
            btnChangePassword.Visible = true;
            pbBack.Visible = true;
            pbReEnterPassword.Visible = true;
            txtUsername.Clear();
            txtPassword.Clear();
            txtReEnterPassword.Clear();
            label3.Visible = true;
            label4.Visible = true;
            textBox1.Visible = true;
            button2.Visible = true;
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtReEnterPassword.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter both username and new password.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            if (txtReEnterPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                MessageBox.Show("Passwords do not match. Please try again.", "Input Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Attempt to update and get number of affected rows
                int affectedRows =
                    userTableAdapter.UpdateQuery1(txtReEnterPassword.Text.Trim(), txtUsername.Text.Trim(),
                                                  textBox1.Text.Trim(), txtUsername.Text.Trim());

                if (affectedRows > 0)
                {
                    MessageBox.Show("Password changed successfully!", "Success", MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                    txtUsername.Clear();
                    txtPassword.Clear();
                    txtReEnterPassword.Clear();
                    lblReEnter.Visible = false;
                    txtReEnterPassword.Visible = false;
                    btnChangePassword.Visible = false;
                    pbReEnterPassword.Visible = false;
                    pbBack.Visible = false;
                    btnLogin.Visible = true;
                    lblForgotPassword.Visible = true;
                    label3.Visible = false;
                    label4.Visible = false;
                    textBox1.Visible = false;
                    button2.Visible = false;
                    textBox1.Clear();
                    label4.Text = "Verification Question:";
                }
                else
                {
                    MessageBox.Show("Incorrect username/verification question answer. Password not changed.", "Warning",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while changing the password.", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void Parent_Load(object sender, EventArgs e)
        {
            this.userTableAdapter.Fill(this.wstGrp22DataSet.User);
            this.WindowState = FormWindowState.Maximized; // Set the form to maximized state
        }

        private void pbBack_Click(object sender, EventArgs e)
        {
            pbBack.Visible = false;
            btnLogin.Visible = true;
            lblForgotPassword.Visible = true;
            lblReEnter.Visible = false;
            txtReEnterPassword.Visible = false;
            btnChangePassword.Visible = false;
            pbReEnterPassword.Visible = false;
            txtUsername.Clear();
            txtPassword.Clear();
            txtReEnterPassword.Clear();
            textBox1.Clear();
            label3.Visible = false;
            label4.Text = "Verification Question:";
            label4.Visible = false;
            textBox1.Visible = false;
            button2.Visible = false;
        }

        private void pbReEnterPassword_Click(object sender, EventArgs e)
        {
        }

        private void pbReEnterPassword_MouseDown(object sender, MouseEventArgs e)
        {
            txtReEnterPassword.PasswordChar = '\0';
            pbReEnterPassword.Image = Image.FromFile("HidePassword.png");
        }

        private void pbReEnterPassword_MouseUp(object sender, MouseEventArgs e)
        {
            txtReEnterPassword.PasswordChar = '*';
            pbReEnterPassword.Image = System.Drawing.Image.FromFile("ShowPassword.png");
        }

        private void supplierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Supplier(this));
        }

        private void createOrderRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new CreateOrderRequest(this));
        }

        private void paymentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new Payment(this));
        }

        private void ShowChildForm(Form child)
        {
            // Keep a single workspace open inside the main window.
            if (this.ActiveMdiChild != null)
            {
                this.ActiveMdiChild.Close();
            }
            menuStrip1.Items[8].Visible = false;
            child.MdiParent = this;
            child.WindowState = FormWindowState.Maximized;
            child.Show();
            panel1.Visible = false;
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            menuStrip1.Visible = false;
            txtUsername.Visible = true;
            txtPassword.Visible = true;
            btnLogin.Visible = true;
            lblForgotPassword.Visible = true;
            label1.Visible = true;
            label2.Visible = true;
            pbShowPassword.Visible = true;
            label3.Visible = false;
            globalvar.userName = string.Empty;      // Clear the global variable
            globalvar.userFirstName = string.Empty; // Clear the global variable
            globalvar.userSurname = string.Empty;   // Clear the global variable
            globalvar.clientID = 0;                 // Reset client ID
            globalvar.orderID = 0;                  // Reset order ID
            globalvar.paymentID = 0;                // Reset payment ID
            lblLogin.Visible = true;
            lblLogin.Text = "Login";
            label4.Text = "Verification Question:";
            lblLogin.Location = new Point(750, 150); // Adjust the position as needed
            lblReEnter.Visible = false;
            txtUsername.Clear();
            txtPassword.Clear();
            txtReEnterPassword.Clear();
            textBox1.Clear();
            globalvar.priceTotal = 0;  // Reset price total
            globalvar.productID = 0;   // Reset product ID
            globalvar.loggedInAs = -1; // Reset logged in status

            MessageBox.Show("You have been logged out successfully.", "Logout", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
        }

        private void addUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new AddUser(this));
        }

        private void searchForUsersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowChildForm(new User(this));
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                // Fill the DataGridView with users matching the entered username
                userTableAdapter.FillByUsername(wstGrp22DataSet.User, txtUsername.Text.Trim());
                // label4.Text = dataGridView1.Rows[0].Cells[6].Value.ToString();
                label4.Text = wstGrp22DataSet.User.Rows[0]["UserQuestion"].ToString();
            }
            catch (Exception ex)
            {
                // Handle the case where there are no rows or the cell is empty
                label4.Text = "Verification Question:";
            }
        }
    }
}
