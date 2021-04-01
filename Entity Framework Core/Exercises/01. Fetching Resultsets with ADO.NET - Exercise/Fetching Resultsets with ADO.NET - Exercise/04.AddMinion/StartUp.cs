namespace AddMinion
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
            string[] minion = Console.ReadLine()
                .Split(" ");
            string[] vilian = Console.ReadLine()
                .Split(" ");

            string minionName = minion[1];
            int minionAge = int.Parse(minion[2]);
            string minionTown = minion[3];

            string vilianName = vilian[1];

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                try
                {
                    string commandString = "SELECT Id FROM Towns " +
                                    "WHERE Name = @townName";
                    SqlCommand command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@townName", minionTown);
                    int? currentTownId = (int?)command.ExecuteScalar();

                    if (currentTownId == null)
                    {
                        AddNewTown(connection, minionTown);
                        currentTownId = (int)command.ExecuteScalar();
                    }

                    commandString = "SELECT Id FROM Villains " +
                                            "WHERE Name = @name";
                    command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@name", vilianName);
                    int? currentVilianId = (int?)command.ExecuteScalar();

                    if (currentVilianId == null)
                    {
                        AddNewVilian(connection, vilianName);
                        currentVilianId = (int)command.ExecuteScalar();
                    }

                    commandString = "SELECT Id FROM Minions " +
                                    "WHERE Name = @name";
                    command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@name", minionName);
                    int? currentMinionId = (int?)command.ExecuteScalar();

                    if (currentMinionId == null)
                    {
                        AddNewMinion(connection, minionName, minionAge, (int)currentTownId);
                        currentMinionId = (int)command.ExecuteScalar();                       
                    }

                    commandString = "SELECT MinionId FROM MinionsVillains " +
                                    "WHERE MinionId = @currentMinionId AND VillainId = @currentVilianId";
                    command = new SqlCommand(commandString, connection);
                    command.Parameters.AddWithValue("@currentMinionId", currentMinionId);
                    command.Parameters.AddWithValue("@currentVilianId", currentVilianId);
                    int? currentId = (int?)command.ExecuteScalar();

                    if(currentId == null)
                    {
                        AddNewRelationshipInMinionsViliansTable(connection, (int)currentMinionId, (int)currentVilianId);
                        Console.WriteLine($"Successfully added {minionName} to be minion of {vilianName}.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void AddNewTown(SqlConnection connection, string minionTown)
        {
            string addNewTown = "INSERT INTO Towns (Name) VALUES " +
                                 "(@townName)";
            SqlCommand command = new SqlCommand(addNewTown, connection);
            command.Parameters.AddWithValue("@townName", minionTown);
            command.ExecuteNonQuery();

            Console.WriteLine($"Town {minionTown} was added to the database.");
        }

        private static void AddNewVilian(SqlConnection connection, string vilianName)
        {
            string addNewVilian = "INSERT INTO Villains (Name, EvilnessFactorId)  VALUES " +
                                    "(@villainName, 4)";
            SqlCommand command = new SqlCommand(addNewVilian, connection);
            command.Parameters.AddWithValue("@villainName", vilianName);
            command.ExecuteNonQuery();

            Console.WriteLine($"Villain {vilianName} was added to the database.");
        }

        private static void AddNewMinion(SqlConnection connection, string minionName, int minionAge, int currentTownId)
        {
            string addNewMinion = "INSERT INTO Minions (Name, Age, TownId)" +
                                   "VALUES (@name, @age, @townId)";
            SqlCommand command = new SqlCommand(addNewMinion, connection);
            command.Parameters.AddWithValue("@name", minionName);
            command.Parameters.AddWithValue("@age", minionAge);
            command.Parameters.AddWithValue("@townId", currentTownId);
            command.ExecuteNonQuery();
        }

        private static void AddNewRelationshipInMinionsViliansTable(SqlConnection connection, int minionId, int vilianId)
        {
            string addRelationship = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES " +
                                    "(@minionId, @villainId)";
            SqlCommand command = new SqlCommand(addRelationship, connection);
            command.Parameters.AddWithValue("@minionId", minionId);
            command.Parameters.AddWithValue("@villainId", vilianId);
            command.ExecuteNonQuery();         
        }
    }
}
