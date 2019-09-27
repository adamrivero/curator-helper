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
using Word = Microsoft.Office.Interop.Word;

namespace CourseWork
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window // главное окно, которое открывается при запуске приложения
    {
        public MainWindow() // работает при запуске приложения 
        {
            InitializeComponent();
            Main.Content = new Pages.MainPage(); // открываем главную страницу 
            btnReport.Visibility = Visibility.Hidden; // скрываем кнопку отчет 
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e)
        {
            Close(); // функция закрытия приложения
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Windows.AddWindow aw = new Windows.AddWindow(); // создаем ссылку на объект AddWindow

            aw.ShowDialog(); // открываем окно

            Main.Content = new Pages.MainPage(); // перезагружаем страницу
        }

        private void BtnProgress_Click(object sender, RoutedEventArgs e)
        {
            btnReport.Visibility = Visibility.Visible; // показываем кнопку "Отчет"
            Main.Content = new Pages.ProgressPage();  // открываем страницу с успеваемостью
        }

        private void BtnStudent_Click(object sender, RoutedEventArgs e)
        {
            btnReport.Visibility = Visibility.Hidden; // скрываем кнопку "Отчет"
            Main.Content = new Pages.MainPage(); // открываем страницу студенты
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            Windows.ReportWindow rw = new Windows.ReportWindow(); // ссылка на обьект ReportWindows

            rw.ShowDialog(); // открываем окно ReportWindow
        }

    }
}
