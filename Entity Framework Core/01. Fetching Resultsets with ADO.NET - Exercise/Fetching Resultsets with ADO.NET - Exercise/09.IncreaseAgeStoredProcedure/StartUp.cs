namespace IncreaseAgeStoredProcedure
{
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        static void Main()
        {
            string connectionString = "Server=NEIKO\\SQLEXPRESS;" +
                                     "Database=MinionsDb;" +
                                     "Integrated Security=true";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            int id = int.Parse(Console.ReadLine());

            try
            {
                using (connection)
                {
                    string commandString = @"EXEC usp_GetOlder @id";
                    SqlCommand command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();

                    commandString = "SELECT Name, Age FROM Minions " +
                                    "WHERE Id = @id";
                    command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            string name = (string)reader[0];
                            int age = (int)reader[1];

                            Console.WriteLine($"{name} – {age} years old");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
