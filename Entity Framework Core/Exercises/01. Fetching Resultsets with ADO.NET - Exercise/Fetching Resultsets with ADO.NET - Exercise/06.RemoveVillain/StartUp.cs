namespace RemoveVillain
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
            int id = int.Parse(Console.ReadLine());

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlTransaction transaction = connection.BeginTransaction();

            try
            {
                using (connection)
                {
                    string currentVilian = "SELECT Name FROM Villains " +
                                            "WHERE Id = @villainId";
                    SqlCommand command = new SqlCommand(currentVilian, connection);
                    command.Parameters.AddWithValue("@villainId", id);
                    command.Transaction = transaction;
                    string vilianName = (string)command.ExecuteScalar();

                    if (vilianName == null)
                    {
                        Console.WriteLine("No such villain was found.");
                    }
                    else
                    {
                        string deleteVilian = @"DELETE FROM MinionsVillains 
                                            WHERE VillainId = @villainId";
                        command = new SqlCommand(deleteVilian, connection);
                        command.Parameters.AddWithValue("@villainId", id);
                        command.Transaction = transaction;
                        command.ExecuteNonQuery();

                        string deleteVilianId = @"DELETE FROM Villains
                                              WHERE Id = @villainId";
                        command = new SqlCommand(deleteVilianId, connection);
                        command.Parameters.AddWithValue("@villainId", id);
                        command.Transaction = transaction;
                        int numberOfMinions = command.ExecuteNonQuery();

                        transaction.Commit();

                        Console.WriteLine($"{vilianName} was deleted.");
                        Console.WriteLine($"{numberOfMinions} minions were released.");

                    }
                }
            }
            catch (Exception e)
            {
                try
                {
                    Console.WriteLine(e.Message);
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
