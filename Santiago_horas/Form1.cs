using System;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
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

                    string setores = ObterSetorDoProjeto(numeroProjeto);
                    CarregarPecasPorSetor(l, setores);
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

        private double ConverterHorasParaDouble(string horas)
        {
            if (!TimeSpan.TryParse(horas, out TimeSpan ts))
                return 0;

            return ts.TotalHours;
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
                        return "Ferramentaria";

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

        private (string setores, double custoHora)? ObterSetorECusto(string nomeFunc)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();

                // 1️⃣ Buscar FUNÇÃO do funcionário
                var cmdFunc = new OleDbCommand(
                    "SELECT funçaoFunc FROM Funcionarios WHERE nomeFunc = ?",
                    conn);

                cmdFunc.Parameters.Add("?", OleDbType.VarChar).Value = nomeFunc;

                var funcaoObj = cmdFunc.ExecuteScalar();
                if (funcaoObj == null)
                    return null;

                string funçaoFunc = funcaoObj.ToString().Trim();

                // 2️⃣ Buscar SETOR e CUSTO pela FUNÇÃO
                var cmdSetor = new OleDbCommand(
                    "SELECT setores, custoHora FROM Setores WHERE setores = ?",
                    conn);

                cmdSetor.Parameters.Add("?", OleDbType.VarChar).Value = funçaoFunc;

                using (var reader = cmdSetor.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string setores = reader.GetString(0);         
                        double custoHora = Convert.ToDouble(reader.GetValue(1));

                        return (setores, custoHora);
                    }
                }
            }

            return null;
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

        private int? ObterCodigoProjeto(string numeroPro)
        {
            using (var conn = Db.GetConnection())
            {
                conn.Open();

                var cmd = new OleDbCommand(
                    @"SELECT Código
              FROM Projetos
              WHERE numeroPro = ?", conn);

                cmd.Parameters.Add("?", OleDbType.VarChar).Value = numeroPro;

                var result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                    return null;

                return Convert.ToInt32(result);
            }
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

            if (!TimeSpan.TryParse(linha.Horas, out TimeSpan ts))
            {
                MessageBox.Show("Horas inválidas.");
                return false;
            }

            if (ts.TotalHours <= 0)
            {
                MessageBox.Show("Informe as horas.");
                return false;
            }

            return true;
        }


        private bool SalvarLancamentosOS()
        {
            bool gravou = false;

            string funcionario = comboFuncionario.SelectedItem.ToString();
            DateTime data = dataPicker.Value.Date;

            using (var conn = Db.GetConnection())
            {
                conn.Open();

                foreach (var linha in linhas)
                {
                    if (linha == null) continue;
                    if (!linha.IsOS) continue;


                    var dadosSetor = ObterSetorECusto(funcionario);
                    if (dadosSetor == null)
                    {
                        MessageBox.Show($"Setor não encontrado para funcionário: {funcionario}");
                        continue;
                    }


                    string setores = dadosSetor.Value.setores;
                    double valorHora = dadosSetor.Value.custoHora;
                    

                    if (!TimeSpan.TryParseExact(linha.Horas, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan ts))
                    {
                        MessageBox.Show($"Horas inválidas: {linha.Horas}");
                        continue;
                    }

                    if (ts.TotalHours <= 0)
                        continue;

                    double totalMoeda = ts.TotalHours * valorHora;

                    string numeroOS = linha.NumeroOS;
                    if (string.IsNullOrWhiteSpace(numeroOS)) continue;
                    DateTime horaParaBanco = new DateTime(1899, 12, 30).AddHours(ts.Hours).AddMinutes(ts.Minutes);

                    var cmd = new OleDbCommand(
                        @"INSERT INTO [Valores Os]
                  (nOs, setor_forncOs, valorHora_unitOs, nHorasOs, tipo, totMatOs, funcionario, data)
                  VALUES (?, ?, ?, ?, ?, ?, ?, ?)", conn);

                    cmd.Parameters.Add("?", OleDbType.Integer).Value = int.Parse(numeroOS);
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = setores;
                    cmd.Parameters.Add("?", OleDbType.Double).Value = valorHora;
                    cmd.Parameters.Add("@horas", OleDbType.Date).Value = horaParaBanco;
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = 1;
                    cmd.Parameters.Add("?", OleDbType.Currency).Value = totalMoeda;
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = funcionario;
                    cmd.Parameters.Add("?", OleDbType.Date).Value = data;


                    cmd.ExecuteNonQuery();
                    gravou = true;
                }
            }
            return gravou;
        }


        private bool SalvarLancamentosPJ()
        {
            bool gravou = false;

            string funcionario = comboFuncionario.SelectedItem.ToString();
            DateTime data = dataPicker.Value.Date;
            using (var conn = Db.GetConnection())
            {
                conn.Open();

                foreach (var linha in linhas)
                {
                    if (linha == null) continue;
                    if (!linha.IsPRJ) continue;

                    var dadosSetor = ObterSetorECusto(funcionario);
                    if (dadosSetor == null)
                        continue;

                    string setor = dadosSetor.Value.setores;
                    double valorHora = dadosSetor.Value.custoHora;

                    if (!TimeSpan.TryParseExact(linha.Horas, @"hh\:mm", CultureInfo.InvariantCulture, out TimeSpan ts))
                    {
                        MessageBox.Show($"Horas inválidas: {linha.Horas}");
                        continue;
                    }

                    if (ts.TotalHours <= 0)
                        continue;

                    double totalMoeda = ts.TotalHours * valorHora;

                    string numeroPro = linha.Projeto;
                    if (string.IsNullOrWhiteSpace(numeroPro)) continue;
                    DateTime horaParaBanco = new DateTime(1899, 12, 30).AddHours(ts.Hours).AddMinutes(ts.Minutes);

                    int? codigoProjeto = ObterCodigoProjeto(numeroPro);
                    if (codigoProjeto == null)
                        continue;

                    string peca = linha.Peca;
                    if (string.IsNullOrWhiteSpace(peca))
                        continue;

                    var cmd = new OleDbCommand(
                        @"INSERT INTO [Valores Pj]
                  (CódigoPj, func_matPj, setor_forncPj, valorHora_unitPj, nHorasPj, tipoPj, totMatPj, data, funcionario)
                  VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)", conn);

                    // ⚠ ORDEM E TIPO ABSOLUTAMENTE CORRETOS
                    cmd.Parameters.Add("?", OleDbType.Integer).Value = codigoProjeto.Value; // CodigoPj
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = peca;                // func_matPj
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = setor;
                    cmd.Parameters.Add("?", OleDbType.Double).Value = valorHora;
                    cmd.Parameters.Add("@horas", OleDbType.Date).Value = horaParaBanco;
                    cmd.Parameters.Add("?", OleDbType.Boolean).Value = true;
                    cmd.Parameters.Add("?", OleDbType.Currency).Value = totalMoeda;
                    cmd.Parameters.Add("?", OleDbType.Date).Value = data;                  // data
                    cmd.Parameters.Add("?", OleDbType.VarChar).Value = funcionario;        // funcionario

                    cmd.ExecuteNonQuery();
                    gravou = true;
                }
            }
            return gravou;
        }

        private bool SalvarLancamentosJUST()
        {
            bool gravou = false;

            // Verifica se há um funcionário selecionado para evitar erros
            if (comboFuncionario.SelectedItem == null)
            {
                return false;
            }

            string nomeFunc = comboFuncionario.SelectedItem.ToString();
            DateTime dataDia = dataPicker.Value.Date;

            using (var conn = Db.GetConnection())
            {
                try
                {
                    conn.Open();

                    foreach (var linha in linhas)
                    {
                        // Pula linhas vazias ou que não sejam de justificativa
                        if (linha == null || !linha.IsJust) continue;

                        // Tenta converter o texto da linha para TimeSpan (duração)
                        if (!TimeSpan.TryParseExact(linha.Horas,@"hh\:mm",CultureInfo.InvariantCulture,out TimeSpan ts))
                        {
                            MessageBox.Show($"Horas inválidas: {linha.Horas}");
                            continue;
                        }

                        // Não salva se a hora for zero ou negativa
                        if (ts.TotalHours <= 0) continue;

                        // Obtém a justificativa do ComboBox da linha
                        string justificativa = linha.combo.SelectedItem?.ToString();
                        if (string.IsNullOrWhiteSpace(justificativa)) continue;

                        // --- O SEGREDO PARA O ACCESS ---
                        // Criamos a data base 30/12/1899 (o "zero" do Access) e somamos as horas e minutos.
                        // Isso evita que a data de hoje (2026) apareça no banco.
                        DateTime horaParaBanco = new DateTime(1899, 12, 30).AddHours(ts.Hours).AddMinutes(ts.Minutes);

                        var cmd = new OleDbCommand(
                            @"INSERT INTO [Analise Justificativas]
                    (data, nHorasJus, ID_Justificativas, nomeFunc)
                    VALUES (?, ?, ?, ?)",
                            conn);

                        // Adicionando os parâmetros na ordem EXATA das interrogações (?)
                        cmd.Parameters.Add("@data", OleDbType.Date).Value = dataDia;
                        cmd.Parameters.Add("@horas", OleDbType.Date).Value = horaParaBanco;
                        cmd.Parameters.Add("@just", OleDbType.VarChar).Value = justificativa;
                        cmd.Parameters.Add("@func", OleDbType.VarChar).Value = nomeFunc;

                        cmd.ExecuteNonQuery();
                        gravou = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Erro ao gravar lançamentos:\n" + ex.Message,
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            return gravou;
        }


        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            bool gravouJUST = false;
            bool gravouOS = false;
            bool gravouPJ = false;
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

            // ===== GRAVAÇÃO =====
            

            try
            {
                gravouOS = SalvarLancamentosOS();
                gravouPJ = SalvarLancamentosPJ();
                gravouJUST = SalvarLancamentosJUST();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Erro ao gravar lançamentos:\n" + ex.Message,
                    "Banco de Dados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // ===== FEEDBACK AO USUÁRIO =====
            if (gravouOS && gravouPJ && gravouJUST)
            {
                MessageBox.Show(
                    "Lançamentos de OS, Projeto e Justificativa realizados com sucesso.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (gravouOS && gravouPJ)
            {
                MessageBox.Show(
                    "Lançamento de OS e Projeto realizado com sucesso.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (gravouOS)
            {
                MessageBox.Show(
                    "Lançamento de OS realizado com sucesso.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (gravouPJ)
            {
                MessageBox.Show(
                    "Lançamento de Projeto realizado com sucesso.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if (gravouJUST)
            {
                MessageBox.Show(
                    "Lançamento da Justificativa realizado com sucesso.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "Nenhum lançamento foi realizado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

    }
}
