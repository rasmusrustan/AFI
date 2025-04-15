using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
namespace Api_Labb1.Models
{
    public class Annonsormetoder
    {
        public Annonsormetoder() { }

        private string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Annonser;Integrated Security=True;";
       
        /*----------------------------------CREATE-----------------------------------------------*/
        public int InsertAnnonsor(Annonsor annonsor, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string sqlstring = @"INSERT INTO tbl_annonsorer (ann_typ, ann_namn, ann_telefon, ann_adress, ann_postnummer, ann_ort,
                 ann_organisationsnummer, ann_faktura_adress, ann_faktura_postnummer, ann_faktura_ort)
                VALUES (@typ, @namn, @telefon, @adress, @postnummer, @ort,@orgnummer, @fakturaadress, @fakturapostnummer, @fakturaort)";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@typ", SqlDbType.NVarChar, 100).Value = annonsor.Typ;
            sqlCommand.Parameters.Add("@namn", SqlDbType.NVarChar, 20).Value = annonsor.Namn;
            sqlCommand.Parameters.Add("@telefon", SqlDbType.NVarChar, 255).Value = annonsor.Telefonnummer;
            sqlCommand.Parameters.Add("@adress", SqlDbType.NVarChar, 10).Value = annonsor.Adress;
            sqlCommand.Parameters.Add("@postnummer", SqlDbType.NVarChar, 100).Value = annonsor.Postnummer;
            sqlCommand.Parameters.Add("@ort", SqlDbType.NVarChar, 100).Value = annonsor.Ort;
            sqlCommand.Parameters.Add("@orgnummer", SqlDbType.NVarChar, 20).Value =
                 string.IsNullOrEmpty(annonsor.Organisationsnummer) ? DBNull.Value : annonsor.Organisationsnummer;
            sqlCommand.Parameters.Add("@fakturaadress", SqlDbType.NVarChar, 255).Value =
                string.IsNullOrEmpty(annonsor.FakturaAdress) ? DBNull.Value : annonsor.FakturaAdress;
            sqlCommand.Parameters.Add("@fakturapostnummer", SqlDbType.NVarChar, 10).Value =
                string.IsNullOrEmpty(annonsor.FakturaPostnummer) ? DBNull.Value : annonsor.FakturaPostnummer;
            sqlCommand.Parameters.Add("@fakturaort", SqlDbType.NVarChar, 100).Value =
                string.IsNullOrEmpty(annonsor.FakturaOrt) ? DBNull.Value : annonsor.FakturaOrt;
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
                    errormsg = "Insert Succeded";
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
        /*----------------------------READ----------------------------------------*/

        public List<Annonsor> GetAllAnnonsor(out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string sqlstring = "SELECT * FROM tbl_annonsorer";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataset = new DataSet();

            List<Annonsor> annonsorList = new List<Annonsor>();

            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataset, "Annonser");

