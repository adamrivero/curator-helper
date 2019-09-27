using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CourseWork
{
    class Notice
    {
        public static async void showNotice(string noticeText, string backgroundColor)
        {
            MainWindow mw = new MainWindow();

            mw.NoticeText.Text = noticeText;
            switch (backgroundColor)
            {
                case "Ok":
                    mw.NoticeGrid.Background = Brushes.Green;
                    break;
                case "Warn":
                    mw.NoticeGrid.Background = Brushes.Orange;
                    break;
                case "Error":
                    mw.NoticeGrid.Background = Brushes.Red;
                    break;
            }
            mw.NoticeGrid.Visibility = Visibility.Visible;
            await Task.Delay(2000);
            mw.NoticeGrid.Visibility = Visibility.Hidden; 
        }
    }
}
