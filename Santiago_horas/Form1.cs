using System;
using System.Drawing;
using System.Windows.Forms;

namespace Santiago_horas
{
    public partial class Form1 : Form
    {
        private int minHoras = 9; // 9 por padrão

        public Form1()
        {
            InitializeComponent();
            CriarEventos();

            foreach (var l in linhas)
                l.txtHoras.Text = "00:00";
        }

        private void CriarEventos()
        {
            // modos do dia (exclusivos)
            btnSexta.Click += (s, e) => SelecionarDia(8, btnSexta);
            btnSabado.Click += (s, e) => SelecionarDia(0, btnSabado);
            btnFeriado.Click += (s, e) => SelecionarDia(0, btnFeriado);

            // para cada linha: garantir exclusividade e atualizar total quando horas mudam
            for (int i = 0; i < linhas.Length; i++)
            {
                int idx = i;
                linhas[idx].chkPRJ.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkPRJ);
                linesafe_EventWire(linhas[idx].chkPRJ, idx);

                linhas[idx].chkOS.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkOS);
                linesafe_EventWire(linhas[idx].chkOS, idx);

                linhas[idx].chkJUST.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkJUST);
                linesafe_EventWire(linhas[idx].chkJUST, idx);

                linhas[idx].txtHoras.TextChanged += (s, e) => AtualizarTotal();
            }

            btnSalvar.Click += BtnSalvar_Click;

            // preencher funcionários ao abrir dropdown (exemplo temporário)
            comboFuncionario.DropDown += (s, e) =>
            {
                if (comboFuncionario.Items.Count == 0)
                    comboFuncionario.Items.AddRange(new object[] { "Luccas", "Sergio", "Schiabel" });
            };

            // data atual
            dataPicker.Value = DateTime.Today;

            AtualizarTotal();
        }

        // helper to avoid closure capture issues and make click behavior predictable
        private void linesafe_EventWire(CheckBox chk, int idx)
        {
            chk.Click += (s, e) => { /* nothing extra here; CheckedChanged handles logic */ };
        }

        private Button botaoSelecionado = null;

        private void SelecionarDia(int minimo, Button btn)
        {
            // se clicou no MESMO botão → desmarcar
            if (botaoSelecionado == btn)
            {
                botaoSelecionado.BackColor = SystemColors.Control;
                botaoSelecionado = null;

                minHoras = 9;          // padrão
                AtualizarTotal();
                return;
            }

            // se havia um outro selecionado → limpa
            if (botaoSelecionado != null)
                botaoSelecionado.BackColor = SystemColors.Control;

            // ativa o novo
            botaoSelecionado = btn;
            botaoSelecionado.BackColor = Color.LightBlue;

            minHoras = minimo;
            AtualizarTotal();
        }

        private void Exclusivo(LinhaItem linha, CheckBox marcado)
        {
            if (!marcado.Checked)
            {
                // se desmarcou, liberamos combobox
                linha.combo.DataSource = null;
                linha.combo.Enabled = false;
                return;
            }

            // desmarca os outros da mesma linha
            if (marcado != linha.chkPRJ) linha.chkPRJ.Checked = false;
            if (marcado != linha.chkOS) linha.chkOS.Checked = false;
            if (marcado != linha.chkJUST) linha.chkJUST.Checked = false;

            // popula combo conforme tipo
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
                linha.combo.DataSource = new string[] { "Lopes", "Limpeza", "Aguard. Desenho", "Manutenção", "Atestado" };
                linha.combo.Enabled = true;
            }
        }

        private void AtualizarTotal()
        {
            double soma = 0.0;

            foreach (var l in linhas)
            {
                string txt = l.txtHoras.Text.Trim();

                // Obrigatório HH:mm — ignora qualquer coisa vazia ou fora do padrão
                if (string.IsNullOrWhiteSpace(txt))
                    continue;

                // Validação estrita no formato 00:00
                if (!DateTime.TryParseExact(
                        txt,
                        "HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime hora))
                {
                    // Se inválido, zera o campo e ignora
                    l.txtHoras.BackColor = Color.LightCoral;  // feedback visual
                    continue;
                }
                else
                {
                    l.txtHoras.BackColor = Color.White; // válido
                }

                // Converte HH:mm → decimal (ex: 05:30 = 5,5)
                soma += hora.Hour + (hora.Minute / 60.0);
            }

            txtTotal.Text = soma.ToString("0.##");
        }

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // valida mínimo
            if (!double.TryParse(txtTotal.Text, out double total)) total = 0.0;

            if (total < minHoras)
            {
                MessageBox.Show($"Erro: total ({total:0.##}h). Requer um mínimo de ({minHoras}h).", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // validações por linha (ex.: se marcada, tem seleção)
            for (int i = 0; i < linhas.Length; i++)
            {
                var l = linhas[i];
                if (l.chkPRJ.Checked || l.chkOS.Checked || l.chkJUST.Checked)
                {
                    if (l.combo.SelectedItem == null)
                    {
                        MessageBox.Show($"Linha {i + 1}: selecione peça/justificativa.", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            MessageBox.Show("Validação OK. (Agora você pode conectar ao ACCDB para salvar.)", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}