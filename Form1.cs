//diseño de form desde el codigo
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RaceX
{
    public partial class MainForm : Form
    {
        // Constantes del juego
        private const int META = 150;
        private const int MIN_AUTOS = 3;
        private const int AVANCE_MIN = 5;
        private const int AVANCE_MAX = 15;
        private const int PENALIZACION_OBSTACULO = -5;
        private const double PROBABILIDAD_OBSTACULO = 0.3;

        // Variables del juego
        private List<Auto> autos;
        private string climaActual;
        private bool carreraIniciada;
        private Random random;

        // Controles UI
        private TextBox txtNombreAuto;
        private ComboBox cmbTipoAuto;
        private ComboBox cmbClima;
        private Button btnAgregarAuto;
        private Button btnIniciarCarrera;
        private Button btnSiguienteTurno;
        private Button btnReiniciar;
        private DataGridView dgvAutos;
        private Label lblEstado;
        private Panel panelProgreso;

        public MainForm()
        {
            ConfigurarFormulario();
            InicializarJuego();
        }

        private void ConfigurarFormulario()
        {
            // Configuración básica del formulario
            this.Text = "RaceX - Simulador de Carrera Multivehículos";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Panel de configuración
            Panel panelConfig = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(780, 150),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelConfig);

            // Controles para registrar autos
            Label lblNombreAuto = new Label
            {
                Text = "Nombre del Auto:",
                Location = new Point(10, 20),
                Size = new Size(120, 20)
            };
            panelConfig.Controls.Add(lblNombreAuto);

            txtNombreAuto = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(150, 20)
            };
            panelConfig.Controls.Add(txtNombreAuto);

            Label lblTipoAuto = new Label
            {
                Text = "Tipo de Auto:",
                Location = new Point(10, 50),
                Size = new Size(120, 20)
            };
            panelConfig.Controls.Add(lblTipoAuto);

            cmbTipoAuto = new ComboBox
            {
                Location = new Point(130, 50),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbTipoAuto.Items.AddRange(new string[] { "Deportivo", "Todoterreno", "Híbrido" });
            panelConfig.Controls.Add(cmbTipoAuto);

            btnAgregarAuto = new Button
            {
                Text = "Agregar Auto",
                Location = new Point(130, 80),
                Size = new Size(150, 30)
            };
            btnAgregarAuto.Click += BtnAgregarAuto_Click;
            panelConfig.Controls.Add(btnAgregarAuto);

            // Controles para seleccionar clima
            Label lblClima = new Label
            {
                Text = "Clima:",
                Location = new Point(350, 20),
                Size = new Size(120, 20)
            };
            panelConfig.Controls.Add(lblClima);

            cmbClima = new ComboBox
            {
                Location = new Point(470, 20),
                Size = new Size(150, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbClima.Items.AddRange(new string[] { "Soleado", "Lluvia", "Ventoso" });
            panelConfig.Controls.Add(cmbClima);

            // Botones de control del juego
            btnIniciarCarrera = new Button
            {
                Text = "Iniciar Carrera",
                Location = new Point(470, 50),
                Size = new Size(150, 30),
                Enabled = false
            };
            btnIniciarCarrera.Click += BtnIniciarCarrera_Click;
            panelConfig.Controls.Add(btnIniciarCarrera);

            btnSiguienteTurno = new Button
            {
                Text = "Siguiente Turno",
                Location = new Point(470, 90),
                Size = new Size(150, 30),
                Enabled = false
            };
            btnSiguienteTurno.Click += BtnSiguienteTurno_Click;
            panelConfig.Controls.Add(btnSiguienteTurno);

            btnReiniciar = new Button
            {
                Text = "Reiniciar Juego",
                Location = new Point(630, 90),
                Size = new Size(120, 30)
            };
            btnReiniciar.Click += BtnReiniciar_Click;
            panelConfig.Controls.Add(btnReiniciar);

            // DataGridView para mostrar los autos
            dgvAutos = new DataGridView
            {
                Location = new Point(10, 170),
                Size = new Size(780, 180),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvAutos.Columns.Add("Nombre", "Nombre");
            dgvAutos.Columns.Add("Tipo", "Tipo");
            dgvAutos.Columns.Add("Distancia", "Distancia (m)");
            this.Controls.Add(dgvAutos);

            // Label para mostrar el estado del juego
            lblEstado = new Label
            {
                Location = new Point(10, 360),
                Size = new Size(780, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Arial", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblEstado);

            // Panel para mostrar el progreso visual de los autos
            panelProgreso = new Panel
            {
                Location = new Point(10, 410),
                Size = new Size(780, 150),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };
            this.Controls.Add(panelProgreso);
        }

        private void InicializarJuego()
        {
            autos = new List<Auto>();
            carreraIniciada = false;
            random = new Random();
            climaActual = "";

            ActualizarUI();
            ActualizarEstado("Registre al menos 3 autos para iniciar la carrera.");
        }

        private void BtnAgregarAuto_Click(object sender, EventArgs e)
        {
            try
            {
                if (carreraIniciada)
                {
                    MessageBox.Show("No se pueden agregar autos una vez iniciada la carrera.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string nombre = txtNombreAuto.Text.Trim();
                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("El nombre del auto no puede estar vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Verificar si ya existe un auto con ese nombre
                bool duplicado = false;
                foreach (var auto in autos)
                {
                    if (auto.Nombre.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                    {
                        duplicado = true;
                        break;
                    }
                }

                if (duplicado)
                {
                    MessageBox.Show("Ya existe un auto con ese nombre.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cmbTipoAuto.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar un tipo de auto.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string tipo = cmbTipoAuto.SelectedItem.ToString();
                Auto nuevoAuto = AutoFactory.CrearAuto(nombre, tipo);

                autos.Add(nuevoAuto);
                ActualizarUI();

                txtNombreAuto.Clear();
                cmbTipoAuto.SelectedIndex = -1;

                string mensaje;
                if (autos.Count >= MIN_AUTOS)
                    mensaje = $"Auto {nombre} ({tipo}) agregado. Ya puede iniciar la carrera.";
                else
                    mensaje = $"Auto {nombre} ({tipo}) agregado. Faltan {MIN_AUTOS - autos.Count} autos para iniciar.";

                ActualizarEstado(mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar auto: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnIniciarCarrera_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbClima.SelectedIndex == -1)
                {
                    MessageBox.Show("Debe seleccionar un clima para la carrera.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                climaActual = cmbClima.SelectedItem.ToString();
                carreraIniciada = true;

                // Actualizar UI
                btnAgregarAuto.Enabled = false;
                btnIniciarCarrera.Enabled = false;
                btnSiguienteTurno.Enabled = true;
                cmbClima.Enabled = false;
                txtNombreAuto.Enabled = false;
                cmbTipoAuto.Enabled = false;

                ActualizarEstado($"¡Carrera iniciada! Clima: {climaActual}. Presione 'Siguiente Turno' para comenzar.");
                ActualizarVistaProgreso();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al iniciar carrera: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSiguienteTurno_Click(object sender, EventArgs e)
        {
            try
            {
                string mensaje = "Avances en este turno:\n";

                // Aplicar obstáculo aleatorio (30% de probabilidad)
                int autoObstaculoIndex = -1;
                if (random.NextDouble() < PROBABILIDAD_OBSTACULO)
                {
                    autoObstaculoIndex = random.Next(autos.Count);
                    autos[autoObstaculoIndex].AplicarObstaculo(PENALIZACION_OBSTACULO);
                    mensaje += $"¡OBSTÁCULO! {autos[autoObstaculoIndex].Nombre} sufre una penalización de {PENALIZACION_OBSTACULO} metros.\n";
                }

                // Avanzar cada auto
                foreach (var auto in autos)
                {
                    int avanceBase = random.Next(AVANCE_MIN, AVANCE_MAX + 1);
                    int avanceTotal = auto.Avanzar(avanceBase, climaActual);

                    if (autoObstaculoIndex == -1 || auto != autos[autoObstaculoIndex])
                    {
                        mensaje += $"{auto.Nombre}: {avanceTotal} metros ({auto.DistanciaRecorrida}m en total)\n";
                    }
                }

                // Verificar si hay ganador
                Auto ganador = null;
                foreach (var auto in autos)
                {
                    if (auto.DistanciaRecorrida >= META)
                    {
                        ganador = auto;
                        break;
                    }
                }

                if (ganador != null)
                {
                    ActualizarUI();
                    ActualizarVistaProgreso();
                    MessageBox.Show($"¡{ganador.Nombre} ha ganado la carrera alcanzando {ganador.DistanciaRecorrida} metros!", "¡Tenemos un ganador!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnSiguienteTurno.Enabled = false;
                    ActualizarEstado($"Carrera finalizada. {ganador.Nombre} es el ganador con {ganador.DistanciaRecorrida} metros recorridos.");
                    return;
                }

                ActualizarUI();
                ActualizarVistaProgreso();
                ActualizarEstado(mensaje);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error en el turno: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnReiniciar_Click(object sender, EventArgs e)
        {
            // Limpiar todo y reiniciar el juego
            txtNombreAuto.Clear();
            cmbTipoAuto.SelectedIndex = -1;
            cmbClima.SelectedIndex = -1;

            txtNombreAuto.Enabled = true;
            cmbTipoAuto.Enabled = true;
            cmbClima.Enabled = true;
            btnAgregarAuto.Enabled = true;

            InicializarJuego();
        }

        private void ActualizarUI()
        {
            // Actualizar DataGridView
            dgvAutos.Rows.Clear();
            foreach (var auto in autos)
            {
                dgvAutos.Rows.Add(auto.Nombre, auto.TipoAuto, auto.DistanciaRecorrida);
            }

            // Actualizar estado de los botones
            btnIniciarCarrera.Enabled = autos.Count >= MIN_AUTOS && !carreraIniciada;
        }

        private void ActualizarEstado(string mensaje)
        {
            lblEstado.Text = mensaje;
        }

        private void ActualizarVistaProgreso()
        {
            panelProgreso.Controls.Clear();
            int anchoMaximo = panelProgreso.Width - 30;
            int altoCarril = 30;
            int factorEscala = anchoMaximo / META;

            for (int i = 0; i < autos.Count; i++)
            {
                // Etiqueta del auto
                Label lblAuto = new Label
                {
                    Text = autos[i].Nombre,
                    Location = new Point(5, i * altoCarril + 10),
                    Size = new Size(100, 20),
                    TextAlign = ContentAlignment.MiddleRight
                };
                panelProgreso.Controls.Add(lblAuto);

                // Barra de progreso
                int anchoBarra = (int)(autos[i].DistanciaRecorrida * factorEscala);
                anchoBarra = Math.Min(anchoBarra, anchoMaximo);
                Panel barraProg = new Panel
                {
                    Location = new Point(110, i * altoCarril + 10),
                    Size = new Size(anchoBarra, 20),
                    BackColor = ObtenerColorAuto(autos[i].TipoAuto)
                };
                panelProgreso.Controls.Add(barraProg);

                // Etiqueta de distancia
                Label lblDist = new Label
                {
                    Text = $"{autos[i].DistanciaRecorrida}m",
                    Location = new Point(110 + anchoBarra + 5, i * altoCarril + 10),
                    Size = new Size(60, 20),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                panelProgreso.Controls.Add(lblDist);

                // Meta
                if (i == 0)
                {
                    Panel lineaMeta = new Panel
                    {
                        Location = new Point(110 + factorEscala * META, 0),
                        Size = new Size(2, panelProgreso.Height),
                        BackColor = Color.Red
                    };
                    panelProgreso.Controls.Add(lineaMeta);

                    Label lblMeta = new Label
                    {
                        Text = "META",
                        Location = new Point(110 + factorEscala * META - 25, panelProgreso.Height - 25),
                        Size = new Size(50, 20),
                        ForeColor = Color.Red,
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    panelProgreso.Controls.Add(lblMeta);
                }
            }
        }

        private Color ObtenerColorAuto(string tipo)
        {
            if (tipo == "Deportivo")
                return Color.Red;
            else if (tipo == "Todoterreno")
                return Color.Blue;
            else if (tipo == "Híbrido")
                return Color.Green;
            else
                return Color.Gray;
        }
    }
}