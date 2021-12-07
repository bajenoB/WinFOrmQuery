using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace WindowsFormsApp23 {
    public partial class Form1 : Form {
        private string dbName = string.Empty;
        private string tableName = string.Empty;
        SqlConnection sqlConnection = new SqlConnection();
        public Form1() {
            InitializeComponent();
        }

        private void Connection(string connectionstr) {
            if (sqlConnection.State == ConnectionState.Open) {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
        }


        private void button1_Click(object sender, EventArgs e) {
            //string conString = $"server{textBox1.Text};uid={textBox2.Text};pwd={textBox3.Text};database={textBox4.Text}";

            try {
                Connection($@"Data Source={this.textBox1.Text};Initial Catalog={this.textBox4.Text};User Id={this.textBox2.Text};Password={this.textBox3.Text};Integrated Security=False;");
                MessageBox.Show("Harow!");
                this.treeView1.Nodes.Clear();
                using (SqlCommand command = new SqlCommand(@"SELECT name FROM sys.databases;", sqlConnection)) {
                    using (SqlDataReader reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            object name = reader.GetValue(0);
                            this.treeView1.Nodes.Add(name.ToString());
                        }
                    }
                }


            }
            catch (Exception ex) {
                MessageBox.Show("Error connection");
            }

        }

        private void TreeView1_AfterSelect1(object sender, System.Windows.Forms.TreeViewEventArgs e) {
            dbName = string.Empty;
            tableName = string.Empty;
            if ((sender as TreeView).SelectedNode.Parent != null &&
     (sender as TreeView).SelectedNode.GetType() == typeof(TreeNode)) {
                dbName = (sender as TreeView).SelectedNode.Parent.Text;
                tableName = (sender as TreeView).SelectedNode.Text;
            }
            else {
                dbName = (sender as TreeView).SelectedNode.Text;
            }
            if (isDatabaseExists(dbName)) {
                using (SqlConnection connection = new SqlConnection(($@"Data Source={this.textBox1.Text};Initial Catalog={textBox4.Text};UID={this.textBox2.Text};Password={this.textBox3.Text}"))) ;
                (sender as TreeView).SelectedNode.Nodes.Clear();
                foreach (var item in Select(dbName, tableName)) {
                    (sender as TreeView).SelectedNode.Nodes.Add(item);
                }
            }
        }

        private List<string> Select(string dbname, string tableName) {
            List<string> list = new List<string>();
            string commandstring = string.Empty;
            try {
                if (tableName != string.Empty)
                    commandstring = Command(tableName);
                else
                    commandstring = Command(dbname);

                if (commandstring != string.Empty) {
                    using (SqlCommand command = new SqlCommand(commandstring, sqlConnection)) {
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                object names = reader.GetValue(0);
                                list.Add(names.ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return list;
        }

        private string Command(string name) {
            if (isDatabaseExists(name)) {
                return $@"SELECT name FROM [{name}].sys.tables;";
            }
            else {
                return $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{name}';";
            }
        }

        private bool isDatabaseExists(string name) {
            using (SqlCommand command = new SqlCommand($@"if Exists(select 1 from master.dbo.sysdatabases where name='{name}') 
                       select 1 else select 0", sqlConnection)) {
                int exists = Convert.ToInt32(command.ExecuteScalar());

                if (exists > 0) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            string data = string.Empty;
            if (tableName != string.Empty) {
                if (isDatabaseExists(dbName)) {
                    using (SqlCommand command = new SqlCommand($@"SELECT * FROM [{tableName}]", sqlConnection)) {
                        using (SqlDataReader reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                for (int i = 0; i < reader.FieldCount; i++) {
                                    data += $"{reader.GetName(i)}:  {reader.GetValue(i)} \r\n";
                                }
                                data += "\r\n";
                            }
                        }
                    }
                    this.Hide();
                    Show tableData = new Show(tableName, data);
                    tableData.ShowDialog();
                    this.Show();
                }
            }
        }
    }
}
