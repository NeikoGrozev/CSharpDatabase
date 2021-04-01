namespace ChangeTownNamesCasing
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    class StartUp
    {
        private static string conectionString = "Server=NEIKO\\SQLEXPRESS;" +
                                                "Database=MinionsDb;" +
                                                "Integrated Security=true";
        static void Main()
        {
            string country = Console.ReadLine();

            SqlConnection connection = new SqlConnection(conectionString);
            connection.Open();

            using (connection)
            {
                try
                {
                    string countryString = "SELECT c.Id FROM Countries AS c " +
                                            "WHERE c.Name = @countryName";
                    SqlCommand command = new SqlCommand(countryString, connection);
                    command.Parameters.AddWithValue("@countryName", country);
                    int? countryId = (int?)command.ExecuteScalar();

                    bool isTrue = false;

                    if (countryId == null)
                    {
                        isTrue = true;
                    }
                    else
                    {
                        string updateTown = @"UPDATE Towns " +
                                             "SET Name = UPPER(Name) " +
                                             $"WHERE CountryCode = {(int)countryId}";
                        command = new SqlCommand(updateTown, connection);
                        int townCount = command.ExecuteNonQuery();

                        if (townCount == 0)
                        {
                            isTrue = true;
                        }
                        else
                        {
                            Console.WriteLine($"{townCount} town names were affected.");

                            string selectTown = @"SELECT t.Name 
                                                  FROM Towns as t
                                                  JOIN Countries AS c ON c.Id = t.CountryCode
                                                  WHERE c.Name = @countryName";
                            command = new SqlCommand(selectTown, connection);
                            command.Parameters.AddWithValue("@countryName", country);
                            SqlDataReader reader = command.ExecuteReader();

                            List<string> allTowns = new List<string>();
                            using (reader)
                            {
                                while (reader.Read())
                                {
                                    allTowns.Add((string)reader[0]);
                                }
                            }

                            Console.WriteLine($"[{string.Join(", ", allTowns)}]");
                        }
                    }

                    if(isTrue)
                    {
                        Console.WriteLine("No town names were affected.");
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
