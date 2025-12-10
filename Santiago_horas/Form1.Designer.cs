using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Panel painelLinhas;       // esquerda
        private Panel painelDireita;      // direita
        private Button btnSalvar;
        private Button btnSexta;
        private Button btnSabado;
        private Button btnFeriado;
        private ComboBox comboFuncionario;
        private DateTimePicker dataPicker;
        private TextBox txtTotal;
        private Label lblFunc;
        private Label lblData;
        private Label lblTotal;
        private Label lblEsq;
        private Label lblDir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.painelLinhas = new Panel();
            this.painelDireita = new Panel();

            this.btnSalvar = new Button();
            this.btnSexta = new Button();
            this.btnSabado = new Button();
            this.btnFeriado = new Button();

            this.comboFuncionario = new ComboBox();
            this.dataPicker = new DateTimePicker();
            this.txtTotal = new TextBox();

            this.lblFunc = new Label();
            this.lblData = new Label();
            this.lblTotal = new Label();
            this.lblEsq = new Label();
            this.lblDir = new Label();

            this.SuspendLayout();

            // painelLinhas
            this.painelLinhas.Location = new Point(20, 100);
            this.painelLinhas.Size = new Size(500, 550);
            this.painelLinhas.BorderStyle = BorderStyle.FixedSingle;

            // painelDireita
            this.painelDireita.Location = new Point(540, 100);
            this.painelDireita.Size = new Size(500, 550);
            this.painelDireita.BorderStyle = BorderStyle.FixedSingle;

            // títulos
            this.lblEsq.Text = "Tabela Principal";
            this.lblEsq.Location = new Point(20, 70);
            this.lblEsq.Width = 200;

            this.lblDir.Text = "Tabela Peça (Opcional)";
            this.lblDir.Location = new Point(540, 70);
            this.lblDir.Width = 200;

            // funcionário
            this.lblFunc.Text = "Funcionário:";
            this.lblFunc.Location = new Point(20, 20);
            this.comboFuncionario.Location = new Point(120, 16);
            this.comboFuncionario.Width = 180;

            // data
            this.lblData.Text = "Data:";
            this.lblData.Location = new Point(340, 20);
            this.dataPicker.Location = new Point(390, 16);
            this.dataPicker.Width = 150;

            // total
            this.lblTotal.Text = "Total:";
            this.lblTotal.Location = new Point(600, 20);
            this.txtTotal.Location = new Point(650, 16);
            this.txtTotal.Width = 80;

            // botões
            this.btnSexta.Text = "Sexta";
            this.btnSexta.Location = new Point(750, 14);
            this.btnSexta.Width = 80;

            this.btnSabado.Text = "Sábado";
            this.btnSabado.Location = new Point(840, 14);
            this.btnSabado.Width = 80;

            this.btnFeriado.Text = "Feriado";
            this.btnFeriado.Location = new Point(930, 14);
            this.btnFeriado.Width = 80;

            // salvar
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.Location = new Point(930, 670);
            this.btnSalvar.Width = 110;
            this.btnSalvar.Height = 40;

            // form
            this.ClientSize = new Size(1080, 730);
            this.Controls.Add(this.painelLinhas);
            this.Controls.Add(this.painelDireita);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.btnSexta);
            this.Controls.Add(this.btnSabado);
            this.Controls.Add(this.btnFeriado);
            this.Controls.Add(this.comboFuncionario);
            this.Controls.Add(this.dataPicker);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.lblFunc);
            this.Controls.Add(this.lblData);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblEsq);
            this.Controls.Add(this.lblDir);

            this.Text = "Santiago - Lançamento de Horas";
            this.ResumeLayout(false);
        }
    }
}