using System;
using System.Collections.Generic;
using System.Data.SqlClient; // пакет для работы с БД
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
using System.Windows.Shapes;

namespace CourseWork.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        SqlConnection sqlConnection; // ссылку на объект класса sqlConnetion
        public AddWindow()
        {
            InitializeComponent();
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e) // кнопка закрытия окна
        {
            Close(); // закрываем окно
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tbGroup.Text == "" || tbLive.Text == "" || tbName.Text == "" || tbNumber.Text == "" || tbSpec.Text == "" || cmbCourse.Text == "" || cmbFaculty.Text == "") // проверяем заполнены ли все поля 
            {
                MessageBox.Show("Заполнены не все поля", "Ошибка"); // выводим ошибку если не заполнены
            }
            else // если заполнены выполняем код ниже
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO Student (name, course, faculty, spec, live, number, group_) VALUES (@name, @course, @faculty, @spec, @live, @number, @group_)", sqlConnection); // инициализируем SQL запрос

                    cmd.Parameters.AddWithValue("name", tbName.Text); // подставляем значения
                    cmd.Parameters.AddWithValue("course", cmbCourse.Text);
                    cmd.Parameters.AddWithValue("faculty", cmbFaculty.Text);
                    cmd.Parameters.AddWithValue("spec", tbSpec.Text);
                    cmd.Parameters.AddWithValue("live", tbLive.Text);
                    cmd.Parameters.AddWithValue("number", tbNumber.Text);
                    cmd.Parameters.AddWithValue("group_", tbGroup.Text);

                    cmd.ExecuteNonQuery(); // выполняем SQL запрос

                    MessageBox.Show(tbName.Text + " успешно внесен(а) в базу данных", "Успех", MessageBoxButton.OK, MessageBoxImage.Information); // выводим сообщение пользователю 

                    Close(); // зкрываем окно после добавления 
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Add", MessageBoxButton.OK, MessageBoxImage.Error); // выводим ошибку если она есть 
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) // функция срабатывает при открытии окна
        {
            DBConnection.SetDataDir(); // задаем путь к БД

            sqlConnection = new SqlConnection(DBConnection.GetConnection()); // передаем путь к БД в класс SQLConnection
            sqlConnection.Open(); // открываем соединение к БД
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) // срабатывает при закрытии окна
        {
            sqlConnection.Close(); // закрываем подключение к БД
        } 
    }
}
