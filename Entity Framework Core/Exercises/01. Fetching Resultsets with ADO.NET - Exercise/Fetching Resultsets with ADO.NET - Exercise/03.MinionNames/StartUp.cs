namespace MinionNames
{
    using System;
    using System.Data.SqlClient;

    class StartUp
    {
        private static string connectionString = "Server=NEIKO\\SQLEXPRESS;" +
                                                "Database=MinionsDb;" +
                                                "Integrated Security=true";
        static void Main()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                using (connection)
                {
                    int idVilian = int.Parse(Console.ReadLine());

                    string selectionVilian = "SELECT Name FROM Villains WHERE Id = @Id";
                    SqlCommand command = new SqlCommand(selectionVilian, connection);
                    command.Parameters.AddWithValue("@Id", idVilian);
                    string vilian = (string)command.ExecuteScalar();

                    if (vilian == null)
                    {
                        Console.WriteLine($"No villain with ID {idVilian} exists in the database.");
                    }
                    else
                    {
                        Console.WriteLine($"Villain: {vilian}");
                    }
                    
                    string selectMinionName = @$"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum,
                                                m.Name, 
                                                m.Age
                                                FROM MinionsVillains AS mv
                                                JOIN Minions As m ON mv.MinionId = m.Id
                                                WHERE mv.VillainId = @Id
                                                ORDER BY m.Name";

                    command = new SqlCommand(selectMinionName, connection);
                    command.Parameters.AddWithValue("@Id", idVilian);
                    SqlDataReader reader = command.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            if (reader == null)
                            {
                                Console.WriteLine("(no minions)");
                                break;
                            }

                            long rowNum = (long)reader[0];
                            string name = (string)reader[1];
                            int age = (int)reader[2];

                            Console.WriteLine($"{rowNum}. {name} {age}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
