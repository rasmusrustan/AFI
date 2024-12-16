using Microsoft.Data.SqlClient;

namespace BattleShits.Models
{
    public class UsersMethods
    {
        private readonly string _connectionString;

        public UsersMethods(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Users GetUserByUsername(string username)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM [Users] WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Users
                        {
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString()
                        };
                    }
                }
            }

            return null;
        }

        public void AddUser(Users user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "INSERT INTO [Users] (Username, Password) VALUES (@Username, @Password)",
                    connection
                );

                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@Password", user.Password);

                command.ExecuteNonQuery();
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(
                    "SELECT COUNT(*) FROM [Users] WHERE Username = @Username AND Password = @Password",
                    connection
                );

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);

                return (int)command.ExecuteScalar() > 0;
            }
        }
    }
}
