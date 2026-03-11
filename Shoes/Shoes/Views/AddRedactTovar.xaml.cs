using Shoes.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using System.Drawing;

namespace Shoes.Views
{
    public partial class AddRedactTovar : Window
    {
        private Tovar product;
        private bool isEditMode = false;
        private string currentImagePath = "";

        private string resourcesPath = @"C:\Users\79174\Downloads\Telegram Desktop\Материал к заданию\Материалы к заданию\Shoes\Shoes\Resources\";
        private static Random random = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public AddRedactTovar(Tovar p = null)
        {
            InitializeComponent();

            CreateResourcesDirectory();

            cbCategory.ItemsSource = GetCategories();
            cbManufacturers.ItemsSource = GetManufacturers();
            cbdelivery.ItemsSource = GetSuppliers();

            if (p != null)
            {
                product = p;
                isEditMode = true;
                article.Text = p.TovarArticle;
                article.IsEnabled = false;
                unit.IsEnabled = false;
                name.Text = p.TovarName;
                unit.Text = p.TovarUnit;
                cost.Text = p.TovarCost.ToString();
                current_sale.Text = p.TovarCurrentSale.ToString();
                quantity.Text = p.TovarCount.ToString();
                description.Text = p.TovarDesc;
                photo.Text = p.TovarPhoto;
                cbManufacturers.SelectedIndex = (int)p.ManufactrurerId - 1;
                cbdelivery.SelectedIndex = (int)p.SupplierId - 1;
                cbCategory.SelectedIndex = (int)p.TovarCategoryId - 1;

                Title = "Редактирование товара";
                SaveGood.Content = "Сохранить изменения";

                LoadProductImage();
            }
            else
            {
                GenerateArticle();
                article.IsEnabled = false;
                unit.IsEnabled = false;
                Title = "Добавление нового товара";
                SaveGood.Content = "Добавить товар";
            }
        }

        private void GenerateArticle()
        {
            using (var db = new DbShoesContext())
            {
                string newArticle;
                do
                {
                    newArticle = new string(Enumerable.Repeat(chars, 6).Select(a => a[random.Next(a.Length)]).ToArray());
                }
                while (db.Tovars.Any(t => t.TovarArticle == newArticle));
                article.Text = newArticle;
            }
        }

