using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp10
{
    public partial class Form2: Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "admin" && textBox2.Text == "admin123")
            {       
                Form3 form3 = new Form3();          
                form3.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Kullanıcı adı veya şifre yanlış!");
            }
        }
    }
}
