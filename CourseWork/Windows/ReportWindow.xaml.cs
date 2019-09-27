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
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using WinForms = System.Windows.Forms; // ссылка на пакет для работы с Word документами 
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using iTextSharp.text.html;
using Path = System.IO.Path;

namespace CourseWork.Windows
{
    /// <summary>
    /// Логика взаимодействия для ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        SqlConnection sqlConnection;

        public ReportWindow()
        {
            InitializeComponent();
            this.tbPercent.PreviewTextInput += new TextCompositionEventHandler(TbPercent_PreviewTextInput); // ЗАПРЕЩАЕМ ВВОДИТЬ ВСЕ КРОМЕ ЦИФР
        }

        private void ButtonFechar_Click(object sender, RoutedEventArgs e) // кнопка закрытия окна
        {
            Close(); // закрываем окно
        }

        private void DoReport(string query, string firstParam, string secondParam)
        {
            SqlDataReader sqlReader = null; // инициализируем reader

            using (WinForms.SaveFileDialog saveFileDialog = new WinForms.SaveFileDialog()
            {
                Filter = "PDF file|*.pdf", ValidateNames = true // выбираем фильтр для сохранения
            })
            if(saveFileDialog.ShowDialog() == WinForms.DialogResult.OK)
            {
                    iTextSharp.text.Document doc = new iTextSharp.text.Document(PageSize.A4);
                    try
                    {
                        if (tbPercent.Text == "" || cmbGroup.Text == "") // проверяем выбраны ли все параметры
                        {
                            MessageBox.Show("Необходимо выбрать все параметры", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else if (Convert.ToDouble(tbPercent.Text) < 0 || Convert.ToDouble(tbPercent.Text) > 100) // проверяем правильный ли диапазон процента
                        {
                            MessageBox.Show("Введите коректный параметр процента посещения (от 0 до 100)", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            PdfWriter.GetInstance(doc, new FileStream(saveFileDialog.FileName, FileMode.Create));

                            doc.Open();
                            BaseFont bf = BaseFont.CreateFont(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Verdana.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            BaseFont italf = BaseFont.CreateFont(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Verdanai.ttf"), BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            iTextSharp.text.Font itf = new iTextSharp.text.Font(bf, 10f, iTextSharp.text.Font.ITALIC);
                            iTextSharp.text.Font htf = new iTextSharp.text.Font(bf, 20f, iTextSharp.text.Font.NORMAL);
                            iTextSharp.text.Font ttf = new iTextSharp.text.Font(bf, 10f, iTextSharp.text.Font.NORMAL);
                            iTextSharp.text.Font stf = new iTextSharp.text.Font(bf, 8f, iTextSharp.text.Font.NORMAL);

                            doc.Add(new iTextSharp.text.Paragraph("ХНЭУ", htf));
                            doc.Add(new iTextSharp.text.Paragraph("им. Семена Кузнеца", ttf));
                            PdfPTable table = new PdfPTable(5);
                            PdfPCell cell = new PdfPCell(new Phrase("Список студентов", ttf));
                            cell.Colspan = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.AddCell(cell);
                            table.AddCell(new Phrase("ФИО", ttf));
                            table.AddCell(new Phrase("Тел.", ttf));
                            table.AddCell(new Phrase("Всего часов", ttf));
                            table.AddCell(new Phrase("Пропущено", ttf));
                            table.AddCell(new Phrase("По ув. причине", ttf));
                            doc.Add(new iTextSharp.text.Paragraph("\n", ttf));
                            if (rbMore.IsChecked == true)
                            {
                                doc.Add(new iTextSharp.text.Paragraph("Список студентов, у которых % пропусков больше " + tbPercent.Text, ttf));
                            }
                            else if (rbLess.IsChecked == true)
                            {
                                doc.Add(new iTextSharp.text.Paragraph("Список студентов, у которых % пропусков меньше " + tbPercent.Text, ttf));
                            }
                            doc.Add(new iTextSharp.text.Paragraph("Группа: " + cmbGroup.Text, ttf));
                            doc.Add(new iTextSharp.text.Paragraph("\n", ttf));
                            SqlCommand command = new SqlCommand(query, sqlConnection); // создаем SQL запрос к БД
                            command.Parameters.AddWithValue("@group", firstParam); // подставляем параметры в запрос
                            command.Parameters.AddWithValue("@percent", secondParam);
                            sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                            while (sqlReader.Read()) // пока sqlReader содержит строки добавляем их в отчет
                            {
                                table.AddCell(new Phrase(sqlReader["name"].ToString(), stf));
                                table.AddCell(new Phrase(sqlReader["number"].ToString(), stf));
                                table.AddCell(new Phrase(sqlReader["allhours"].ToString(), stf));
                                table.AddCell(new Phrase(sqlReader["losthours"].ToString(), stf));
                                table.AddCell(new Phrase(sqlReader["reason"].ToString(), stf));
                            }
                            doc.Add(table);
                            doc.Add(new iTextSharp.text.Paragraph("\n", ttf));
                            PdfPTable dateTable = new PdfPTable(1);
                            cell = new PdfPCell(new Phrase("Дата: " + DateTime.Now, ttf));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.Border = 0;
                            dateTable.AddCell(cell);
                            doc.Add(dateTable);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, ex.Source, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    finally
                    {
                        doc.Close();
                        sqlReader.Close();
                    }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
                if (rbMore.IsChecked == true)
                {
                    DoReport("SELECT * FROM Student WHERE group_ = @group AND percent_ > @percent", cmbGroup.SelectedItem.ToString(), tbPercent.Text);
                }
                else if (rbLess.IsChecked == true)
                {
                    DoReport("SELECT * FROM Student WHERE group_ = @group AND percent_ < @percent", cmbGroup.SelectedItem.ToString(), tbPercent.Text);
                }
            
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DBConnection.SetDataDir(); // указываем путь к БД

            sqlConnection = new SqlConnection(DBConnection.GetConnection()); // передаем путь к БД в sqlConnection
            sqlConnection.Open(); // открываем соединение к БД

            SqlDataReader sqlReader = null; // инициализируем reader
            try
            {
                SqlCommand command = new SqlCommand("SELECT DISTINCT group_ FROM Student", sqlConnection); // создаем SQL запрос к БД

                sqlReader = command.ExecuteReader(); // исполняем SQL запрос

                while (sqlReader.Read())
                {
                    cmbGroup.Items.Add(sqlReader["group_"].ToString()); // заполняем комбобокс 
                }

                cmbGroup.SelectedIndex = 1;
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sqlConnection.Close(); // при закрытии окна закрываем соединение к БД
        }

        private void TbPercent_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Char.IsDigit(e.Text, 0)) e.Handled = true; // проверяем вводит ли пользователь цифры, если нет то запрещаем это делать
        }
    }
}
