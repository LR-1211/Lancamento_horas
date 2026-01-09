using System;
using System.Data.OleDb;
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

            CorrigirLayoutPainel();
            InicializarHeader();
            CriarLinhas();
            WireHeaderEvents();
            AtualizarTotal();

            TestarConexaoDB();
            CarregarFuncionarios(); 
        }

        private void Form_1_Load(object sender, EventArgs e)
        {
            TestarConexaoDB();
            CarregarFuncionarios();
        }

        private void TestarConexaoDB()
        {
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    MessageBox.Show(
                        "Conexão com o banco de dados realizada com sucesso.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao conectar ao banco de dados:\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarFuncionarios()
        {
            try
            {    using (var conn = Db.GetConnection())
                {
                    conn.Open();
                    var cmd = new OleDbCommand(
                        "SELECT [nomeFunc] FROM Funcionarios ORDER BY [nomeFunc]",
                        conn);
                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            comboFuncionario.Items.Add(rd[0].ToString());
                        }
                    }
                }
                if (comboFuncionario.Items.Count > 0)
                    comboFuncionario.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar funcionários:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        // =========================================================
        // FIX LAYOUT
        // =========================================================
        private void CorrigirLayoutPainel()
        {
            panelOuter.Dock = DockStyle.Fill;
            panelOuter.Margin = new Padding(0);
            panelOuter.Padding = new Padding(0);
            panelOuter.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            painelLinhas.Dock = DockStyle.Fill;
            painelLinhas.Margin = new Padding(0);
            painelLinhas.Padding = new Padding(0);
            painelLinhas.AutoScroll = true;
            painelLinhas.HorizontalScroll.Enabled = false;
            painelLinhas.HorizontalScroll.Visible = false;
            painelLinhas.HorizontalScroll.Maximum = 0;
            painelLinhas.AutoScrollMinSize = new Size(0, 0);
        }

        // =========================================================
        // HEADER
        // =========================================================
        private void InicializarHeader()
        {
            comboFuncionario.Items.Clear();
            dataPicker.Value = DateTime.Today;
        }
        // =========================================================
        // LINHAS
        // =========================================================
        private void CriarLinhas()
        {
            painelLinhas.Controls.Clear();

            int y = 40;
            for (int i = 0; i < QTDE_LINHAS; i++)
            {
                var l = new LinhaItem();

                l.Base.Left = 105;
                l.Base.Top = y;
                l.Base.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                l.Base.Width = Math.Max(painelLinhas.ClientSize.Width - 20, 200);
                l.combo.SelectedIndexChanged += (s, e) =>
                {
                    if (!l.chkPRJ.Checked)
                        return;

                    string numeroProjeto = l.combo.SelectedItem?.ToString();
                    if (string.IsNullOrEmpty(numeroProjeto))
                        return;

                    string setor = ObterSetorDoProjeto(numeroProjeto);
                    CarregarPecasPorSetor(l, setor);
                };

                painelLinhas.Controls.Add(l.Base);
                linhas[i] = l;

                int idx = i;

                l.chkPRJ.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkPRJ);
                l.chkOS.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkOS);
                l.chkJUST.CheckedChanged += (s, e) => Exclusivo(linhas[idx], linhas[idx].chkJUST);

                l.txtHoras.TextChanged += (s, e) => AtualizarTotal();
                l.txtHoras.Text = "00:00";

                y += l.Base.Height + 13;
            }

            painelLinhas.Resize += (s, e) =>
            {
                foreach (var l in linhas)
                {
                    if (l != null)
                        l.Base.Width = painelLinhas.ClientSize.Width - 20;
                }
            };
        }

        // =========================================================
        // EVENTOS HEADER
        // =========================================================
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

        // =========================================================
        // EXCLUSIVIDADE + OS (BANCO)
        // =========================================================
        private void Exclusivo(LinhaItem linha, CheckBox marcado)
        {
            // SEMPRE resetar peças ao trocar o tipo
            linha.comboPeca.Items.Clear();
            linha.comboPeca.Enabled = false;

            if (!marcado.Checked)
            {
                CarregarProjetos(linha); // ← BANCO

            }

            if (marcado != linha.chkPRJ) linha.chkPRJ.Checked = false;
            if (marcado != linha.chkOS) linha.chkOS.Checked = false;
            if (marcado != linha.chkJUST) linha.chkJUST.Checked = false;

            if (marcado == linha.chkPRJ)
            {
                CarregarProjetos(linha); // ← BANCO
            }
            else if (marcado == linha.chkOS)
            {
                CarregarOrdensServico(linha); // ← BANCO
            }
            else if (marcado == linha.chkJUST)
            {
                CarregarJustificativas(linha); // ← BANCO
            }
        }

        private void CarregarOrdensServico(LinhaItem linha)
        {
            linha.combo.Items.Clear();
            linha.combo.Enabled = false;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT [numeroOS]
                  FROM [Ordem de serviço]
                  WHERE encerradaOS = 0
                  ORDER BY [numeroOS]",
                        conn);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            linha.combo.Items.Add(rd[0].ToString());
                        }
                    }
                }

                if (linha.combo.Items.Count > 0)
                {
                    linha.combo.SelectedIndex = 0;
                    linha.combo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar Ordens de Serviço:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarProjetos(LinhaItem linha)
        {
            linha.combo.Items.Clear();
            linha.combo.Enabled = false;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT numeroPro
                  FROM Projetos
                  WHERE encerradaOS = 0
                  ORDER BY numeroPro",
                        conn);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            linha.combo.Items.Add(rd[0].ToString());
                        }
                    }
                }

                if (linha.combo.Items.Count > 0)
                {
                    linha.combo.SelectedIndex = 0;
                    linha.combo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar Projetos:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarPecas(LinhaItem linha, string setorProjeto)
        {
            linha.comboPeca.Items.Clear();
            linha.comboPeca.Enabled = false;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT ID_Peca
                  FROM Lista_Pecas
                  WHERE Empresa = ?
                  ORDER BY ID_Peca",
                        conn);

                    cmd.Parameters.AddWithValue("?", setorProjeto);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            linha.comboPeca.Items.Add(rd["ID_Peca"].ToString());
                        }
                    }
                }

                if (linha.comboPeca.Items.Count > 0)
                {
                    linha.comboPeca.SelectedIndex = 0;
                    linha.comboPeca.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar Peças:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CarregarJustificativas(LinhaItem linha)
        {
            linha.combo.Items.Clear();
            linha.combo.Enabled = false;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT ID_Justificativas
                  FROM Justificativas
                  ORDER BY ID_Justificativas",
                        conn);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            linha.combo.Items.Add(rd[0].ToString());
                        }
                    }
                }

                if (linha.combo.Items.Count > 0)
                {
                    linha.combo.SelectedIndex = 0;
                    linha.combo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar Justificativas:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string ObterSetorDoProjeto(string numeroProjeto)
        {
            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT setorT
                          FROM Projetos
                          WHERE numeroPro = ?",
                        conn);

                    cmd.Parameters.AddWithValue("?", numeroProjeto);

                    var result = cmd.ExecuteScalar();
                    if (result == null) return null;

                    // Filtragem canonica
                    var setor = result.ToString().Trim().ToUpper();

                    if (setor == "MOLDES")
                        return "Moldes";

                    if (setor == "FERRAMENTARIA")
                        return "Ferramenaria";

                    return null;
                }
            }
            catch
            {
                return null;
            }
        }


        private void CarregarPecasPorSetor(LinhaItem linha, string setor)
        {
            linha.comboPeca.Items.Clear();
            linha.comboPeca.Enabled = false;

            if (string.IsNullOrEmpty(setor))
                return;

            try
            {
                using (var conn = Db.GetConnection())
                {
                    conn.Open();

                    var cmd = new OleDbCommand(
                        @"SELECT ID_Peca
                          FROM Lista_Pecas
                          WHERE Empresa = ?
                          ORDER BY ID_Peca",
                        conn);

                    cmd.Parameters.AddWithValue("?", setor);

                    using (var rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            linha.comboPeca.Items.Add(rd[0].ToString());
                        }
                    }
                }

                if (linha.comboPeca.Items.Count > 0)
                {
                    linha.comboPeca.SelectedIndex = 0;
                    linha.comboPeca.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao carregar Peças:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // TOTAL
        // =========================================================
        private void AtualizarTotal()
        {
            double soma = 0.0;

            foreach (var l in linhas)
            {
                if (l == null) continue;

                if (DateTime.TryParseExact(
                    l.txtHoras.Text.Trim(),
                    "HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime hora))
                {
                    l.txtHoras.BackColor = Color.White;
                    soma += hora.Hour + (hora.Minute / 60.0);
                }
                else
                {
                    l.txtHoras.BackColor = Color.LightCoral;
                }
            }

            txtTotal.Text = soma.ToString("0.##");
        }

        // =========================================================
        // SALVAR (VALIDAÇÃO)
        // =========================================================
        private bool ValidarLancamentoOS(LinhaItem linha)
        {
            if (!linha.IsOS)
                return false;

            if (string.IsNullOrWhiteSpace(linha.NumeroOS))  
            {
                MessageBox.Show("Informe a OS.");
                return false;
            }

            if (linha.Horas <= 0)
            {
                MessageBox.Show("Informe as horas.");
                return false;
            }

            return true;
        }


        private void SalvarLancamentosOS()
        {
            string funcionario = comboFuncionario.SelectedItem.ToString();
            DateTime data = dataPicker.Value.Date;

            using (var conn = Db.GetConnection())
            {
                conn.Open();

                foreach (var linha in linhas)
                {
                    if (linha == null) continue;

                    // 🔒 Só grava OS
                    if (!linha.IsOS) continue;

                    if (!TimeSpan.TryParse(linha.Horas, out TimeSpan ts))
                        continue;

                    decimal horas = (decimal)ts.TotalHours;

                    string numeroOS = linha.combo.SelectedItem?.ToString();
                    if (string.IsNullOrWhiteSpace(numeroOS)) continue;

                    var cmd = new OleDbCommand(
                        @"INSERT INTO [Valores Os]
                  (funcionario, nOs, data, nHorasOs)
                  VALUES (?, ?, ?, ?)", conn);

                    cmd.Parameters.AddWithValue("?", funcionario);
                    cmd.Parameters.AddWithValue("?", numeroOS);
                    cmd.Parameters.AddWithValue("?", data);
                    cmd.Parameters.AddWithValue("?", horas);

                    cmd.ExecuteNonQuery();
                }
            }
        }



        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            // ===== VALIDAÇÕES (INALTERADAS) =====
            if (comboFuncionario.SelectedItem == null)
            {
                MessageBox.Show(
                    "Erro: Obrigatório Preenchimento de Funcionário",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!double.TryParse(txtTotal.Text, out double total))
                total = 0.0;

            if (total < minHoras)
            {
                MessageBox.Show(
                    $"Erro: total ({total:0.##}h). Requer mínimo de ({minHoras}h).",
                    "Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // ===== GRAVAÇÃO OS =====
            SalvarLancamentosOS();

            MessageBox.Show(
                "Lançamentos de OS gravados com sucesso.",
                "Sucesso",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

    }
}
