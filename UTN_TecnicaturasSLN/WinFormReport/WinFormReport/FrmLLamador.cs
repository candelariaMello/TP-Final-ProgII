﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormReport
{
    public partial class FrmLLamador : Form
    {
        public FrmLLamador()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new WinNetFramework.FrmReportMaterias().Show();
        }

        private void btnRptAlumnos_Click(object sender, EventArgs e)
        {
            new WinNetFramework.FrmReportAlumnos().Show();
        }

        private void FrmLLamador_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}
