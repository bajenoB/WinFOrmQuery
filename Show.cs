using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp23 {
    public partial class Show : Form {
        public Show() {
            InitializeComponent();
        }

        public Show(string tableName, string text) {
            InitializeComponent();
            this.label1.Text = tableName;
            this.textBox1.Text = text;
        }
    }
}
