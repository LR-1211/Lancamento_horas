using System.Drawing;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;

namespace Santiago_horas
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelOuter;   // borda vermelha escura
        private System.Windows.Forms.Panel painelLinhas; // painel interno onde pousam as linhas
        private System.Windows.Forms.ComboBox comboFuncionario;
        private System.Windows.Forms.DateTimePicker dataPicker;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnSexta;
        private System.Windows.Forms.Button btnSabado;
        private System.Windows.Forms.Button btnFeriado;
        private System.Windows.Forms.Label lblFuncionario;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.Label lblTotal;

        /// <summary> 
        /// Limpar recursos
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // header (vermelho escuro)
            this.panelHeader = new System.Windows.Forms.Panel();
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(115, 20, 20); // vermelho escuro
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Height = 80;
            this.panelHeader.Margin = new Padding(0);
            this.panelHeader.Padding = new Padding(0);


            // labels e controles do header
            this.lblFuncionario = new System.Windows.Forms.Label();
            this.comboFuncionario = new System.Windows.Forms.ComboBox();
            this.lblData = new System.Windows.Forms.Label();
            this.dataPicker = new System.Windows.Forms.DateTimePicker();
            this.lblTotal = new System.Windows.Forms.Label();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.btnSexta = new System.Windows.Forms.Button();
            this.btnSabado = new System.Windows.Forms.Button();
            this.btnFeriado = new System.Windows.Forms.Button();
            this.btnSalvar = new System.Windows.Forms.Button();


            // panelOuter cria efeito de borda vermelha ao redor do painel interno
            this.panelOuter = new System.Windows.Forms.Panel();
            this.panelOuter.BackColor = System.Drawing.Color.FromArgb(115, 20, 20);
            this.panelOuter.Left = 130;
            this.panelOuter.Top = 80;
            this.panelOuter.Width = 700;
            this.panelOuter.Height = 620;
            this.panelOuter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));

            // painelLinhas interno (light grey)
            this.painelLinhas = new System.Windows.Forms.Panel();
            this.painelLinhas.BackColor = Color.FromArgb(155, 155, 155);
            this.painelLinhas.Left = 0;
            this.painelLinhas.Top = 10;
            this.painelLinhas.Width = 100; // AJUSTE AQUI MANUALMENTE
            this.painelLinhas.Height = this.panelOuter.Height - 40;
            // NÃO deixa esticar pra direita
            this.painelLinhas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.painelLinhas.AutoScroll = true;
            this.panelOuter.Padding = new Padding(0, 0, 250, 0);


            // header children layout
            this.lblFuncionario.Text = "Funcionário";
            this.lblFuncionario.ForeColor = Color.White;
            this.lblFuncionario.Location = new System.Drawing.Point(18, 18);
            this.lblFuncionario.AutoSize = true;

            this.comboFuncionario.Location = new System.Drawing.Point(105, 16);
            this.comboFuncionario.Width = 200;
            this.comboFuncionario.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.lblData.Text = "Data";
            this.lblData.ForeColor = Color.White;
            this.lblData.Location = new System.Drawing.Point(330, 18);
            this.lblData.AutoSize = true;

            this.dataPicker.Location = new System.Drawing.Point(370, 14);
            this.dataPicker.Width = 150;
            this.dataPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.lblTotal.Text = "TOTAL";
            this.lblTotal.ForeColor = Color.White;
            this.lblTotal.Location = new System.Drawing.Point(540, 18);
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);

            this.txtTotal.Location = new System.Drawing.Point(600, 14);
            this.txtTotal.Width = 120;
            this.txtTotal.ReadOnly = true;
            this.txtTotal.BackColor = Color.FromArgb(240, 240, 240);
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // day buttons
            this.btnSexta.Text = "Sexta";
            this.btnSexta.BackColor = System.Drawing.Color.FromArgb(115, 20, 20);
            this.btnSexta.ForeColor = Color.White;
            this.btnSexta.FlatStyle = FlatStyle.Flat;
            this.btnSexta.FlatAppearance.BorderColor = Color.FromArgb(80, 10, 10);
            this.btnSexta.Location = new System.Drawing.Point(760, 12);
            this.btnSexta.Width = 70;

            this.btnSabado.Text = "Sábado";
            this.btnSabado.BackColor = System.Drawing.Color.FromArgb(115, 20, 20);
            this.btnSabado.ForeColor = Color.White;
            this.btnSabado.FlatStyle = FlatStyle.Flat;
            this.btnSabado.FlatAppearance.BorderColor = Color.FromArgb(80, 10, 10);
            this.btnSabado.Location = new System.Drawing.Point(840, 12);
            this.btnSabado.Width = 70;

            this.btnFeriado.Text = "Feriado";
            this.btnFeriado.BackColor = System.Drawing.Color.FromArgb(115, 20, 20);
            this.btnFeriado.ForeColor = Color.White;
            this.btnFeriado.FlatStyle = FlatStyle.Flat;
            this.btnFeriado.FlatAppearance.BorderColor = Color.FromArgb(80, 10, 10);
            this.btnFeriado.Location = new System.Drawing.Point(920, 12);
            this.btnFeriado.Width = 70;
              
            // salvar
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Width = 90;
            this.btnSalvar.Height = 34;
            this.btnSalvar.Location = new System.Drawing.Point(1100, 12);
            this.btnSalvar.BackColor = System.Drawing.Color.FromArgb(115, 20, 20);
            this.btnSalvar.ForeColor = Color.White;
            this.btnSalvar.FlatStyle = FlatStyle.Flat;

            // assemble
            this.panelHeader.Controls.Add(this.lblFuncionario);
            this.panelHeader.Controls.Add(this.comboFuncionario);
            this.panelHeader.Controls.Add(this.lblData);
            this.panelHeader.Controls.Add(this.dataPicker);
            this.panelHeader.Controls.Add(this.lblTotal);
            this.panelHeader.Controls.Add(this.txtTotal);
            this.panelHeader.Controls.Add(this.btnSexta);
            this.panelHeader.Controls.Add(this.btnSabado);
            this.panelHeader.Controls.Add(this.btnFeriado);
            this.panelHeader.Controls.Add(this.btnSalvar);

            // add painelLinhas into panelOuter
            this.panelOuter.Controls.Add(this.painelLinhas);

            // add top-level controls to the form
            this.Controls.Add(this.panelOuter);
            this.Controls.Add(this.panelHeader);

            // Form properties
            this.Text = "Santiago - Lançamento de Horas";
            this.ClientSize = new System.Drawing.Size(1080, 720);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        }
    }
}
