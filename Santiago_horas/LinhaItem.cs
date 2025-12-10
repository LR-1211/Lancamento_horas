using System;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public class LinhaItem
    {
        public Panel Base { get; private set; }
        public CheckBox chkPRJ { get; private set; }
        public CheckBox chkOS { get; private set; }
        public CheckBox chkJUST { get; private set; }
        public ComboBox combo { get; private set; }
        public TextBox txtHoras { get; private set; }

        public LinhaItem()
        {
            Base = new Panel();
            Base.Width = 480;
            Base.Height = 40;
            Base.BorderStyle = BorderStyle.None;

            chkPRJ = new CheckBox()
            {
                Text = "PRJ",
                Location = new Point(5, 10),
                Width = 50
            };

            chkOS = new CheckBox()
            {
                Text = "OS",
                Location = new Point(60, 10),
                Width = 45
            };

            chkJUST = new CheckBox()
            {
                Text = "JUST",
                Location = new Point(110, 10),
                Width = 55
            };

            combo = new ComboBox()
            {
                Location = new Point(180, 7),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };

            txtHoras = new TextBox()
            {
                Location = new Point(350, 7),
                Width = 115,
                Text = "00:00"
            };

            // conecta o handler nomeado (corrige problema de remoção/reentrada)
            txtHoras.TextChanged += TxtHoras_TextChanged;
            txtHoras.KeyPress += TxtHoras_KeyPress;
            txtHoras.Leave += TxtHoras_Leave;

            Base.Controls.Add(chkPRJ);
            Base.Controls.Add(chkOS);
            Base.Controls.Add(chkJUST);
            Base.Controls.Add(combo);
            Base.Controls.Add(txtHoras);
        }

        // Evita que o usuário digite letras (apenas dígitos e backspace)
        private void TxtHoras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '\b')
                e.Handled = true;
        }

        // Garante formatação final ao perder foco
        private void TxtHoras_Leave(object sender, EventArgs e)
        {
            FormatHorasTextBox(txtHoras);
        }

        // Handler nomeado para TextChanged — usamos unsubscribe/subscribe com o mesmo método
        private void TxtHoras_TextChanged(object sender, EventArgs e)
        {
            // captura os dígitos e normaliza para 4 chars
            string raw = "";
            foreach (char c in txtHoras.Text)
                if (char.IsDigit(c)) raw += c;

            if (raw.Length > 4) raw = raw.Substring(0, 4);
            raw = raw.PadLeft(4, '0');

            // monta HH:mm
            string hh = raw.Substring(0, 2);
            string mm = raw.Substring(2, 2);
            string novo = $"{hh}:{mm}";

            // se já está no formato esperado, não faz nada
            if (txtHoras.Text == novo) return;

            // evita reentrada: remove o handler, altera o texto, readiciona o handler
            txtHoras.TextChanged -= TxtHoras_TextChanged;
            txtHoras.Text = novo;

            // posiciona o caret no fim (opcional)
            txtHoras.SelectionStart = txtHoras.Text.Length;

            txtHoras.TextChanged += TxtHoras_TextChanged;
        }

        // utilitário público caso seja necessário formatar em outras partes
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
    }
}
