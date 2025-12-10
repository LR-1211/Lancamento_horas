using System;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public partial class Form1 : Form
    {
        private int minHoras = 9;
        private Button botaoSelecionado = null;

        private const int QTDE_LINHAS = 10;
        private LinhaItem[] linhas;

        public Form1()
        {
            InitializeComponent();
            CriarLinhas();
            CriarEventos();
        }

        // monta as linhas no painel esquerdo
        private void CriarLinhas()
        {
            linhas = new LinhaItem[QTDE_LINHAS];

            int y = 10;
            for (int i = 0; i < QTDE_LINHAS; i++)
            {
                linhas[i] = new LinhaItem();
                linhas[i].Base.Location = new Point(10, y);

                painelLinhas.Controls.Add(linhas[i].Base);

                linhas[i].txtHoras.Text = "00:00";

                y += 45;

                // eventos exclusivos
                int idx = i;

                linhas[i].chkPRJ.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkPRJ);
                linhas[i].chkOS.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkOS);
                linhas[i].chkJUST.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkJUST);

                linhas[i].txtHoras.TextChanged += (s, e) => AtualizarTotal();
            }
        }

        private void CriarEventos()
        {
            btnSexta.Click += (s, e) => SelecionarDia(8, btnSexta);
            btnSabado.Click += (s, e) => SelecionarDia(0, btnSabado);
            btnFeriado.Click += (s, e) => SelecionarDia(0, btnFeriado);

            comboFuncionario.DropDown += (s, e) =>
            {
                if (comboFuncionario.Items.Count == 0)
                    comboFuncionario.Items.AddRange(new object[] { "Luccas", "Sergio", "Schiabel" });
            };

            dataPicker.Value = DateTime.Today;

            btnSalvar.Click += BtnSalvar_Click;

            AtualizarTotal();
        }

        private void SelecionarDia(int minimo, Button btn)
        {
            if (botaoSelecionado == btn)
            {
                botaoSelecionado.BackColor = SystemColors.Control;
                botaoSelecionado = null;
                minHoras = 9;
                AtualizarTotal();
                return;
            }

            if (botaoSelecionado != null)
                botaoSelecionado.BackColor = SystemColors.Control;

            botaoSelecionado = btn;
            botaoSelecionado.BackColor = Color.LightBlue;

            minHoras = minimo;
            AtualizarTotal();
        }

        private void Exclusivo(LinhaItem linha, CheckBox marcado)
        {
            if (!marcado.Checked)
            {
                linha.combo.DataSource = null;
                linha.combo.Enabled = false;
                return;
            }

            // desmarca outros
            if (marcado != linha.chkPRJ) linha.chkPRJ.Checked = false;
            if (marcado != linha.chkOS) linha.chkOS.Checked = false;
            if (marcado != linha.chkJUST) linha.chkJUST.Checked = false;

            // popula combo
            if (marcado == linha.chkPRJ)
            {
                linha.combo.DataSource = new string[] { "Projeto A", "Projeto B", "Projeto C" };
                linha.combo.Enabled = true;
            }
            else if (marcado == linha.chkOS)
            {
                linha.combo.DataSource = new string[] { "OS-100", "OS-241", "OS-999" };
                linha.combo.Enabled = true;
            }
            else if (marcado == linha.chkJUST)
            {
                linha.combo.DataSource = new string[]
                {
                    "Lopes", "Limpeza", "Aguard. Desenho", "Manutenção", "Atestado"
                };
                linha.combo.Enabled = true;
            }
        }

        private void AtualizarTotal()
        {
            double soma = 0.0;

            foreach (var l in linhas)
            {
                string txt = l.txtHoras.Text;

                if (!DateTime.TryParseExact(txt, "HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime hora))
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
            if (!double.TryParse(txtTotal.Text, out double total))
                total = 0;

            if (total < minHoras)
            {
                MessageBox.Show(
                    $"Erro: total ({total:0.##}h). Requer mínimo de {minHoras}h.",
                    "Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            for (int i = 0; i < linhas.Length; i++)
            {
                var l = linhas[i];
                if (l.chkPRJ.Checked || l.chkOS.Checked || l.chkJUST.Checked)
                {
                    if (l.combo.SelectedItem == null)
                    {
                        MessageBox.Show(
                            $"Linha {i + 1}: selecione uma opção no combo.",
                            "Validação",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }
                }
            }

            MessageBox.Show("OK — pronto para salvar no banco.", "Sucesso", MessageBoxButtons.OK);
        }
    }
}