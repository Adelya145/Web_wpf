using Microsoft.EntityFrameworkCore;
using Shoes.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace Shoes.Views
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        public OrderWindow()
        {
            InitializeComponent();
            LoadOrders();
            Display();
        }

        private void Back_btn_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            this.Close();
        }

        private void Add_order_btn_Click(object sender, RoutedEventArgs e)
        {
            new AddRedactOrder().Show();
            this.Close();
        }
        private void LoadOrders()
        {
            using (var db = new DbShoesContext())
            {
                var orders = db.Orders.Include(p => p.PickUpPoint).ToList();
                foreach(var i in orders)
                {
                    orders_lv.Items.Add(new OrderControl(i));
                }
            }
        }
        private void Display()
        {
           
            if (App.CurrentClient.UserRole == "Менеджер")
            {
                Add_order_btn.Visibility = Visibility.Collapsed;
                context_menu.Visibility = Visibility.Collapsed;
            }
        }
        private void redact_Click(object sender, RoutedEventArgs e)
        {
            if (orders_lv.SelectedItem is OrderControl selectedOrder)
            {
                if (selectedOrder.DataContext is Order orders)
                {
                    AddRedactOrder addRedactOrder = new AddRedactOrder(orders);
                    addRedactOrder.Show();
                    this.Close();
                }
            }
        }

        private void dell_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (orders_lv.SelectedItem is OrderControl selectedOrder)
                {
                    if (selectedOrder.DataContext is Order order)
                    {
                        MessageBoxResult result = MessageBox.Show(
                            $"Вы действительно хотите удалить заказ №{order.OrderId}?\n\n",
                            
                            "Подтверждение удаления",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            using (var db = new DbShoesContext())
                            {
                                var orderToDelete = db.Orders
                                    .Include(o => o.OrderCompositions)
                                    .FirstOrDefault(o => o.OrderId == order.OrderId);

                                if (orderToDelete != null)
                                {
                                    db.OrderCompositions.RemoveRange(orderToDelete.OrderCompositions);

                                    db.Orders.Remove(orderToDelete);

                                    db.SaveChanges();

                                    MessageBox.Show(
                                        $"Заказ №{order.OrderId} и все его позиции успешно удалены.",
                                        "Удаление выполнено",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);

                                    orders_lv.Items.Clear();
                                    LoadOrders();
                                }
                                else
                                {
                                    MessageBox.Show(
                                        "Заказ не найден в базе данных.",
                                        "Ошибка",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "Не удалось получить данные заказа.",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Выберите заказ для удаления!",
                        "Предупреждение",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Ошибка при удалении заказа: {ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
