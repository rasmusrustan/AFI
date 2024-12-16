using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace BattleShits.Models
{
    public class DatabaseMethods
    {
        //Konstruktor
        public DatabaseMethods() { }

        SqlConnection sqlConnection = new SqlConnection
        {
            ConnectionString = "Data Source=battleshipsserver.database.windows.net;Initial Catalog=Battleships;User ID=sqladmin;Password=********;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"
        };

        // Publika metoder
        public int getFirstNullShip(int gameId)
        {

            SqlCommand sqlCommand = new SqlCommand(sqlstring, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@gameId", gameId);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            DataSet dataSet = new DataSet();

            try
            {
                sqlConnection.Open();

                sqlDataAdapter.Fill(dataSet, "Game");

                int board1Id = -1;

                board1Id = dataSet.Tables["Tbl_Person"].Rows.Count;

                if (count > 0)
                {
                    while (i < count)
                    {
                        PersonDetails personDetails = new PersonDetails();
                        personDetails.Id = Convert.ToUInt16(dataSet.Tables["Tbl_Person"].Rows[i]["Pe_Id"]);
                        personDetails.Name = dataSet.Tables["Tbl_Person"].Rows[i]["Pe_Namn"].ToString();

                        i++;
                        personDetailsList.Add(personDetails);
                    }
                    errormsg = "";
                    return personDetailsList;
                }
                else
                {
                    errormsg = "No person details exist in database";
                    return null;
                }
            }
            catch (Exception e)
            {
                errormsg = e.Message;
                return null;
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
