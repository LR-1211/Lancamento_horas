using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public class LinhaItem
    {
        public Panel Base { get; private set; }

        // Controls acessíveis a Form1
        public CheckBox chkPRJ { get; private set; }
        public CheckBox chkOS { get; private set; }
        public CheckBox chkJUST { get; private set; }

        // Combo da esquerda (tipo) e comboPeca (peça dependente)
        public ComboBox combo { get; private set; }
        public ComboBox comboPeca { get; private set; }

        // Horas
        public TextBox txtHoras { get; private set; }

        #region Propriedades públicas (usadas pelo Form1)

        private Panel panelPeca;

        public LinhaItem()
        {
            CriarBase();
            CriarControles();
            CriarPainelPeca();
            AplicarEstilos();
            WireEvents();
        }

        private void CriarBase()
        {
            Base = new Panel()
            {
                Height = 48,
                Width = 980,
                BackColor = Color.FromArgb(245, 245, 245),
                Margin = new Padding(6),
                // ancoragem para se estender horizontalmente no painel pai
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
        }

        private void CriarControles()
        {
            chkPRJ = new CheckBox()
            {
                Text = "PRJ",
                Location = new Point(10, 14),
                AutoSize = true
            };

            chkOS = new CheckBox()
            {
                Text = "OS",
                Location = new Point(60, 14),
                AutoSize = true
            };

            chkJUST = new CheckBox()
            {
                Text = "JUST",
                Location = new Point(110, 14),
                AutoSize = true
            };

            combo = new ComboBox()
            {
                Location = new Point(170, 10),
                Width = 170,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            // inicialmente vazio - Form1 pode popular listas específicas
            combo.Items.AddRange(new object[] { /* preenchido por Exclusivo no Form1 */ });

            // comboPeca será criado no painelPeca (à direita)
            txtHoras = new TextBox()
            {
                Location = new Point(980, 10),
                Width = 80,
                Text = "00:00",
                TextAlign = HorizontalAlignment.Center
            };

            Base.Controls.Add(chkPRJ);
            Base.Controls.Add(chkOS);
            Base.Controls.Add(chkJUST);
            Base.Controls.Add(combo);
            Base.Controls.Add(txtHoras);
        }

        private void CriarPainelPeca()
        {
            // painel da peça integrado (fica entre combo e horas)
            panelPeca = new Panel()
            {
                Left = 350,
                Top = 8,
                Width = 445,
                Height = 32,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.None,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };

            comboPeca = new ComboBox()
            {
                Left = 6,
                Top = 4,
                Width = panelPeca.Width - 12,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            // ensure comboPeca resizes with panel:
            comboPeca.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            panelPeca.Controls.Add(comboPeca);
            Base.Controls.Add(panelPeca);
        }

        private void AplicarEstilos()
        {
            // paleta neutra (fundo cinza claro para o Base já aplicado),
            // o Form1 aplica as bordas em torno do painel inteiro — aqui mantemos leve contraste.
            panelPeca.BackColor = Color.FromArgb(242, 242, 242);
            txtHoras.BackColor = Color.White;
            txtHoras.Left = Base.Width - txtHoras.Width - 100;
            txtHoras.Anchor = AnchorStyles.None | AnchorStyles.Top;
            combo.BackColor = Color.White;
            comboPeca.BackColor = Color.White;
        }

        private void WireEvents()
        {

            // Eventos de formatação/hardening do txtHoras
            txtHoras.KeyPress += TxtHoras_KeyPress;
            txtHoras.TextChanged += TxtHoras_TextChanged;
            txtHoras.Leave += TxtHoras_Leave;
        }


        #region Horas - formatação segura

        private void TxtHoras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void TxtHoras_Leave(object sender, EventArgs e)
        {
            FormatHorasTextBox(txtHoras);
        }

        private void TxtHoras_TextChanged(object sender, EventArgs e)
        {
            // remove não-dígitos
            string raw = "";
            foreach (char c in txtHoras.Text)
                if (char.IsDigit(c)) raw += c;

            if (raw.Length > 4) raw = raw.Substring(0, 4);
            raw = raw.PadLeft(4, '0');

            string hh = raw.Substring(0, 2);
            string mm = raw.Substring(2, 2);

            string novo = $"{hh}:{mm}";
            if (txtHoras.Text == novo) return;

            txtHoras.TextChanged -= TxtHoras_TextChanged;
            txtHoras.Text = novo;
            txtHoras.SelectionStart = txtHoras.Text.Length;
            txtHoras.TextChanged += TxtHoras_TextChanged;
        }

        public static void FormatHorasTextBox(TextBox txt)
        {
            if (txt == null) return;
            string raw = "";
            foreach (char c in txt.Text)
                if (char.IsDigit(c)) raw += c;

            if (raw.Length > 4) raw = raw.Substring(0, 4);
            raw = raw.PadLeft(4, '0');

            string hh = raw.Substring(0, 2);
            string mm = raw.Substring(2, 2);
            txt.Text = $"{hh}:{mm}";
        }

        #endregion

        #region Acessores simplificados (read-only)
        public string Projeto => combo?.Text ?? string.Empty;
        public string Peca => comboPeca?.Text ?? string.Empty;
        public string Horas => txtHoras?.Text ?? "00:00";
        public bool IsPRJ => chkPRJ?.Checked ?? false;
        public bool IsOS => chkOS?.Checked ?? false;
        public bool IsJust => chkJUST?.Checked ?? false;
        #endregion
        public string NumeroOS => combo?.SelectedItem?.ToString();

    }
}