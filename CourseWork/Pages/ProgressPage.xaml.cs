using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWork.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProgressPage.xaml
    /// </summary>
    public partial class ProgressPage : Page
    {
        SqlConnection sqlConnection; // создаем ссылку на объект класса SqlConnection
        public ProgressPage()
        {
            InitializeComponent();
        }
        private string RowIndex(int index) // функция для вывода ячейки выбранной строки в табоице
        {
            var ci = new DataGridCellInfo(ProgressGrid.SelectedItem, ProgressGrid.Columns[index]);
            var content = ci.Column.GetCellContent(ci.Item) as TextBlock;
            return content.Text; // возвращаем значение заданной ячейки таблицы
        }
        public void DisplayData(string query)
        {
            SqlDataReader sqlReader = null; // инициализируем reader

            List<StudentTable> info = new List<StudentTable>(); // создаем список

            try // ловим исключения
            {
                SqlCommand command = new SqlCommand(query, sqlConnection); // создаем SQL запрос к БД

                sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                while (sqlReader.Read())
                {
                    info.Add(new StudentTable(sqlReader["ID"].ToString(), sqlReader["name"].ToString(), sqlReader["allhours"].ToString(), sqlReader["losthours"].ToString(), sqlReader["reason"].ToString(), sqlReader["percent_"].ToString(), sqlReader["group_"].ToString())); // .toString - конвертируем переменную в строку | Добавляем данные с БД в список
                }

                ProgressGrid.ItemsSource = info; // добавляем данные в таблицу
            }
            catch (Exception ex) // исполняется если ошибка
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally // исполняется в любом случае
            {
                if (sqlReader != null)
                    sqlReader.Close();
            }
        }
        private void ProgressGrid_Loaded(object sender, RoutedEventArgs e)
        {
            DBConnection.SetDataDir(); // инициализируем подключение к базе данных

            sqlConnection = new SqlConnection(DBConnection.GetConnection()); // передаем строку подключения к БД
            sqlConnection.Open(); // открываем подключение к БД
             
            DisplayData("SELECT * FROM Student"); // загружаем данные в таблицу 
        }

        private void ProgressGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            sqlConnection.Close(); // закрываем подключение к БД
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            double percent; // создаем переменную с плавающей точкой

            percent = (Convert.ToDouble(RowIndex(3)) * 100) / Convert.ToDouble(RowIndex(2)); // формула подсчета процента

            percent = Math.Round(percent, 2); // округляем до двух значений после запятой

            int allhours = Convert.ToInt32(RowIndex(2));
            int losthours = Convert.ToInt32(RowIndex(3));
            int reason = Convert.ToInt32(RowIndex(4));

            if (allhours < losthours || allhours < reason || losthours < reason || allhours < 0 || losthours < 0 || reason < 0)
            {
                MessageBox.Show("Проверьте правильность введенных данных.\n\nИнструкция:\n1. Пропусков не может быть больше чем часов по расписанию\n2. Пропусков по уважительной причине не может быть больше чем часов по расписанию\n3. Пропусков по уважительной причине не может быть больше чем самих пропусков\n4. Посещение не может быть отрицательным числом", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Information);
                DisplayData("SELECT * FROM Student"); // заполняем таблицу 
            }
            else
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Student SET name=@name, allhours=@allhours, losthours=@losthours, reason=@reason, percent_=@percent, group_=@group WHERE ID=@ID", sqlConnection); // создаем SQL запрос

                    cmd.Parameters.AddWithValue("@ID", RowIndex(0)); // параметры запроса
                    cmd.Parameters.AddWithValue("@name", RowIndex(1));
                    cmd.Parameters.AddWithValue("@allhours", RowIndex(2));
                    cmd.Parameters.AddWithValue("@losthours", RowIndex(3));
                    cmd.Parameters.AddWithValue("@reason", RowIndex(4));
                    cmd.Parameters.AddWithValue("@percent", percent.ToString().Replace(",", "."));
                    cmd.Parameters.AddWithValue("@group", RowIndex(6));

                    cmd.ExecuteNonQuery(); // выполняем SQL запрос

                    MessageBox.Show("Данные о студенте с именем " + RowIndex(1) + " успешно обновлены", "Успех"); // выводим пользователю сообщение
                }
                catch (Exception ex) // выводим ошибку в случае ошибки
                {
                    MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    DisplayData("SELECT * FROM Student"); // заполняем таблицу 
                }
            }
        }
    }
}
