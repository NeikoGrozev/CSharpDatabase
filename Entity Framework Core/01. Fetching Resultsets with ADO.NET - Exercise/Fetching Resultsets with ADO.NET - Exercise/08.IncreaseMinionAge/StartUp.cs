namespace IncreaseMinionAge
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    class StartUp
    {
        static void Main()
        {
            List<int> id = Console.ReadLine()
                .Split()
                .Select(int.Parse)
                .ToList();

            string connectionString = "Server=NEIKO\\SQLEXPRESS;" +
                                     "Database=MinionsDb;" +
                                     "Integrated Security=true";

            SqlConnection conection = new SqlConnection(connectionString);
            conection.Open();

            try
            {
                using (conection)
                {
                    string updateString = @" UPDATE Minions
                                         SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1
                                         WHERE Id = @Id";

                    for (int i = 0; i < id.Count; i++)
                    {
                        SqlCommand command = new SqlCommand(updateString, conection);
                        command.Parameters.AddWithValue("@Id", id[i]);
                        command.ExecuteNonQuery();
                    }

                    string selectAllMinions = "SELECT Name, Age FROM Minions";
                    SqlCommand selectCommand = new SqlCommand(selectAllMinions, conection);
                    SqlDataReader reader = selectCommand.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            string name = (string)reader[0];
                            int age = (int)reader[1];

                            Console.WriteLine($"{name} {age}");
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