        private void CreateResourcesDirectory()
        {
            try
            {
                if (!Directory.Exists(resourcesPath))
                {
                    Directory.CreateDirectory(resourcesPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании папки Resources: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Path.Combine(resourcesPath, "picture.png");

            string imagePath = Path.Combine(resourcesPath, fileName);
            return File.Exists(imagePath) ? imagePath : Path.Combine(resourcesPath, "picture.png");
        }

        private void LoadProductImage()
        {
            try
            {
                string imagePath = GetImagePath(product?.TovarPhoto);

                if (File.Exists(imagePath))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgPreview.Source = bitmap;
                    imgPreview.Visibility = Visibility.Visible;
                }
                else
                {
                    imgPreview.Visibility = Visibility.Collapsed;
                }
            }
            catch
            {
                imgPreview.Visibility = Visibility.Collapsed;
            }
        }

        private bool CheckImageSize(string imagePath)
        {
            try
            {
                using (var fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                using (var image = System.Drawing.Image.FromStream(fileStream))
                {
                    if (image.Width == 300 && image.Height == 200)
                    {
                        return true;
                    }

                    MessageBox.Show($"Изображение должно быть размером 300x200 пикселей.\nТекущий размер: {image.Width}x{image.Height}",
                        "Ошибка размера", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке размеров изображения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void BrowseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Все файлы (*.*)|*.*",
                Title = "Выберите изображение товара (требуемый размер: 300x200)"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string selectedFilePath = openFileDialog.FileName;

                // Проверяем размер только для выбранного пользователем файла
                if (!CheckImageSize(selectedFilePath))
                {
                    return;
                }

                currentImagePath = selectedFilePath;

                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(currentImagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgPreview.Source = bitmap;
                    imgPreview.Visibility = Visibility.Visible;
                    photo.Text = Path.GetFileName(currentImagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить изображение: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    imgPreview.Visibility = Visibility.Collapsed;
                }
            }
        }

        private string GenerateUniqueFileName(string originalFileName)
        {
            string extension = Path.GetExtension(originalFileName);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
            string guid = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"img_{timestamp}_{guid}{extension}";
        }

        private string SaveImageToResources(string sourcePath, string fileName)
        {
            try
            {
                string destinationPath = Path.Combine(resourcesPath, fileName);
                File.Copy(sourcePath, destinationPath, true);
                return fileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изображения: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return "";
            }
        }

        private void DeleteImageFile(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName) ||
                imageFileName.Equals("picture.png", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                string filePath = Path.Combine(resourcesPath, imageFileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить файл изображения: {ex.Message}",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateImagePreview(string imageFileName)
        {
            photo.Text = imageFileName;
            string imagePath = GetImagePath(imageFileName);

            if (File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();

                    imgPreview.Source = bitmap;
                    imgPreview.Visibility = Visibility.Visible;
                }
                catch
                {
                    imgPreview.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                imgPreview.Visibility = Visibility.Collapsed;
            }
        }

        private void SaveGood_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(article.Text) ||
                string.IsNullOrWhiteSpace(name.Text) ||
                string.IsNullOrWhiteSpace(cost.Text))
            {
                MessageBox.Show("Заполните все обязательные поля", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(int.Parse(cost.Text) <= 0)
            {
                MessageBox.Show("Цена долна быть больше 0", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.TryParse(current_sale.Text, out int saleValue) && (saleValue <= 0 || saleValue >= 100))
            {
                MessageBox.Show("Скидка не может быть меньше или равна 0 и больше или равна 100", "Предупреждение",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(quantity.Text, out int quantityValue) || quantityValue <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом больше 0",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (name.Text.Length > 35)
            {
                MessageBox.Show("Длина названия не может превышать больше 35 символов!",
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var db = new DbShoesContext())
                {
                    string imageFileName = "";

                    string costText = cost.Text.Replace(',', '.');
                    if (!decimal.TryParse(costText, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out decimal costValue))
                    {
                        MessageBox.Show("Некорректный формат стоимости. Используйте число (например: 1500.50 или 1500,50)",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    int.TryParse(current_sale.Text, out saleValue);

                    if (isEditMode && product != null)
                    {
                        var currentProduct = db.Tovars.FirstOrDefault(g => g.TovarArticle == product.TovarArticle);

                        if (currentProduct != null)
                        {
                            string oldPhotoName = currentProduct.TovarPhoto;

                            if (!string.IsNullOrEmpty(currentImagePath) && File.Exists(currentImagePath))
                            {
                                string newFileName = GenerateUniqueFileName(currentImagePath);
                                imageFileName = SaveImageToResources(currentImagePath, newFileName);

                                if (!string.IsNullOrEmpty(imageFileName) && !string.IsNullOrEmpty(oldPhotoName))
                                {
                                    DeleteImageFile(oldPhotoName);
                                }
                            }
                            else
                            {
                                imageFileName = oldPhotoName;
                            }

                            currentProduct.TovarName = name.Text;
                            currentProduct.TovarUnit = unit.Text;
                            currentProduct.TovarDesc = description.Text;
                            currentProduct.TovarCost = costValue;
                            currentProduct.ManufactrurerId = cbManufacturers.SelectedIndex + 1;
                            currentProduct.SupplierId = cbdelivery.SelectedIndex + 1;
                            currentProduct.TovarCategoryId = cbCategory.SelectedIndex + 1;
                            currentProduct.TovarCurrentSale = saleValue;
                            currentProduct.TovarCount = quantityValue;
                            currentProduct.TovarPhoto = imageFileName;

                            db.SaveChanges();

                            MessageBox.Show("Товар успешно обновлен", "Успешно",
                                MessageBoxButton.OK, MessageBoxImage.Information);

                            UpdateImagePreview(imageFileName);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(currentImagePath) && File.Exists(currentImagePath))
                        {
                            string newFileName = GenerateUniqueFileName(currentImagePath);
                            imageFileName = SaveImageToResources(currentImagePath, newFileName);
                        }

                        if (string.IsNullOrEmpty(imageFileName))
                        {
                            string defaultImagePath = Path.Combine(resourcesPath, "picture.png");
                            if (!File.Exists(defaultImagePath))
                            {
                                MessageBox.Show("Файл изображения по умолчанию (picture.png) не найден в папке Resources.",
                                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            imageFileName = "picture.png";
                        }
                       

                        var newProduct = new Tovar
                        {
                            TovarArticle = article.Text,
                            TovarName = name.Text,
                            TovarUnit = unit.Text,
                            TovarDesc = description.Text,
                            TovarCost = costValue,
                            ManufactrurerId = cbManufacturers.SelectedIndex + 1,
                            SupplierId = cbdelivery.SelectedIndex + 1,
                            TovarCategoryId = cbCategory.SelectedIndex + 1,
                            TovarCurrentSale = saleValue,
                            TovarCount = quantityValue,
                            TovarPhoto = imageFileName
                        };

                        db.Tovars.Add(newProduct);
                        db.SaveChanges();

                        MessageBox.Show("Товар успешно добавлен", "Успешно",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        UpdateImagePreview(imageFileName);
                    }

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n{ex.StackTrace}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static List<string> GetCategories()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.TovarCategories.Select(m => m.TovarCategoryName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке категорий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<string>();
                }
            }
        }

        private List<string> GetManufacturers()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.Manufacturers.Select(m => m.ManufacturerName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке производителей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<string>();
                }
            }
        }

        private List<string> GetSuppliers()
        {
            using (var db = new DbShoesContext())
            {
                try
                {
                    return db.Suppliers.Select(m => m.SupplierName).ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return new List<string>();
                }
            }
        }

        private void cost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!Regex.IsMatch(e.Text, "[0-9 ,]"))
            {
                e.Handled = true;
            }
        }

        private void back_btn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}