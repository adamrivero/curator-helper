using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    class DBConnection
    {
        public static void SetDataDir() // функция указывает путь к БД
        {
            string dataDir;

            dataDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
        }

        public static string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\AppData\DataBase.mdf;Integrated Security=True"; // строка подключения к БД
        public static string GetConnection() // функция которая возвращает путь к БД
        { 
            return ConnectionString;
        }
    }
}
