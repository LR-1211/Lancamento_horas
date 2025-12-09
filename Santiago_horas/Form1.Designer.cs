using System;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public partial class Form1 : Form
    {
        // controles principais
        private ComboBox comboFuncionario;
        private DateTimePicker dataPicker;
        private Button btnSexta;
        private Button btnSabado;
        private Button btnFeriado;
        private Panel painelLinhas;
        private TextBox txtTotal;
        private Button btnSalvar;

        // array com 8 linhas
        private LinhaItem[] linhas;

        private void InitializeComponent()
        {
            this.Text = "Lançamento de Horas";
            this.Size = new Size(1100, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(242, 242, 242);
            this.Font = new Font("Arial", 10);

            // cabeçalho: funcionario e data
            comboFuncionario = new ComboBox() { Left = 40, Top = 40, Width = 420, DropDownStyle = ComboBoxStyle.DropDown };
            comboFuncionario.Font = new Font("Arial", 11);
            this.Controls.Add(comboFuncionario);

            dataPicker = new DateTimePicker() { Left = 820, Top = 40, Width = 200, Format = DateTimePickerFormat.Short };
            this.Controls.Add(dataPicker);

            btnSexta = new Button() { Text = "SEXTA", Left = 480, Top = 40, Width = 80, Height = 30, BackColor = SystemColors.Control };
            btnSabado = new Button() { Text = "SÁBADO", Left = 570, Top = 40, Width = 80, Height = 30, BackColor = SystemColors.Control };
            btnFeriado = new Button() { Text = "FERIADO", Left = 660, Top = 40, Width = 80, Height = 30, BackColor = SystemColors.Control };
            this.Controls.AddRange(new Control[] { btnSexta, btnSabado, btnFeriado });

            // painel com as linhas
            painelLinhas = new Panel()
            {
                Left = 40,
                Top = 100,
                Width = 980,
                Height = 460,
                AutoScroll = true,
                BackColor = Color.Transparent
            };
            this.Controls.Add(painelLinhas);

            // criar 8 linhas
            linhas = new LinhaItem[8];
            for (int i = 0; i < 8; i++)
            {
                linhas[i] = new LinhaItem();
                linhas[i].Base.Left = 0;
                linhas[i].Base.Top = i * 56;
                painelLinhas.Controls.Add(linhas[i].Base);
            }

            // total e salvar no rodapé
            txtTotal = new TextBox() { Left = 740, Top = 580, Width = 100, ReadOnly = true, Text = "0.0" };
            txtTotal.Font = new Font("Arial", 11, FontStyle.Bold);
            this.Controls.Add(txtTotal);

            btnSalvar = new Button()
            {
                Text = "SALVAR",
                Left = 860,
                Top = 575,
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            this.Controls.Add(btnSalvar);
        }
    }

    // Classe auxiliar representando 1 linha do grid
    public class LinhaItem
    {
        public Panel Base;
        public CheckBox chkPRJ, chkOS, chkJUST;
        public ComboBox combo;
        public TextBox txtHoras;

        public LinhaItem()
        {
            Base = new Panel()
            {
                Width = 940,
                Height = 48,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            chkPRJ = new CheckBox() { Left = 8, Top = 12, Width = 40, Text = "PRJ" };
            chkOS = new CheckBox() { Left = 68, Top = 12, Width = 40, Text = "OS" };
            chkJUST = new CheckBox() { Left = 118, Top = 12, Width = 50, Text = "JUST" };

            combo = new ComboBox() { Left = 200, Top = 8, Width = 520, Enabled = false, DropDownStyle = ComboBoxStyle.DropDownList };
            txtHoras = new TextBox() { Left = 740, Top = 10, Width = 140, Text = "" };

            Base.Controls.AddRange(new Control[] { chkPRJ, chkOS, chkJUST, combo, txtHoras });
        }
    }
}
