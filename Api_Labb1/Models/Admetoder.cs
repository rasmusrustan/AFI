using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Api_Labb1.Models
{
    public class AdMetoder
    {
        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Annonser;Integrated Security=True;";

        /*-------------------------------CREATE-----------------------------------*/
        public int InsertAd(Ad ad, out string errormsg)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = @"INSERT INTO tbl_ads (ann_id, ad_rubrik, ad_innehall, ad_pris, ad_annonspris)
                           VALUES (@annonsorID, @rubrik, @innehall, @pris, @annonspris)";

            SqlCommand cmd = new SqlCommand(sql, connection);

            cmd.Parameters.Add("@annonsorID", SqlDbType.Int).Value = ad.AnnonsorID;
            cmd.Parameters.Add("@rubrik", SqlDbType.VarChar, 255).Value = ad.Rubrik;
            cmd.Parameters.Add("@innehall", SqlDbType.Text).Value = ad.Innehall;
            cmd.Parameters.Add("@pris", SqlDbType.Decimal).Value = ad.Pris;
            cmd.Parameters.Add("@annonspris", SqlDbType.Decimal).Value = ad.AnnonsPris;

            try
            {
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                errormsg = i > 0 ? "" : "Insert failed";
                return i;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return 0;
            }
            finally
            {
                connection.Close();
            }
        }

        /*-------------------------------READ--------------------------------*/
        public List<Ad> GetAllAds(out string errormsg)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = "SELECT * FROM tbl_ads";
            SqlCommand cmd = new SqlCommand(sql, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();

            List<Ad> adList = new List<Ad>();

            try
            {
                connection.Open();
                adapter.Fill(dataset, "Ads");

                foreach (DataRow row in dataset.Tables["Ads"].Rows)
                {
                    var ad = new Ad
                    {
                        ID = Convert.ToInt32(row["ad_id"]),
                        AnnonsorID = Convert.ToInt32(row["ann_id"]),
                        Rubrik = row["ad_rubrik"].ToString(),
                        Innehall = row["ad_innehall"].ToString(),
                        Pris = Convert.ToDecimal(row["ad_pris"]),
                        AnnonsPris = Convert.ToDecimal(row["ad_annonspris"])
                    };

                    adList.Add(ad);
                }

                errormsg = "";
                return adList;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        /*-------------------------------READ------------------------------*/
        public Ad GetAdById(int id, out string errormsg)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = "SELECT * FROM tbl_ads WHERE ad_id = @id";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataSet dataset = new DataSet();

            try
            {
                connection.Open();
                adapter.Fill(dataset, "Ads");

                if (dataset.Tables["Ads"].Rows.Count == 1)
                {
                    var row = dataset.Tables["Ads"].Rows[0];
                    var ad = new Ad
                    {
                        ID = Convert.ToInt32(row["ad_id"]),
                        AnnonsorID = Convert.ToInt32(row["ann_id"]),
                        Rubrik = row["ad_rubrik"].ToString(),
                        Innehall = row["ad_innehall"].ToString(),
                        Pris = Convert.ToDecimal(row["ad_pris"]),
                        AnnonsPris = Convert.ToDecimal(row["ad_annonspris"])
                    };

                    errormsg = "";
                    return ad;
                }
                else
                {
                    errormsg = "Ad not found";
                    return null;
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        /*-------------------------------UPDATE----------------------------------*/
        public Ad UpdateAd(Ad ad, out string errormsg)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = @"UPDATE tbl_ads SET 
                            ann_id = @annonsorID, 
                            ad_rubrik = @rubrik,
                            ad_innehall = @innehall,
                            ad_pris = @pris,
                            ad_annonspris = @annonspris
                           WHERE ad_id = @id";

            SqlCommand cmd = new SqlCommand(sql, connection);

            cmd.Parameters.Add("@id", SqlDbType.Int).Value = ad.ID;
            cmd.Parameters.Add("@annonsorID", SqlDbType.Int).Value = ad.AnnonsorID;
            cmd.Parameters.Add("@rubrik", SqlDbType.VarChar, 255).Value = ad.Rubrik;
            cmd.Parameters.Add("@innehall", SqlDbType.Text).Value = ad.Innehall;
            cmd.Parameters.Add("@pris", SqlDbType.Decimal).Value = ad.Pris;
            cmd.Parameters.Add("@annonspris", SqlDbType.Decimal).Value = ad.AnnonsPris;

            try
            {
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                if (i > 0)
                {
                    errormsg = "";
                    return ad;
                }
                else
                {
                    errormsg = "Update failed";
                    return null;
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return null;
            }
            finally
            {
                connection.Close();
            }
        }

        /*-------------------------------DELETE----------------------------------*/
        public bool DeleteAd(int id, out string errormsg)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            string sql = "DELETE FROM tbl_ads WHERE ad_id = @id";
            SqlCommand cmd = new SqlCommand(sql, connection);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

            try
            {
                connection.Open();
                int i = cmd.ExecuteNonQuery();
                errormsg = i > 0 ? "" : "Delete failed";
                return i > 0;
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return false;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
