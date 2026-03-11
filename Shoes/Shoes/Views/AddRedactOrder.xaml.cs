using Microsoft.EntityFrameworkCore;
using Shoes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Shoes.Views
{
    /// <summary>
    /// Логика взаимодействия для AddRedactOrder.xaml
    /// </summary>
    public partial class AddRedactOrder : Window
    {
        private Order order;
        private bool isEditMode = false;
        private DbShoesContext db;
        
        public class OrderItemDisplay
        {
            public string TovarArticle { get; set; }
            public string TovarName { get; set; }
            public int Count { get; set; }
            public Tovar OriginalTovar { get; set; }
        }

        public AddRedactOrder(Order or = null)
        {
            InitializeComponent();
            db = new DbShoesContext();

            LoadComboBoxes();
            LoadProducts();

            if (or != null)
            {
                order = or;
                isEditMode = true;

                article.Text = or.OrderId.ToString();
                date.Text = or.OrderDate?.ToString("dd.MM.yyyy") ?? "";
                date_delivery.Text = or.OrderDateDelivery?.ToString("dd.MM.yyyy") ?? "";
                code.Text = or.OrderCode;

                cbUser.SelectedValue = or.UserId;
                cbPickup.SelectedValue = or.PickUpPointId;

                if (!string.IsNullOrEmpty(or.OrderStatus))
                    status.SelectedItem = or.OrderStatus;

                Title = "Редактирование заказа";
                SaveOrder.Content = "Сохранить изменения";
                
                LoadOrderComposition();
            }
            else
            {
                order = new Order();

                Random rand = new Random();
                order.OrderCode = rand.Next(100, 999).ToString();
                code.Text = order.OrderCode;

                order.OrderStatus = "Новый";
                status.SelectedItem = order.OrderStatus;

                article.Text = "Будет присвоен автоматически";

                Title = "Добавить заказ";
                SaveOrder.Content = "Добавить заказ";
                
                lvOrderItems.ItemsSource = new List<OrderItemDisplay>();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                cbUser.ItemsSource = db.Users.ToList();
                cbPickup.ItemsSource = db.PickUpPoints.ToList();
                status.ItemsSource = new List<string> { "Новый", "Завершен" };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = db.Tovars.ToList();
                foreach (var product in products)
                {
                    product.DisplayName = $"{product.TovarArticle} - {product.TovarName}";
                }
                cbProducts.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке товаров: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderComposition()
        {
            try
            {
                var orderItems = db.OrderCompositions
                    .Include(oc => oc.TovarArticleNavigation)
                    .Where(oc => oc.OrderId == order.OrderId)
                    .ToList();

                var displayItems = orderItems.Select(oc => new OrderItemDisplay
                {
                    TovarArticle = oc.TovarArticle,
                    TovarName = oc.TovarArticleNavigation?.TovarName ?? "Неизвестно",
                    Count = oc.OrderCompositionCount ?? 1,
                    OriginalTovar = oc.TovarArticleNavigation
                }).ToList();

                lvOrderItems.ItemsSource = displayItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке состава заказа: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void cbProducts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbProducts.SelectedItem != null)
            {
                txtProductCount.Text = "1";
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbProducts.SelectedItem == null)
                {
                    MessageBox.Show("Выберите товар!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtProductCount.Text, out int count) || count <= 0)
                {
                    MessageBox.Show("Введите корректное количество (целое положительное число)!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var selectedProduct = cbProducts.SelectedItem as Tovar;
                var currentItems = lvOrderItems.ItemsSource as List<OrderItemDisplay> ?? new List<OrderItemDisplay>();
                var itemsList = currentItems.ToList();

                var existingItem = itemsList.FirstOrDefault(i => i.TovarArticle == selectedProduct.TovarArticle);
                
                if (existingItem != null)
                {
                    existingItem.Count += count;
                }
                else
                {
                    itemsList.Add(new OrderItemDisplay
                    {
                        TovarArticle = selectedProduct.TovarArticle,
                        TovarName = selectedProduct.TovarName,
                        Count = count,
                        OriginalTovar = selectedProduct
                    });
                }

                lvOrderItems.ItemsSource = itemsList;
                lvOrderItems.Items.Refresh();
                
                cbProducts.SelectedItem = null;
                txtProductCount.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var itemToRemove = button?.Tag as OrderItemDisplay;

                if (itemToRemove != null)
                {
                    var currentItems = lvOrderItems.ItemsSource as List<OrderItemDisplay>;
                    if (currentItems != null)
                    {
                        currentItems.Remove(itemToRemove);
                        lvOrderItems.ItemsSource = currentItems;
                        lvOrderItems.Items.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            this.Close();
        }

        private void SaveOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbUser.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пользователя!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbPickup.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пункт выдачи!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (status.SelectedItem == null)
                {
                    MessageBox.Show("Выберите статус заказа!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(date.Text))
                {
                    MessageBox.Show("Введите дату заказа!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(date_delivery.Text))
                {
                    MessageBox.Show("Введите дату доставки!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var orderItems = lvOrderItems.ItemsSource as List<OrderItemDisplay>;
                if (orderItems == null || !orderItems.Any())
                {
                    MessageBox.Show("Добавьте хотя бы один товар в заказ!", "Предупреждение",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cbUser.SelectedItem is User selectedUser)
                    order.UserId = selectedUser.UserId;

                if (cbPickup.SelectedItem is PickUpPoint selectedPickup)
                    order.PickUpPointId = selectedPickup.PickUpPointId;

                order.OrderStatus = status.SelectedItem.ToString();

                if (DateTime.TryParse(date.Text, out DateTime orderDateTime))
                    order.OrderDate = DateOnly.FromDateTime(orderDateTime);
                else
                {
                    MessageBox.Show("Неверный формат даты заказа! Используйте ДД.ММ.ГГГГ", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (DateTime.TryParse(date_delivery.Text, out DateTime deliveryDateTime))
                    order.OrderDateDelivery = DateOnly.FromDateTime(deliveryDateTime);
                else
                {
                    MessageBox.Show("Неверный формат даты доставки! Используйте ДД.ММ.ГГГГ", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                using (var saveDb = new DbShoesContext())
                {
                    if (isEditMode)
                    {
                        var existingOrder = saveDb.Orders
                            .Include(o => o.OrderCompositions)
                            .FirstOrDefault(o => o.OrderId == order.OrderId);
                        
                        if (existingOrder != null)
                        {
                            existingOrder.UserId = order.UserId;
                            existingOrder.PickUpPointId = order.PickUpPointId;
                            existingOrder.OrderDate = order.OrderDate;
                            existingOrder.OrderDateDelivery = order.OrderDateDelivery;
                            existingOrder.OrderStatus = order.OrderStatus;

                            saveDb.OrderCompositions.RemoveRange(existingOrder.OrderCompositions);

                            foreach (var item in orderItems)
                            {
                                saveDb.OrderCompositions.Add(new OrderComposition
                                {
                                    OrderId = existingOrder.OrderId,
                                    TovarArticle = item.TovarArticle,
                                    OrderCompositionCount = item.Count
                                });
                            }

                            saveDb.SaveChanges();
                            MessageBox.Show("Заказ успешно обновлен!", "Информация",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        saveDb.Orders.Add(order);
                        saveDb.SaveChanges();

                        foreach (var item in orderItems)
                        {
                            saveDb.OrderCompositions.Add(new OrderComposition
                            {
                                OrderId = order.OrderId,
                                TovarArticle = item.TovarArticle,
                                OrderCompositionCount = item.Count
                            });
                        }

                        saveDb.SaveChanges();
                        MessageBox.Show("Заказ успешно добавлен!", "Информация",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                new OrderWindow().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void txtProductCount_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "[0-9]"))
            {
                e.Handled = true;
            }
        }
    }
}