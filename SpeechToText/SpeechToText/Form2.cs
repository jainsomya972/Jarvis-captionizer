﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace SpeechToText
{
    public partial class Form2 : Form
    {
        
        Form1 form1 = new Form1();
        public Form2()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            bunifuProgressBar1.Value += 1;
            if (bunifuProgressBar1.Value >= bunifuProgressBar1.MaximumValue )
            {
                timer1.Stop();
                form1.Show();
                this.Hide();
            }
        }
    }
}
