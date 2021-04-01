namespace PrintAllMinionNames
{
    using System;
    using System.Collections.Generic;
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

            List<string> minionsName = new List<string>();

            try
            {
                using (connection)
                {
                    string allMinions = "SELECT Name FROM Minions";

                    SqlCommand command = new SqlCommand(allMinions, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    using (reader)
                    {
                        while (reader.Read())
                        {
                            minionsName.Add((string)reader[0]);
                        }
                    }
                }

                Console.WriteLine(string.Join(", ", minionsName));

                while (minionsName.Count != 0)
                {
                    if (minionsName[0] != null)
                    {
                        Console.WriteLine(minionsName[0]);
                        minionsName.RemoveAt(0);
                    }

                    if (minionsName.Count == 0)
                    {
                        break;
                    }

                    if (minionsName[minionsName.Count - 1] != null)
                    {
                        Console.WriteLine(minionsName[minionsName.Count - 1]);
                        minionsName.RemoveAt(minionsName.Count - 1);
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
