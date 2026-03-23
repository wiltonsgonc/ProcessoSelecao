using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        var connectionString = "Server=127.0.0.1,1433;Database=ProcessoSelecaoDb;User Id=sa;Password=Processo@123;TrustServerCertificate=True;";
        
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("Connected to database!");
            
            // Reset identity seed for all tables
            var tables = new[] { "ProcessosSelecao", "Candidatos", "Avaliadores", "Baremas", "Documentos" };
            
            foreach (var table in tables)
            {
                Console.WriteLine($"\n--- Resetting {table} ---");
                
                // Delete all records
                try
                {
                    using (var command = new SqlCommand($"DELETE FROM {table}", connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Deleted all records from {table}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not delete records from {table}: {ex.Message}");
                }
                
                // Reset identity seed
                try
                {
                    using (var command = new SqlCommand($"DBCC CHECKIDENT ('{table}', RESEED, 0)", connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine($"Reset identity seed for {table}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not reset identity seed for {table}: {ex.Message}");
                }
                
                // Verify new seed
                try
                {
                    using (var command = new SqlCommand($"DBCC CHECKIDENT ('{table}', NORESEED)", connection))
                    {
                        var result = command.ExecuteScalar();
                        Console.WriteLine($"New identity seed for {table}: {result}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Could not verify identity seed for {table}: {ex.Message}");
                }
            }
            
            Console.WriteLine("\nIdentity reset completed successfully!");
            Console.WriteLine("All tables are now empty and ready to start from ID 1.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
