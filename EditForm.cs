using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp23
{
    public partial class EditForm : Form
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        string name = string.Empty;
        string actionName = string.Empty;
        SqlConnection sqlConnection = new SqlConnection();
        public EditForm()
        {
            InitializeComponent();
        }

        public EditForm(string action, string tableName, SqlConnection connection)
        {
            InitializeComponent();
            name = tableName;
            actionName = action;
            this.label.Text = $"{tableName}";
            sqlConnection = connection;
            Start();
        }
        private void submitBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(this.textBox.Text, sqlConnection))
                {
                    command.ExecuteNonQuery();
                    MessageBox.Show("Succes!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Start()
        {
            switch (actionName)
            {
                case "Insert":
                    Insert();
                    break;
                case "Delete":
                    Delete();
                    break;
                case "Create Table":
                    CreateTable();
                    break;
                case "Delete Table":
                    DeleteTable();
                    break;
                default:
                    break;
            }
        }
        private void Insert()
        {
            this.textBox.Text = $@"INSERT INTO [{name}](";
            using (SqlCommand command = new SqlCommand($@"SELECT * FROM [{name}]", sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    for (int i = 1; i < reader.FieldCount; i++)
                    {
                        this.textBox.Text += $"{reader.GetName(i)},";
                    }
                }
            }
            this.textBox.Text = this.textBox.Text.Remove(this.textBox.Text.Length - 1, 1);
            this.textBox.Text += ") VALUES(value);";
        }
        private void Delete()
        {
            this.textBox.Text = $"DELETE FROM [{name}] WHERE [COLUMN_NAME]=VALUE;";
        }
        private void CreateTable()
        {
            this.textBox.Text = "CREATE TABLE [NAME]\r\n(\r\n[id] INT IDENTITY PRIMARY KEY,\r\n);";
        }
        private void DeleteTable()
        {
            this.textBox.Text = $"DROP TABLE [{this.name}];";
        }
    }
}
