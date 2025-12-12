using System;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public partial class Form1 : Form
    {
        private const int QTDE_LINHAS = 10;
        private LinhaItem[] linhas = new LinhaItem[QTDE_LINHAS];

        private int minHoras = 9;
        private Button botaoSelecionado = null;

        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.FromArgb(155, 155, 155);
            // FIX 1 — Painel não sai mais da tela
            CorrigirLayoutPainel();

            InicializarHeader();
            CriarLinhas();
            WireHeaderEvents();
            AtualizarTotal();
        }

        // =======================================================================
        // FIX: impede painel de sair da janela sem alterar nada do seu design
        // =======================================================================
        private void CorrigirLayoutPainel()
        {
            // panelOuter corrigido
            panelOuter.Dock = DockStyle.Fill;
            panelOuter.Margin = new Padding(0);
            panelOuter.Padding = new Padding(0);
            panelOuter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // painelLinhas corrigido
            painelLinhas.Dock = DockStyle.Fill;
            painelLinhas.Margin = new Padding(0);
            painelLinhas.Padding = new Padding(0);
            painelLinhas.AutoScroll = true;
            painelLinhas.HorizontalScroll.Enabled = false;
            painelLinhas.HorizontalScroll.Visible = false;
            painelLinhas.HorizontalScroll.Maximum = 0;

            // evita overflow horizontal
            painelLinhas.AutoScrollMinSize = new Size(0, 0);
        }

        private void InicializarHeader()
        {
            comboFuncionario.Items.Clear();
            comboFuncionario.Items.AddRange(new object[] { "Luccas", "Sergio", "Schiabel" });
            if (comboFuncionario.Items.Count > 0) comboFuncionario.SelectedIndex = 0;

            dataPicker.Value = DateTime.Today;
        }

        private void CriarLinhas()
        {
            painelLinhas.Controls.Clear();

            int y = 40;
            for (int i = 0; i < QTDE_LINHAS; i++)
            {
                var l = new LinhaItem();

                // FIX — Garantir que nunca ultrapasse o painel
                l.Base.Left = 105;
                l.Base.Top = y;
                l.Base.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                // largura dinâmica REAL
                l.Base.Width = Math.Max(painelLinhas.ClientSize.Width - 20, 200);

                // combo padrão (após marcar o tipo ele muda)
                l.combo.Items.Clear();
                l.combo.Items.AddRange(new object[] { "Projeto A", "Projeto B", "Projeto C", "OS-100", "OS-241", "Lopes" });

                painelLinhas.Controls.Add(l.Base);
                linhas[i] = l;

                int idx = i;

                l.chkPRJ.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkPRJ);
                l.chkOS.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkOS);
                l.chkJUST.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkJUST);

                l.txtHoras.TextChanged += (s, e) => AtualizarTotal();

                l.txtHoras.Text = "00:00";

                y += l.Base.Height + 13;   // margem vertical maior entre as linhas
            }

            // FIX — Redimensionamento perfeito e automático
            painelLinhas.Resize += (s, e) =>
            {
                foreach (var l in linhas)
                {
                    if (l != null)
                        l.Base.Width = painelLinhas.ClientSize.Width -20;
                }
            };
        }

        private void WireHeaderEvents()
        {
            btnSexta.Click += (s, e) => SelecionarDia(8, btnSexta);
            btnSabado.Click += (s, e) => SelecionarDia(0, btnSabado);
            btnFeriado.Click += (s, e) => SelecionarDia(0, btnFeriado);

            btnSalvar.Click += BtnSalvar_Click;
        }

        private void SelecionarDia(int minimo, Button btn)
        {
            if (botaoSelecionado == btn)
            {
                botaoSelecionado.BackColor = Color.FromArgb(115, 20, 20);
                botaoSelecionado = null;
                minHoras = 9;
                AtualizarTotal();
                return;
            }

            if (botaoSelecionado != null)
                botaoSelecionado.BackColor = Color.FromArgb(115, 20, 20);

            botaoSelecionado = btn;
            botaoSelecionado.BackColor = Color.FromArgb(170, 50, 50);

            minHoras = minimo;
            AtualizarTotal();
        }

        private void Exclusivo(LinhaItem linha, CheckBox marcado)
        {
            if (!marcado.Checked)
            {
                linha.combo.DataSource = null;
                linha.combo.Enabled = false;
                linha.comboPeca.DataSource = null;
                linha.comboPeca.Enabled = false;
                return;
            }

            if (marcado != linha.chkPRJ) linha.chkPRJ.Checked = false;
            if (marcado != linha.chkOS) linha.chkOS.Checked = false;
            if (marcado != linha.chkJUST) linha.chkJUST.Checked = false;

            if (marcado == linha.chkPRJ)
            {
                linha.combo.Items.Clear();
                linha.combo.Items.AddRange(new object[] { "Projeto A", "Projeto B", "Projeto C" });
                linha.combo.Enabled = true;
                linha.combo.SelectedIndex = 0;
            }
            else if (marcado == linha.chkOS)
            {
                linha.combo.Items.Clear();
                linha.combo.Items.AddRange(new object[] { "OS-100", "OS-241", "OS-999" });
                linha.combo.Enabled = true;
                linha.combo.SelectedIndex = 0;
            }
            else if (marcado == linha.chkJUST)
            {
                linha.combo.Items.Clear();
                linha.combo.Items.AddRange(new object[] { "Lopes", "Limpeza", "Aguard. Desenho", "Manutenção", "Atestado" });
                linha.combo.Enabled = true;
                linha.combo.SelectedIndex = 0;
            }
        }

        private void AtualizarTotal()
        {
            double soma = 0.0;

            foreach (var l in linhas)
            {
                if (l == null) continue;

                string txt = l.txtHoras.Text.Trim();

                if (!DateTime.TryParseExact(txt, "HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out DateTime hora))
                {
                    l.txtHoras.BackColor = Color.LightCoral;
                    continue;
                }

                l.txtHoras.BackColor = Color.White;
                soma += hora.Hour + (hora.Minute / 60.0);
            }

            txtTotal.Text = soma.ToString("0.##");
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (comboFuncionario.SelectedItem == null || string.IsNullOrWhiteSpace(comboFuncionario.Text))
            {
                MessageBox.Show("Erro: Obrigatório Preenchimento de Funcionário", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtTotal.Text, out double total)) total = 0.0;

            if (total < minHoras)
            {
                MessageBox.Show(
                    $"Erro: total ({total:0.##}h). Requer um mínimo de ({minHoras}h).",
                    "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < linhas.Length; i++)
            {
                var l = linhas[i];
                if (l.chkPRJ.Checked || l.chkOS.Checked || l.chkJUST.Checked)
                {
                    if (l.combo.SelectedItem == null)
                    {
                        MessageBox.Show($"Linha {i + 1}: selecione a opção (combo).",
                            "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            MessageBox.Show("Validação OK. (agora você pode gravar no banco.)",
                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
