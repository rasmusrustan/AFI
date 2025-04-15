using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Api_Labb1.Models
{
    public class Annonsmetoder
    {
        public Annonsmetoder() { }

        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Annonser;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

        /*----------------------------------CREATE-----------------------------------------------*/
        [Obsolete]
        public int InsertAnnons(Annons annons, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string sqlstring = @"INSERT INTO tbl_ads (ann_id, ad_rubrik, ad_innehall, ad_pris, ad_annonspris)
                           VALUES (@ann_id, @ad_rubrik, @ad_innehall, @ad_pris, @ad_annonspris)"
            ;

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@ann_id", SqlDbType.NVarChar, 100).Value = annons.AnnonsorID;
            sqlCommand.Parameters.Add("@ad_rubrik", SqlDbType.NVarChar, 20).Value = annons.Rubrik;
            sqlCommand.Parameters.Add("@ad_innehall", SqlDbType.NVarChar, 255).Value = annons.Innehall;
            sqlCommand.Parameters.Add("@ad_pris", SqlDbType.NVarChar, 10).Value = annons.Pris;
            sqlCommand.Parameters.Add("@ad_annonspris", SqlDbType.NVarChar, 100).Value = annons.Annonspris;

            try
            {
                sqlConnection.Open();
                int i = sqlCommand.ExecuteNonQuery();
                if (i == 0)
                {
                    errormsg = "";
                }
                else
                {
                    errormsg = "Insert failed";
                }
                return i;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
            finally
            {
                sqlConnection.Close();
            }



        }

        ///*----------------------------READ----------------------------------------*/
        
        //[Obsolete]
        //public List<Annons> GetAllAnnonser(out string errormsg)
        //{

        //}
        //[Obsolete]
        //public Annons GetAnnonsById(int id, out string errormsg)
        //{
        //}
        ///*------------------------------------UPDATE-------------------------------------------*/
        //[Obsolete]
        //public Annons UpdateAnnons(Annons annons, out string errormsg)
        //{
        //}

        ///*------------------------------------DELETE-------------------------------------------*/
        //[Obsolete]
        //public bool DeleteAnnons(int id, out string errormsg)
        //{
        //}
        
    }
}
