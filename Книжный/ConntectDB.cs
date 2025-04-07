using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using BCrypt.Net;
using MetroFramework;
using System.Windows.Forms;

namespace Книжный
{
    public class ConnectDB
    {
        static string Connection_string = "Database = Book_shop; Server = localhost; User = root; Password = ";
        public static DataTable select(string query, Dictionary<string, object> parameters = null)
        {
            DataTable table = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(Connection_string))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    try
                    {
                        connection.Open();
                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(table);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return table;
        }

        public static bool ExecuteQuery(string query, Dictionary<string, object> parameters)
        {
            using (MySqlConnection connection = new MySqlConnection(Connection_string))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }

                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
        }

        public static bool update(string tableName, Dictionary<string, object> setValues, string condition, Dictionary<string, object> conditionParams)
        {
            string setClause = string.Join(", ", setValues.Keys.Select(k => $"{k} = @{k}"));

            string query = $"UPDATE {tableName} SET {setClause} WHERE {condition}";

            Dictionary<string, object> parameters = new Dictionary<string, object>(setValues);
            foreach (var param in conditionParams)
            {
                parameters.Add(param.Key, param.Value);
            }

            return ExecuteQuery(query, parameters);
        }

        public static bool insert(string tableName, Dictionary<string, object> values)
        {
            string columns = string.Join(", ", values.Keys);
            string paramNames = string.Join(", ", values.Keys.Select(k => $"@{k}"));
            string query = $"INSERT INTO {tableName} ({columns}) VALUES ({paramNames})";

            return ExecuteQuery(query, values);
        }

        public static bool delete(string tableName, string condition, Dictionary<string, object> conditionParams)
        {
            string query = $"DELETE FROM {tableName} WHERE {condition}";
            return ExecuteQuery(query, conditionParams);
        }

        public static bool AuthenticateUser(string username, string password, out bool isAdmin)
        {
            isAdmin = false;
            try
            {
                string query = "SELECT Админ, Пароль FROM Сотрудники WHERE Логин = @username";

                using (MySqlConnection conn = new MySqlConnection(Connection_string))
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    conn.Open();

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPassword = reader.GetString("Пароль");
                            if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                            {
                                isAdmin = reader.GetBoolean("Админ");
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка SQL");
                return false;
            }
        }
        public static int InsertAndGetId(string table, Dictionary<string, object> data)
        {
            using (var connection = new MySqlConnection(Connection_string))
            {
                connection.Open();
                var columns = string.Join(", ", data.Keys);
                var values = string.Join(", ", data.Keys.Select(k => "@" + k));
                var cmd = new MySqlCommand($"INSERT INTO {table} ({columns}) VALUES ({values}); SELECT LAST_INSERT_ID();", connection);

                foreach (var pair in data)
                    cmd.Parameters.AddWithValue("@" + pair.Key, pair.Value);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }


    }
}