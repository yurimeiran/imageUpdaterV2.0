using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace imageUpdaterV2._0
{
    public partial class loginDialog : Form
    {

        db dbConn;

        public loginDialog()
        {
            InitializeComponent();
            dbConn = new db();
        }

        //this is a cancel button
        //on click close the window
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //this is login button
        //on click -> check login details-> do what user wanted(delete
        private void button1_Click(object sender, EventArgs e)
        {
            var username = textBox1.Text;
            var password = textBox2.Text;


        }

       
    }
}
