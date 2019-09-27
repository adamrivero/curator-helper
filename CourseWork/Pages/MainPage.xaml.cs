using System;
using System.Collections.Generic;
using System.Data.SqlClient; // добавляем пакет для работы с базой данных
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
using System.Threading;

namespace CourseWork.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        
        SqlConnection sqlConnection; // ссылка на объект класса с подключением к БД
        private bool isSorted = false;
        public MainPage()
        {
            InitializeComponent();
        }
        private string RowIndex(int index) // функция для вывода ячейки выбранной строки в таблице
        {
            var ci = new DataGridCellInfo(StudentGrid.SelectedItem, StudentGrid.Columns[index]);
            var content = ci.Column.GetCellContent(ci.Item) as TextBlock;
            return content.Text; // возвращаем значение заданной ячейки таблицы
        }
        public void DisplayData(string query, string param)
        {
            SqlDataReader sqlReader = null; // инициализируем reader

            List<StudentTable> info = new List<StudentTable>(); // создаем список

            try // ловим исключения
            {
                SqlCommand command = new SqlCommand(query, sqlConnection); // создаем SQL запрос к БД
                command.Parameters.AddWithValue("@param", param);  // вводим параметр фильтрации

                sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                while (sqlReader.Read()) // выполняем цикл пока sqlReader содержит значения с БД
                {
                    info.Add(new StudentTable(sqlReader["ID"].ToString(), sqlReader["name"].ToString(), sqlReader["course"].ToString(), sqlReader["faculty"].ToString(), sqlReader["spec"].ToString(), sqlReader["live"].ToString(), sqlReader["number"].ToString(), sqlReader["group_"].ToString())); // .toString - конвертируем переменную в строку | Добавляем данные с БД в список
                }

                StudentGrid.ItemsSource = info; // добавляем данные в таблицу
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
        public void DisplayData(string query) // используем эту функцию когда нужно вывести все данные без параметра
        {
            SqlDataReader sqlReader = null; // инициализируем reader

            List<StudentTable> info = new List<StudentTable>(); // создаем список

            try // ловим исключения
            {
                SqlCommand command = new SqlCommand(query, sqlConnection); // создаем SQL запрос к БД

                sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                while (sqlReader.Read())
                {
                    info.Add(new StudentTable(sqlReader["ID"].ToString(), sqlReader["name"].ToString(), sqlReader["course"].ToString(), sqlReader["faculty"].ToString(), sqlReader["spec"].ToString(), sqlReader["live"].ToString(), sqlReader["number"].ToString(), sqlReader["group_"].ToString())); // .toString - конвертируем переменную в строку | Добавляем данные с БД в список
                }

                StudentGrid.ItemsSource = info; // добавляем данные в таблицу
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

        private void Page_Loaded(object sender, RoutedEventArgs e) // функция срабатывает когда страница загружается 
        {
            DBConnection.SetDataDir(); // вызываем функцию со строкой подключения 

            sqlConnection = new SqlConnection(DBConnection.GetConnection()); // передает в класс SqlConnetion строку подключения
            sqlConnection.Open(); // открывает соединение с БД

            SqlDataReader sqlReader = null; // инициализируем reader
            try // ловим исключения 
            {
                SqlCommand command = new SqlCommand("SELECT DISTINCT group_ FROM Student", sqlConnection); // создаем SQL запрос к БД

                sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                while (sqlReader.Read()) // цикл работает пока sqlReader содержит значения с БД
                {
                    cmbGroupMain.Items.Add(sqlReader["group_"].ToString()); // добавляем в comboBox значения с БД
                }
            }
            catch (Exception ex) // исполняется если ошибка
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally // исполняется в любом случае
            {
                if (sqlReader != null)
                    sqlReader.Close(); // закрываем SqlReader
            }

            DisplayData("SELECT * FROM Student"); // запускаем функцию заполнения таблицы 
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            sqlConnection.Close(); // закрываем соединение к БД
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Это действие ПОЛНОСТЬЮ удалит данные о студенте из базы данных.\nВы уверены?", "Внимание!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE ID = @ID", sqlConnection); // SQL запрос

                    cmd.Parameters.AddWithValue("ID", RowIndex(0)); // параметр SQL запроса

                    cmd.ExecuteNonQuery(); // исполняем SQL запрос

                    showNotice("Студент " + RowIndex(1) + " успешно удален из базы данных", "Ok"); // выводим сообщение пользователю
                }
                catch
                {
                    showNotice("Ошибка. Вы не выбрали ни одной записи", "Error");
                }
                finally
                {
                    if (isSorted) // если до удаления была сортирока, выводим отсортированные данные, иначе выводим всех
                    {
                        DisplayData("SELECT * FROM Student", cmbGroupMain.Text);
                    }
                    else
                    {
                        DisplayData("SELECT * FROM Student");
                    }
                }
            }
        }
        private void CmbGroupMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbGroupMain.SelectedItem != null)
            {
                DisplayData("SELECT * FROM Student WHERE group_=@param", cmbGroupMain.SelectedItem.ToString()); // выводим данные с параметром из ComboBox
                isSorted = true;
            }
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e) // функция срабатывает при нажатии на кнопку найти
        {
            DisplayData("SELECT * FROM Student WHERE name=@param", tbNameMain.Text); // выводим данные с сортировкой по имени
            isSorted = true;
        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e) // срабатывает при нажатии кнопки Отобразить всех
        {
            DisplayData("SELECT * FROM Student"); // выводит всех студентов в таблицу
            isSorted = false;

            tbNameMain.Text = "";
            cmbGroupMain.SelectedIndex = -1;
        }

        private void StudentGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var myObjectRow = e.Row.Item as StudentTable;

            try
            {
                int course = Convert.ToInt32(myObjectRow.course);

                if (course < 1 || course > 6)
                {
                    showNotice("Внимание! В университете всего 6 курсов", "Warn");
                    DisplayData("SELECT * FROM Student"); // заново заполняем таблицу 
                }
                else
                {
                    try
                    {
                        SqlCommand cmd = new SqlCommand("UPDATE Student SET name=@name, course=@course, faculty=@faculty, spec=@spec, live=@live, number=@number, group_=@group WHERE ID=@ID", sqlConnection); // SQL запрос

                        cmd.Parameters.AddWithValue("@ID", myObjectRow.ID); // параметры
                        cmd.Parameters.AddWithValue("@name", myObjectRow.name);
                        cmd.Parameters.AddWithValue("@course", myObjectRow.course);
                        cmd.Parameters.AddWithValue("@faculty", myObjectRow.faculty);
                        cmd.Parameters.AddWithValue("@spec", myObjectRow.spec);
                        cmd.Parameters.AddWithValue("@live", myObjectRow.live);
                        cmd.Parameters.AddWithValue("@number", myObjectRow.number);
                        cmd.Parameters.AddWithValue("@group", myObjectRow.group_);

                        cmd.ExecuteNonQuery(); // выполняем SQL запрос

                        // MessageBox.Show("Данные о студенте с именем " + myObjectRow.name + " успешно обновлены", "Успех"); // выводим сообщение пользователю

                        showNotice("Данные успешно обновлены", "Ok");

                    }
                    catch (Exception ex) // выводим ошибку в случае ошибки
                    {
                        MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    finally
                    {
                        DisplayData("SELECT * FROM Student"); // заново заполняем таблицу 
                    }
                }
            }
            catch {
                showNotice("Внимание! Допустимый ввод: цифры от 1 до 6", "Warn");
                DisplayData("SELECT * FROM Student");
            }
        }

        public async void showNotice(string noticeText, string backgroundColor)
        {
            NoticeText.Text = noticeText;
            switch(backgroundColor)
            {
                case "Ok":
                    NoticeGrid.Background = Brushes.Green;
                    break;
                case "Warn":
                    NoticeGrid.Background = Brushes.Orange;
                    break;
                case "Error":
                    NoticeGrid.Background = Brushes.Red;
                    break;
            }
            NoticeGrid.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            NoticeGrid.Visibility = Visibility.Hidden;
        }
    }
}
