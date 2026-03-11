using Shoes.Model;
using System;
using System.Collections.Generic;
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
using System.Xml.Linq;

namespace Shoes.Views
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void log_btn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(login_tb.Text) || string.IsNullOrEmpty(pass_pb.Password))
            {
                MessageBox.Show("Заполните все пустые поля!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (login_tb.Text.Length > 60)
            {
                MessageBox.Show("Длина логина не может превышать больше 60 символов!",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new DbShoesContext())
            {
                var userExists = db.Users.Any(u => u.UserLogin == login_tb.Text);

                if (!userExists)
                {
                    MessageBox.Show("Пользователя с таким логином не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var client = db.Users.FirstOrDefault(a => a.UserLogin == login_tb.Text && a.UserPassword == pass_pb.Password);

                if (client == null)
                {
                    MessageBox.Show("Неверный пароль!");
                    return;
                }
                MessageBox.Show("Вы успешно вошли в систему", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                App.CurrentClient = client;
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void enter_guest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            main.Show();
            this.Close();
        }
    }
}
