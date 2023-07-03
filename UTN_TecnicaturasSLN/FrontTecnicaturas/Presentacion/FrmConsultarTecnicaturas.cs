﻿using DLL_API.Dominio;
using FrontTenicaturas.Servicios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;


namespace FrontTecnicaturas.Presentacion
{
    public partial class FrmConsultarTecnicaturas : Form
    {
        List<Tecnicatura> lTecnicatura;
        public FrmConsultarTecnicaturas()
        {   
            InitializeComponent();
            lTecnicatura = new List<Tecnicatura>();
            CenterToScreen();
            CenterToParent();
        }
        private async void FrmConsultarCarreras_Load(object sender, EventArgs e)
        {
            Habilitar(false);
            Editar(false);
            await CargarListaTecAsync();
            CantidadCarreras();
            await CargarAsignaturasAsync();
            
        }

        private async Task CargarListaTecAsync()
        {
            string url = "http://localhost:5241/tecnicaturas/verAltas";
            var data = await ClienteSingleton.GetInstance().GetAsync(url);
#pragma warning disable CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
            List<Tecnicatura> lst = JsonConvert.DeserializeObject<List<Tecnicatura>>(data);
#pragma warning restore CS8600 // Se va a convertir un literal nulo o un posible valor nulo en un tipo que no acepta valores NULL
            lstTecnicaturas.Items.Clear();
            lTecnicatura.Clear();

            if (lst != null)
            {
                lTecnicatura = lst;
                foreach (Tecnicatura t in lTecnicatura)
                {
                    lstTecnicaturas.Items.Add(t.Nombre);
                }
            }
            else MessageBox.Show("No se pueden recuperar las tecnicaturas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }

        private void CantidadCarreras()
        {
            int cant=lTecnicatura.Count;
            if (cant > 0)
            {
                lblCantTecnicaturas.Text = "Cant. Tecnicaturas: " + cant.ToString();
            }
            else
                MessageBox.Show("Error. No se puede obtener la cantidad de tecnicaturas","Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Editar(bool x)
        {
            txtNombre.Enabled = x;
            txtTitulo.Enabled = x;
            dgvDetalle.Enabled = x;
            cboMaterias.Enabled = x;
            nudAnioCursado.Enabled = x;
            rbtPrimero.Enabled = x;
            rbtSegundo.Enabled = x;
            btnAceptar.Enabled = x;
            btnAgregar.Enabled = x;

            btnBorrarTec.Enabled = !x;
            btnModificar.Enabled = !x;
            btnSalir.Enabled = !x;
        }
        private void Habilitar(bool x)
        {
            btnBorrarTec.Enabled = x;
            btnModificar.Enabled = x;
            btnCancelar.Enabled = x;
            btnSalir.Enabled = !x;
            
        }

        private void lblDetalles_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Habilitar(false);
            Editar(false);
            lstTecnicaturas.SelectedIndex = -1;
        }
        private void Limpiar()
        {
            dgvDetalle.Rows.Clear();
            txtNombre.Text = "";
            txtTitulo.Text = "";
            cboMaterias.SelectedIndex = -1;
            rbtPrimero.Checked = false;
            rbtSegundo.Checked = false;
            nudAnioCursado.Value = 1;
        }
        private void lstCarreras_SelectedIndexChanged(object sender, EventArgs e)
        {
            Limpiar();
            
            if (lstTecnicaturas.SelectedIndex > -1)
            {
                Habilitar(true);
                CargarDatos(lstTecnicaturas.SelectedIndex);
            }
        }
        private void CargarDatos(int index)
        {
            foreach (Tecnicatura c in lTecnicatura)
            {
                txtNombre.Text = lTecnicatura[index].Nombre;
                txtTitulo.Text= lTecnicatura[index].Titulo;
                
            }
            foreach (DetalleTecnicatura dc in lTecnicatura[index].Detalles)
            {
                int anio = dc.AñoCursado;
                int cuatr = dc.Cuatrimestre;
                string asig = dc.Materia.Nombre;
                int id = dc.Materia.Id;
                dgvDetalle.Rows.Add(new object[] { id, asig, cuatr, anio });
            }
        }
        private void lblCantCarreras_Click(object sender, EventArgs e)
        {

        }

        private async void BajaTecnicatura()
        {
                string url = "http://localhost:5241/tecnicaturas/darBaja";
                string tecnicaturaJson = JsonConvert.SerializeObject(lTecnicatura[lstTecnicaturas.SelectedIndex]);
                await ClienteSingleton.GetInstance().PutAsync(url, tecnicaturaJson);
        }
        private async void btnBorrarC_Click(object sender, EventArgs e)
        {
            if(lstTecnicaturas.SelectedIndex> -1)
            {
                if (MessageBox.Show("¿Está seguro que desea dar de baja la tecnicatura " + lTecnicatura[lstTecnicaturas.SelectedIndex].Nombre + "?", "Borrar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        BajaTecnicatura();
                        lstTecnicaturas.SelectedIndex = -1;
                        Habilitar(false);
                        MessageBox.Show("La tecnicatura se encuentra ahora dada de baja",
                        "Borrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarListaTecAsync();
                        CantidadCarreras();
                        Limpiar();
                    }
                    catch
                    {
                        MessageBox.Show("Ha ocurrido un error al dar de baja", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }  
        }
        
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (lstTecnicaturas.SelectedIndex > -1)
            {
                Editar(true);                      
            }
        }

        private async void btnAceptar_Click(object sender, EventArgs e)
        {
            
            if (ValidarCarrera())
            {
                foreach (Tecnicatura c in lTecnicatura)
                {
                    if (c.Id == lTecnicatura[lstTecnicaturas.SelectedIndex].Id)
                    {
                        try
                        {
                            c.Nombre = txtNombre.Text;
                            c.Titulo = txtTitulo.Text;

                            string url = "http://localhost:5241/tecnicaturas/modificar";
                            string tecnicaturaJson = JsonConvert.SerializeObject(c);
                            var data = await ClienteSingleton.GetInstance().PutAsync(url, tecnicaturaJson);

                            Editar(false);
                            Habilitar(false);
                            MessageBox.Show("Tecnicatura actualizada", "Actualización", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            await CargarListaTecAsync();
                            Limpiar();
                            break;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Ha ocurrido un error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }     
        }

        private bool ValidarCarrera()
        {
            bool valido = true;
            if (txtNombre.Text == "")
            {
                valido = false;
                MessageBox.Show("Debe ingresar un nombre de tecnicatura", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtNombre.Focus();
            }
            else try { txtNombre.Text.ToString(); }
                catch (Exception)
                {
                    valido = false;
                    MessageBox.Show("Debe ingresar un nombre de tecnicatura válido", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtNombre.Focus();
                }
            if (txtTitulo.Text=="")
            {
                valido = false;
                MessageBox.Show("Debe ingresar un título", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtTitulo.Focus();
            }
            else try { txtTitulo.Text.ToString(); }
                catch (Exception)
                {
                    valido = false;
                    MessageBox.Show("Debe ingresar un título válido", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtTitulo.Focus();
                }
            if (dgvDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Debe ingresar por lo menos una materia", "Control", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;
            }
            return valido;
        }
        private async Task CargarAsignaturasAsync()
        {
            string url = "http://localhost:5241/materias/ver";
            var data = await ClienteSingleton.GetInstance().GetAsync(url);
            List<Materia> lst = JsonConvert.DeserializeObject<List<Materia>>(data);
            cboMaterias.DataSource = lst;
            cboMaterias.DisplayMember = "Nombre";
            cboMaterias.ValueMember = "Id";
            cboMaterias.SelectedIndex = -1;
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (Validar())
            {
                foreach (Tecnicatura c in lTecnicatura)
                {
                    if (c.Id == lTecnicatura[lstTecnicaturas.SelectedIndex].Id)
                    {
                        Materia m = (Materia)cboMaterias.SelectedItem;
                        int cuatr;
                        if (rbtPrimero.Checked) cuatr = 1;
                        else cuatr = 2;
                        int anio = Convert.ToInt32(nudAnioCursado.Value);

                        DetalleTecnicatura dc = new DetalleTecnicatura(m, cuatr, anio);

                        c.Agregar(dc);

                        dgvDetalle.Rows.Add(new object[] { m.Id, m.Nombre, cuatr, anio });
                    }

                }
            }
        }
        private bool Validar()
        {
            bool valido = true;
            if (cboMaterias.Text.Equals(string.Empty))
            {
                MessageBox.Show("Debe seleccionar una materia", "Control",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;

            }
            if (rbtPrimero.Checked == false & rbtSegundo.Checked == false)
            {
                MessageBox.Show("Debe ingresar un cuatrimestre", "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                valido = false;

            }
                try 
                {
                    Convert.ToInt32(nudAnioCursado.Value);
                }
                catch (Exception)
                {
                    MessageBox.Show("Debe ingresar un año de cursado válido", "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    valido = false;
                }
            foreach (DataGridViewRow row in dgvDetalle.Rows)
            {
                if (row.Cells["ClmMateria"].Value.ToString().Equals(cboMaterias.Text))
                {
                    MessageBox.Show("¡La materia: " + cboMaterias.Text + " ya se encuentra añadida!",
                        "Control",
                      MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    valido = false; break;

                }
            }
            return valido;
        }

        private void dgvDetalle_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvDetalle.CurrentCell.ColumnIndex == 4)
            {
                foreach(Tecnicatura c in lTecnicatura)
                {
                    if (c.Id == lTecnicatura[lstTecnicaturas.SelectedIndex].Id)
                    {
                        c.Eliminar(dgvDetalle.CurrentRow.Index); //eliminar detalle de list<>
                        dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow); //eliminar detalle del dgv
                    }
                }
            }
        }

        private async void btnTecBajas_Click(object sender, EventArgs e)
        {
            FrmTecnicaturasBaja frmbajas = new FrmTecnicaturasBaja();
            frmbajas.ShowDialog();
            frmbajas.Dispose();
            await CargarListaTecAsync();
        }
    }
}

