using Microsoft.EntityFrameworkCore;
using Shoes.Model;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Shoes.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Tovar> allGoods;
        private List<Tovar> filteredGoods;
        private List<Supplier> suppliers;
        public MainWindow()
        {
            InitializeComponent();
            LoadSup();
            LoadGoods();
            Name();
            Display();
            RefreshProductImages();
        }
        private void RefreshProductImages()
        {
            if (tovars_lv.ItemsSource is System.Collections.IEnumerable items)
            {
                foreach (var item in items)
                {
                    if (item is Tovar tovar)
                    {
                        string fullPath = @"C:\Users\79174\Downloads\Telegram Desktop\Материал к заданию\Материалы к заданию\Shoes\Shoes\Resources\" +
                            (string.IsNullOrEmpty(tovar.TovarPhoto) ? "picture.png" : tovar.TovarPhoto);
                    }
                }
            }
        }
        private void LoadGoods()
        {
            using (var db = new DbShoesContext())
            {
                allGoods = db.Tovars.Include(m => m.Manufactrurer).Include(s => s.Supplier).Include(c => c.TovarCategory).ToList();
                
            }
            ApplySearchAndFilters();
        }

        private void LoadSup()
        {
            using (var db = new DbShoesContext())
            {
                filterComboBox.Items.Clear();
                suppliers = db.Suppliers.ToList();
                filterComboBox.Items.Add("Все товары");
                foreach (var i in suppliers)
                {
                    filterComboBox.Items.Add(i.SupplierName);
                }
                filterComboBox.SelectedIndex = 0;
            }
        }

        private void UpdateTovarList(List<Tovar> goods)
        {
            tovars_lv.Items.Clear();
            foreach (var good in goods)
            {
                tovars_lv.Items.Add(new TovarControl(good));
            }
        }


        private void Auth_Click(object sender, RoutedEventArgs e)
        {
            Authorization authorization = new Authorization();
            authorization.Show();
            if (App.CurrentClient != null)
            {
                App.CurrentClient = null;
            }
            this.Close();
        }
        private void Name()
        {
            if (App.CurrentClient == null)
            {
                Name_user.Text = "Вы вошли как гость";
            }
            else
            {
                Name_user.Text = $"{App.CurrentClient.UserSurname} " +
                    $"{App.CurrentClient.UserName} {App.CurrentClient.UserLastname} - {App.CurrentClient.UserRole}";
            }

        }
        private void Display()
        {
            if (App.CurrentClient == null || App.CurrentClient.UserRole == "Авторизированный клиент")
            {
                Add_tovar_btn.Visibility = Visibility.Collapsed;
                filter_grid.Visibility = Visibility.Collapsed;
                context_menu.Visibility = Visibility.Collapsed;
                order_btn.Visibility = Visibility.Collapsed;
            }
            else if (App.CurrentClient.UserRole == "Менеджер")
            {
                Add_tovar_btn.Visibility = Visibility.Collapsed;
                context_menu.Visibility = Visibility.Collapsed;
            }
        }

        private void Add_tovar_btn_Click(object sender, RoutedEventArgs e)
        {
            AddRedactTovar tovar = new AddRedactTovar();
            tovar.Show();
            this.Close();
        }
        private void ApplySearchAndFilters()
        {
            if (allGoods == null) return;

            filteredGoods = allGoods.ToList();

            var search = search_tb.Text;
            if (!string.IsNullOrWhiteSpace(search))
            {
                filteredGoods = filteredGoods.Where(g =>
                    g.TovarName != null && g.TovarName.Contains(search, StringComparison.OrdinalIgnoreCase) || g.TovarDesc.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || g.TovarUnit.Contains(search, StringComparison.OrdinalIgnoreCase) || g.TovarCategory.TovarCategoryName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    g.Supplier.SupplierName.Contains(search, StringComparison.OrdinalIgnoreCase) || g.Manufactrurer.ManufacturerName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            
            
            if (filterComboBox.SelectedIndex > 0)
            {
                var selectedSupplierName = filterComboBox.SelectedItem.ToString();
                filteredGoods = filteredGoods
                    .Where(g => g.Supplier != null && g.Supplier.SupplierName == selectedSupplierName)
                    .ToList();
            }
            if (sortComboBox.SelectedItem is ComboBoxItem sortItem)
            {
                var sort = sortItem.Content.ToString();
                switch (sort)
                {
                    case "По кол-ву (возрастание)":
                        filteredGoods = filteredGoods.OrderBy(g => g.TovarCount).ToList();
                        break;
                    case "По кол-ву (убывание)":
                        filteredGoods = filteredGoods.OrderByDescending(g => g.TovarCount).ToList();
                        break;
                }
            }
            if (filteredGoods.Count == 0)
            {
                notFound.Visibility = Visibility.Visible;
            }
            else
            {
                notFound.Visibility = Visibility.Collapsed;
            }
            UpdateTovarList(filteredGoods);
        }

        private void redact_Click(object sender, RoutedEventArgs e)
        {
            if (tovars_lv.SelectedItem is TovarControl selectedGood)
            {
                if (selectedGood.DataContext is Tovar goods)
                {
                    AddRedactTovar addGoodw = new AddRedactTovar(goods);
                    addGoodw.Show();
                    this.Close();
                }
            }
        }

        private void dell_Click(object sender, RoutedEventArgs e)
        {
            
            if(MessageBox.Show("Вы точно хотите удалить товар?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (tovars_lv.SelectedItem is TovarControl selectedGood)
                {
                    if (selectedGood.DataContext is Tovar goods)
                    {
                        using (var db = new DbShoesContext())
                        {
                            bool isInOrders = db.OrderCompositions
                                .Any(oc => oc.TovarArticle == goods.TovarArticle);

                            if (isInOrders)
                            {
                                MessageBox.Show(
                                    "Невозможно удалить товар, так как он содержится в заказах.\n" +
                                    "Сначала удалите связанные записи в заказах.",
                                    "Ошибка",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                                return;
                            }

                            Tovar good = db.Tovars.Find(goods.TovarArticle);
                            if (good == null)
                            {
                                MessageBox.Show("Товар не найден", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                                return;
                            }

                            db.Tovars.Remove(good);
                            db.SaveChanges();
                            LoadGoods();
                        }
                    }
                }
            }
        }
            
            
            
            
        

        private void filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySearchAndFilters();
        }

        private void sortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplySearchAndFilters();
        }

        private void search_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchAndFilters();
        }

        private void order_btn_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            this.Close();
        }
    }
}