                int count = dataset.Tables["Annonser"].Rows.Count;
                if (count > 0)
                {
                    foreach (DataRow row in dataset.Tables["Annonser"].Rows)
                    {
                        var annonsor = new Annonsor
                        {
                            ID = Convert.ToInt32(row["ann_id"]),
                            Typ = row["ann_typ"].ToString(),
                            Namn = row["ann_namn"].ToString(),
                            Telefonnummer = row["ann_telefon"].ToString(),
                            Adress = row["ann_adress"].ToString(),
                            Postnummer = row["ann_postnummer"].ToString(),
                            Ort = row["ann_ort"].ToString(),
                            Organisationsnummer = row["ann_organisationsnummer"].ToString(),
                            FakturaAdress = row["ann_faktura_adress"].ToString(),
                            FakturaPostnummer = row["ann_faktura_postnummer"].ToString(),
                            FakturaOrt = row["ann_faktura_ort"].ToString()
                        };

                        annonsorList.Add(annonsor);
                    }

                    errormsg = "";
                    return annonsorList;
                }
                else
                {
                    errormsg = "No annonsor found";
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
                sqlConnection.Close();
            }
        }

        [Obsolete]
        public Annonsor GetAnnonsorById(int ann_id, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string sqlstring = "SELECT * FROM tbl_annonsorer WHERE ann_id = @ann_id";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@ann_id", SqlDbType.Int).Value = ann_id;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataset = new DataSet();

            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataset, "Annonser");

                int count = dataset.Tables["Annonser"].Rows.Count;
                if (count == 1)
                {
                    var annonsor = new Annonsor
                    {
                        ID = Convert.ToInt32(dataset.Tables["Annonser"].Rows[0]["ann_id"]),  
                        Typ = dataset.Tables["Annonser"].Rows[0]["ann_typ"].ToString(),  
                        Namn = dataset.Tables["Annonser"].Rows[0]["ann_namn"].ToString(),  
                        Telefonnummer = dataset.Tables["Annonser"].Rows[0]["ann_telefon"].ToString(),  
                        Adress = dataset.Tables["Annonser"].Rows[0]["ann_adress"].ToString(),  
                        Postnummer = dataset.Tables["Annonser"].Rows[0]["ann_postnummer"].ToString(),  
                        Ort = dataset.Tables["Annonser"].Rows[0]["ann_ort"].ToString(),  

                       
                        Organisationsnummer = dataset.Tables["Annonser"].Rows[0]["ann_organisationsnummer"].ToString(),  
                        FakturaAdress = dataset.Tables["Annonser"].Rows[0]["ann_faktura_adress"].ToString(), 
                        FakturaPostnummer = dataset.Tables["Annonser"].Rows[0]["ann_faktura_postnummer"].ToString(),  
                        FakturaOrt = dataset.Tables["Annonser"].Rows[0]["ann_faktura_ort"].ToString() 
                    };
                    errormsg = "";
                    return annonsor;
                }
                else
                {
                    errormsg = "No annonsor found";
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
                sqlConnection.Close();
            }

        }


        /*------------------------------------UPDATE-------------------------------------------*/
        public Annonsor UpdateAnnonsor(Annonsor annonsor, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            string sqlstring = @"UPDATE tbl_annonsorer 
                         SET ann_typ = @typ, ann_namn = @namn, ann_telefon = @telefon, ann_adress = @adress, 
                             ann_postnummer = @postnummer, ann_ort = @ort, ann_organisationsnummer = @orgnummer, ann_faktura_adress = @fakturaadress, 
                             ann_faktura_postnummer = @fakturapostnummer, ann_faktura_ort = @fakturaort 
                         WHERE ann_id = @id";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = annonsor.ID;
            sqlCommand.Parameters.Add("@typ", SqlDbType.NVarChar, 100).Value = annonsor.Typ;
            sqlCommand.Parameters.Add("@namn", SqlDbType.NVarChar, 20).Value = annonsor.Namn;
            sqlCommand.Parameters.Add("@telefon", SqlDbType.NVarChar, 255).Value = annonsor.Telefonnummer;
            sqlCommand.Parameters.Add("@adress", SqlDbType.NVarChar, 10).Value = annonsor.Adress;
            sqlCommand.Parameters.Add("@postnummer", SqlDbType.NVarChar, 100).Value = annonsor.Postnummer;
            sqlCommand.Parameters.Add("@ort", SqlDbType.NVarChar, 100).Value = annonsor.Ort;
            sqlCommand.Parameters.Add("@orgnummer", SqlDbType.NVarChar, 20).Value = annonsor.Organisationsnummer;
            sqlCommand.Parameters.Add("@fakturaadress", SqlDbType.NVarChar, 255).Value = annonsor.FakturaAdress;
            sqlCommand.Parameters.Add("@fakturapostnummer", SqlDbType.NVarChar, 10).Value = annonsor.FakturaPostnummer;
            sqlCommand.Parameters.Add("@fakturaort", SqlDbType.NVarChar, 100).Value = annonsor.FakturaOrt;

            try
            {
                sqlConnection.Open();

                int i = sqlCommand.ExecuteNonQuery();
                if (i > 0)
                {
                    errormsg = "";
                    return annonsor;
                }
                else
                {
                    errormsg = "Delete failed";
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
                sqlConnection.Close();
            }

        }


        /*------------------------------------DELETE-------------------------------------------*/
        [Obsolete]
        public bool DeleteAnnonsor(int id, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            string sqlstring = @"DELETE FROM tbl_annonsorer WHERE ann_id = @id";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.Add("@id", SqlDbType.Int).Value = id;

            try
            {
                sqlConnection.Open();

                int i = sqlCommand.ExecuteNonQuery();
                if (i > 0)
                {
                    errormsg = "";
                    return true;
                }
                else
                {
                    errormsg = " <<<<<<<Success>>>>>>>";
                    return false;
                }

            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return false;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
