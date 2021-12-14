using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WindowsFormsApp23 {
    public partial class Form1 : Form
    {
        private string server = string.Empty;
        private string user = string.Empty;
        private string password = string.Empty;
        private string dbName = string.Empty;
        private string tableName = string.Empty;
        SqlConnection sqlConnection = new SqlConnection();
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(SqlConnection connection, string server, string user, string password)
        {
            InitializeComponent();
            this.sqlConnection = connection;
            this.server = server;
            this.user = user;
            this.password = password;
            FillToolStrip();
        }
        private void Connection(string connectionstr)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
            sqlConnection = new SqlConnection(connectionstr);
            sqlConnection.Open();
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            dbName = string.Empty;
            tableName = string.Empty;
            if ((sender as TreeView).SelectedNode.Parent != null &&
     (sender as TreeView).SelectedNode.GetType() == typeof(TreeNode))
            {
                dbName = (sender as TreeView).SelectedNode.Parent.Text;
                tableName = (sender as TreeView).SelectedNode.Text;
            }
            else
            {
                dbName = (sender as TreeView).SelectedNode.Text;
            }
            if (isDatabaseExists(dbName))
            {
                Connection($@"Data Source={server};Initial Catalog={dbName};UID={user};Password={password}");
                (sender as TreeView).SelectedNode.Nodes.Clear();
                foreach (var item in Select(dbName, tableName))
                {
                    (sender as TreeView).SelectedNode.Nodes.Add(item);
                }
            }
        }
        private List<string> Select(string dbname, string tableName)
        {
            List<string> list = new List<string>();
            string commandstring = string.Empty;
            try
            {
                if (tableName != string.Empty)
                    commandstring = Command(tableName);
                else
                    commandstring = Command(dbname);

                if (commandstring != string.Empty)
                {
                    using (SqlCommand command = new SqlCommand(commandstring, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
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
        private string Command(string name)
        {
            if (isDatabaseExists(name))
            {
                return $@"SELECT name FROM [{name}].sys.tables;";
            }
            else
            {
                return $@"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{name}';";
            }
        }
        private bool isDatabaseExists(string name)
        {
            using (SqlCommand command = new SqlCommand($@"if Exists(select 1 from master.dbo.sysdatabases where name='{name}') 
                       select 1 else select 0", sqlConnection))
            {
                int exists = Convert.ToInt32(command.ExecuteScalar());

                if (exists > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => LoadData());
        }
        private void LoadData()
        {
            this.treeView1.Invoke((MethodInvoker)(() => this.treeView1.Nodes.Clear()));
            using (SqlCommand command = new SqlCommand(@"SELECT name FROM sys.databases;", sqlConnection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        object name = reader.GetValue(0);
                        this.treeView1.Invoke((MethodInvoker)(() => this.treeView1.Nodes.Add(name.ToString())));
                    }
                }
            }
        }
        private void FillToolStrip()
        {
            ToolStripMenuItem procItem = new ToolStripMenuItem("Proc");
            procItem.DropDownItems.Add("Select");
            procItem.DropDownItems[0].Click += Select_Click;
            procItem.DropDownItems.Add("Insert");
            procItem.DropDownItems[1].Click += Insert_Click;
            procItem.DropDownItems.Add("Delete data");
            procItem.DropDownItems[2].Click += Delete_Click;

            ToolStripMenuItem tableItem = new ToolStripMenuItem("Table");
            tableItem.DropDownItems.Add("Create Table");
            tableItem.DropDownItems[0].Click += Create_Click;
            tableItem.DropDownItems.Add("Drop Table");
            tableItem.DropDownItems[1].Click += Drop_Click;


            this.menuStrip1.Items.Add(procItem);
            this.menuStrip1.Items.Add(tableItem);
        }
        private void Drop_Click(object sender, EventArgs e) => Proc("Delete Table");
        private void Create_Click(object sender, EventArgs e) => Table("Create Table");
        private void Delete_Click(object sender, EventArgs e) => Proc("Delete");
        private void Insert_Click(object sender, EventArgs e) => Proc("Insert");
        private void Table(string action)
        {
            if (isDatabaseExists(dbName))
            {
                this.Hide();
                EditForm editForm = new EditForm(action, tableName, sqlConnection);
                editForm.ShowDialog();
                Task.Factory.StartNew(() => LoadData());
                this.Show();
            }
            else
                MessageBox.Show("Select table or database!");
        }
        private void Proc(string action)
        {
            if (tableName != string.Empty)
            {
                if (isDatabaseExists(dbName))
                {
                    this.Hide();
                    EditForm editForm = new EditForm(action, tableName, sqlConnection);
                    editForm.ShowDialog();
                    Task.Factory.StartNew(() => LoadData());
                    this.Show();
                }
                else
                    MessageBox.Show("Select table!");
            }
            else
                MessageBox.Show("Select table!");
        }

        private void Select_Click(object sender, EventArgs e)
        {
            string data = string.Empty;
            if (tableName != string.Empty)
            {
                if (isDatabaseExists(dbName))
                {
                    using (SqlCommand command = new SqlCommand($@"SELECT * FROM [{tableName}]", sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
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
            else
                MessageBox.Show("Select table!");
        }
    }
}
