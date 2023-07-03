﻿using DLL_API.Dominio;
using FrontTenicaturas.Servicios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace FrontTecnicaturas.Presentacion
{
    public partial class FrmTecnicaturasBaja : Form
    {
        List<Tecnicatura> lTecnicatura;

        public FrmTecnicaturasBaja()
        {
            InitializeComponent();
            CenterToParent();
            lTecnicatura=new List<Tecnicatura>();
        }

        private async void FrmTecnicaturasBaja_Load(object sender, EventArgs e)
        {
            await CargarTecnicaturasAsync();

        }

        private async Task CargarTecnicaturasAsync()
        {
            lstTecBajas.Items.Clear();
            lTecnicatura.Clear();
            string url = "http://localhost:5241/tecnicaturas/verBajas";
            var data = await ClienteSingleton.GetInstance().GetAsync(url);
#pragma warning disable CS8600 
            List<Tecnicatura> lst = JsonConvert.DeserializeObject<List<Tecnicatura>>(data);
#pragma warning restore CS8600
            if (lst != null)
            {
                lTecnicatura = lst;
                foreach (Tecnicatura t in lTecnicatura)
                {
                    lstTecBajas.Items.Add(t.Nombre);
                }
            }
            
            else
            {
                MessageBox.Show("No existen tecnicaturas anteriores","Tecnicaturas anteriores",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lstTecBajas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTecBajas.SelectedIndex > -1)
            {
                Limpiar();
                CargarDatos(lstTecBajas.SelectedIndex);
                Habilitar(true);
            }
        }
        private void Habilitar(bool x)
        {
            btnAlta.Enabled = x;
            btnCancelar.Enabled = x;
        }

        private void CargarDatos(int index)
        {
            foreach (Tecnicatura c in lTecnicatura)
            {
                txtNombre.Text = lTecnicatura[index].Nombre;
                txtTitulo.Text = lTecnicatura[index].Titulo;

            }
            foreach (DetalleTecnicatura dc in lTecnicatura[index].Detalles)
            {
                int anio = dc.AñoCursado;
                int cuatr = dc.Cuatrimestre;
                string asig = dc.Materia.Nombre;
                int id = dc.Materia.Id;
                dgvMaterias.Rows.Add(new object[] { id, asig, cuatr, anio });
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Habilitar(false);
            Limpiar();
            lstTecBajas.SelectedIndex = -1;
        }
        private void Limpiar()
        {
            dgvMaterias.Rows.Clear();
            txtNombre.Text = "";
            txtTitulo.Text = "";
        }

        private async void btnAlta_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Está seguro que desea dar de alta la tecnicatura "+
                lTecnicatura[lstTecBajas.SelectedIndex].Nombre+" ?","Alta",MessageBoxButtons.YesNo,MessageBoxIcon.Question)
                == DialogResult.Yes)
            {
                try
                {
                    await DarAlta();
                    MessageBox.Show("Se dió de alta la tecnicatura " +
                lTecnicatura[lstTecBajas.SelectedIndex].Nombre, "Alta", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarTecnicaturasAsync();
                    lstTecBajas.SelectedIndex = -1;
                    Habilitar(false);
                    Limpiar();
                }
                catch
                {
                    MessageBox.Show("Ocurrió un error al dar de alta la tecnicatura " +
                lTecnicatura[lstTecBajas.SelectedIndex].Nombre, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async Task DarAlta()
        {
            string url = "http://localhost:5241/tecnicaturas/darAlta";
            string presupuestoJson = JsonConvert.SerializeObject(lTecnicatura[lstTecBajas.SelectedIndex]);
            var data = await ClienteSingleton.GetInstance().PutAsync(url, presupuestoJson);
        }
    }
}
