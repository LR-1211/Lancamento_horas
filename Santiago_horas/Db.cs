using System.Data.OleDb;

namespace Santiago_horas
{
    public static class Db
    {
        private static string caminho =
            @"Z:\CONTROLE ST\Dados\dbControlesv5.accdb";

        public static OleDbConnection GetConnection()
        {
            return new OleDbConnection(
                $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={caminho};Persist Security Info=False;");
        }
    }
}
