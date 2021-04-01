namespace VillainNames
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

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                string selectVillian = @"SELECT v.Name, COUNT(mv.VillainId) AS MinionsCount  
                                         FROM Villains AS v 
                                         JOIN MinionsVillains AS mv ON v.Id = mv.VillainId 
                                         GROUP BY v.Id, v.Name 
                                         HAVING COUNT(mv.VillainId) > 3 
                                         ORDER BY COUNT(mv.VillainId)";

                try
                {
                    SqlCommand command = new SqlCommand(selectVillian, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            string name = (string)reader[0];
                            int count = (int)reader[1];

                            Console.WriteLine($"{name} - {count}");
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
}
