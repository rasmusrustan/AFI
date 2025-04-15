using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Api_Labb1.Models
{
    public class Personmetoder
    {
        public Personmetoder() { }

        /*----------------------------------CREATE-----------------------------------------------*/

        [Obsolete]
        public int InsertPrenumerant(Persondetalj person, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Prenumeranter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            string sqlstring = @"INSERT INTO tbl_prenumeranter (prn_namn, prn_telefon, prn_adress, prn_postnummer, prn_ort) 
                         VALUES (@prn_namn, @prn_telefon, @prn_adress, @prn_postnummer, @prn_ort)";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@prn_namn", SqlDbType.NVarChar, 100).Value = person.Namn;
            sqlCommand.Parameters.Add("@prn_telefon", SqlDbType.NVarChar, 20).Value = person.Telefonnummer;
            sqlCommand.Parameters.Add("@prn_adress", SqlDbType.NVarChar, 255).Value = person.Utdelningsadress;
            sqlCommand.Parameters.Add("@prn_postnummer", SqlDbType.NVarChar, 10).Value = person.Postnummer;
            sqlCommand.Parameters.Add("@prn_ort", SqlDbType.NVarChar, 100).Value = person.Ort;

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
        /*----------------------------READ----------------------------------------*/
        [Obsolete]
        public List<Persondetalj> GetAllPersons(out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Prenumeranter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            string sqlstring = "SELECT * FROM tbl_prenumeranter";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataset = new DataSet();

            List<Persondetalj> persons = new List<Persondetalj>();
            errormsg = "";

            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataset, "Prenumeranter");

                foreach (DataRow row in dataset.Tables["Prenumeranter"].Rows)
                {
                    persons.Add(new Persondetalj
                    {
                        PrenumerantID = Convert.ToInt32(row["prn_id"]),
                        Namn = row["prn_namn"].ToString(),
                        Telefonnummer = row["prn_telefon"].ToString(),
                        Utdelningsadress = row["prn_adress"].ToString(),
                        Postnummer = row["prn_postnummer"].ToString(),
                        Ort = row["prn_ort"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
            }
            finally
            {
                sqlConnection.Close();
            }

            return persons;
        }
        [Obsolete]
        public Persondetalj GetPrenumerant(int prn_id, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Prenumeranter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            string sqlstring = "SELECT * FROM tbl_prenumeranter WHERE prn_id = @prn_id";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@prn_id", SqlDbType.Int).Value = prn_id;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            DataSet dataset = new DataSet();

            try
            {
                sqlConnection.Open();
                sqlDataAdapter.Fill(dataset, "Prenumeranter");

                int count = dataset.Tables["Prenumeranter"].Rows.Count;
                if (count == 1)
                {
                    var person = new Persondetalj
                    {
                        PrenumerantID = Convert.ToInt32(dataset.Tables["Prenumeranter"].Rows[0]["prn_id"]),
                        Namn = dataset.Tables["Prenumeranter"].Rows[0]["prn_namn"].ToString(),
                        Telefonnummer =dataset.Tables["Prenumeranter"].Rows[0]["prn_telefon"].ToString(),
                        Utdelningsadress = dataset.Tables["Prenumeranter"].Rows[0]["prn_adress"].ToString(),
                        Postnummer = dataset.Tables["Prenumeranter"].Rows[0]["prn_postnummer"].ToString(),
                        Ort = dataset.Tables["Prenumeranter"].Rows[0]["prn_ort"].ToString()
                    };
                    errormsg = "";
                    return person;
                }
                else
                {
                    errormsg = "No person found";
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
        [Obsolete]

        public Persondetalj UpdatePrenumerant(Persondetalj person, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Prenumeranter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            string sqlstring = @"UPDATE tbl_prenumeranter 
                         SET prn_namn = @prn_namn, prn_telefon = @prn_telefon, prn_adress = @prn_adress, 
                             prn_postnummer = @prn_postnummer, prn_ort = @prn_ort 
                         WHERE prn_id = @prn_id";

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@prn_id", SqlDbType.Int).Value = person.PrenumerantID;
            sqlCommand.Parameters.Add("@prn_namn", SqlDbType.NVarChar, 100).Value = person.Namn;
            sqlCommand.Parameters.Add("@prn_telefon", SqlDbType.NVarChar, 20).Value = person.Telefonnummer.ToString();
            sqlCommand.Parameters.Add("@prn_adress", SqlDbType.NVarChar, 255).Value = person.Utdelningsadress;
            sqlCommand.Parameters.Add("@prn_postnummer", SqlDbType.NVarChar, 10).Value = person.Postnummer.ToString();
            sqlCommand.Parameters.Add("@prn_ort", SqlDbType.NVarChar, 100).Value = person.Ort;

            try
            {
                sqlConnection.Open();
                int i = sqlCommand.ExecuteNonQuery();
                if (i > 0)
                {
                    errormsg = "";
                    return person;
                }
                else
                {
                    errormsg = "No records updated";
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

        public bool DeletePrenumerant(int prn_id, out string errormsg)
        {
            SqlConnection sqlConnection = new SqlConnection();
            sqlConnection.ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Prenumeranter;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            string sqlstring = "DELETE FROM tbl_prenumeranter WHERE prn_id = @prn_id";
            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);

            sqlCommand.Parameters.Add("@prn_id", SqlDbType.Int).Value = prn_id;

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
                    errormsg = "No records deleted";
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

        

    
