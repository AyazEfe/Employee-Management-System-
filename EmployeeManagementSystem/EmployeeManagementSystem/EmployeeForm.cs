using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class EmployeeForm : Form
    {
        private string _role;
        public EmployeeForm(string role)
        {
            InitializeComponent();
            _role = role;
            LoadEmployees();
            AdjustUIBasedOnRole();
        }
        private void AdjustUIBasedOnRole()
        {
            if (_role != "Admin")
            {
                btnAdd.Enabled = false;
                btnUpdate.Enabled = false;
                btnDelete.Enabled = false;
            }
        }
        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void LoadEmployees()
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Employees", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvEmployees.DataSource = dt;
            }
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                string query = "INSERT INTO Employees (Name, Position, Salary) VALUES (@Name, @Position, @Salary)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                cmd.Parameters.AddWithValue("@Salary", Convert.ToDecimal(txtSalary.Text));
                cmd.ExecuteNonQuery();
            }

            LoadEmployees();
            ClearInputs();
        }

       

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["Id"].Value);
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "UPDATE Employees SET Name=@Name, Position=@Position, Salary=@Salary WHERE Id=@Id";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", txtName.Text);
                    cmd.Parameters.AddWithValue("@Position", txtPosition.Text);
                    if (!decimal.TryParse(txtSalary.Text, out decimal salary))
                    {
                        MessageBox.Show("Please enter a valid salary (numbers only).");
                        return;
                    }

                    cmd.Parameters.AddWithValue("@Salary", salary);
                    cmd.ExecuteNonQuery();
                }

                LoadEmployees();
                ClearInputs();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dgvEmployees.SelectedRows[0].Cells["Id"].Value);
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    string query = "DELETE FROM Employees WHERE Id = @Id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }

                LoadEmployees();
                ClearInputs();
            }
        }
        private void dgvEmployees_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                txtName.Text = dgvEmployees.SelectedRows[0].Cells["Name"].Value.ToString();
                txtPosition.Text = dgvEmployees.SelectedRows[0].Cells["Position"].Value.ToString();
                txtSalary.Text = dgvEmployees.SelectedRows[0].Cells["Salary"].Value.ToString();
            }
        }


        private void ClearInputs()
        {
            txtName.Text = "";
            txtPosition.Text = "";
            txtSalary.Text = "";
        }

        
        
        
        
        
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtDepartment_TextChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            (dgvEmployees.DataSource as DataTable).DefaultView.RowFilter =
        string.Format("Name LIKE '%{0}%' OR Position LIKE '%{0}%'", txtSearch.Text);
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv";
                sfd.FileName = "Employees.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(sfd.FileName))
                        {
                            // Başlıklar
                            for (int i = 0; i < dgvEmployees.Columns.Count; i++)
                            {
                                sw.Write(dgvEmployees.Columns[i].HeaderText);
                                if (i < dgvEmployees.Columns.Count - 1)
                                    sw.Write(",");
                            }
                            sw.WriteLine();

                            // Satırlar
                            foreach (DataGridViewRow row in dgvEmployees.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    for (int i = 0; i < dgvEmployees.Columns.Count; i++)
                                    {
                                        sw.Write(row.Cells[i].Value?.ToString());
                                        if (i < dgvEmployees.Columns.Count - 1)
                                            sw.Write(",");
                                    }
                                    sw.WriteLine();
                                }
                            }
                        }

                        MessageBox.Show("Employees exported successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("No data to export!");
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }
    }
}
