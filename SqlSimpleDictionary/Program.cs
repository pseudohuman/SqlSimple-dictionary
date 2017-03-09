using Microsoft.Data.Sqlite;
using System;

namespace SqlSimpleDictionary
{
    static class Program
    {
        static void Main(string[] args)
        {
            using (var sqlConnection = new SqliteConnection("Data Source=Test01.sqlite;"))
            {
                sqlConnection.Open();
                sqlConnection.CreateUserTable();
                ReadString(sqlConnection);
            }

        }
        private static void ReadString(SqliteConnection sqlConnection)
        {

            while (true)
            {
                Console.Write(">");
                string line = Console.ReadLine();
                char startChar = line[0];
                switch (startChar)
                {
                    case '+':
                        if (line.Length > 1)
                        {
                            var res = NameValueResult.Parse(line.Substring(1));
                            if (res.IsValid) sqlConnection.AddArticle(res.Name, res.Value);
                            else Console.WriteLine("You may made som error please enter string in form +name=value");

                        }
                        else
                        {
                            Console.WriteLine("Please enter article in form name=article");
                            var res = NameValueResult.Parse(Console.ReadLine());
                            if (res.IsValid) sqlConnection.AddArticle(res.Name, res.Value);
                            else Console.WriteLine("You may made some error please repeate the procedure");
                        }
                        break;

                    case '-':
                        if (line.Length > 1)
                        {
                            string str = line.Substring(1);
                            sqlConnection.Remove(str);
                        }
                        else
                        {
                            Console.WriteLine("Please enter article name");
                            
                        }
                        break;
                    case '#':
                        sqlConnection.ShowTable("Dict");
                        break;

                    default:
                        sqlConnection.ShowDictionary(line);
                        break;
                }
            }
        }
        private static bool YesNoQuestion()
        {
            while (true)
            {
                var ans = Console.ReadLine();
                switch (ans.ToLower())
                {
                    case "y":
                    case "yes":
                        return true;
                    case "n":
                    case "no":
                        return false;
                    default:
                        Console.WriteLine("Please answer yes or no.");
                        break;
                }
            }
        }
        static void ShowDictionary(this SqliteConnection sqlConnection, string str)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = "select value from Dict where name=@name";
                command.Parameters.AddWithValue("name",str);
                using (var reader = command.ExecuteReader())
                {

                    if (reader.Read()) Console.WriteLine(reader[0].ToString());
                    else Console.WriteLine("Dictionary doesn`t  contain article on this name");

                }
            }
        }
        static void ShowTable(this SqliteConnection sqlConnection, string tableName)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = $"select * from Dict";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"name: {reader.GetString(0)}");    
                        Console.WriteLine($"value: {reader.GetString(1)}");
                        Console.WriteLine();

                    }
                }
            }
        }
       
        static void Remove(this SqliteConnection sqlConnection, string str)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = "delete from Dict where name=@name";
                command.Parameters.AddWithValue("name",str);
                command.ExecuteNonQuery();
            }
        }
        
        
        static void AddArticle(this SqliteConnection sqlConnection, string name, string value)
        {
            
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = "select null from Dict where name=@name";
                command.Parameters.AddWithValue("name", name);
                var hasName = false;
                using (var reader = command.ExecuteReader()) hasName = reader.HasRows;
                command.Parameters.Clear();
                if (hasName)
                {
                    Console.WriteLine("Article with this name  already exists. Do you wish to replace?(yes/no) ");
                    if (YesNoQuestion())
                    {
                        command.CommandText = "update Dict set value=@value where name=@name";
                        command.Parameters.AddWithValue("value", value);
                        command.Parameters.AddWithValue("name", name);
                        command.ExecuteNonQuery();
                    }
                    return;
                }
                command.CommandText = "insert into Dict (name,value) values(@name,@value)";
                command.Parameters.AddWithValue("name", name);
                command.Parameters.AddWithValue("value", value);
                command.ExecuteNonQuery();

            }

        }
        static void CreateUserTable(this SqliteConnection sqlConnection)
        {
            using (var command = sqlConnection.CreateCommand())
            {
                command.CommandText = "create table if not exists Dict(name text not null primary key, value text)";
                command.ExecuteNonQuery();
            }
        }
    }
}