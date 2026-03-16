using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using sadykovPCBKpartner.Data;
using sadykovPCBKpartner.Models;
using sadykovPCBKpartner.ViewModels;

namespace sadykovPCBKpartner.Views
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            LoadPartners();
            LoadProducts();
        }

        // ================================================================
        // ПАРТНЁРЫ
        // ================================================================

        private void LoadPartners()
        {
            try
            {
                var partners = _context.Partners
                    .Include(p => p.PartnerType)
                    .Include(p => p.Sales)
                    .ThenInclude(s => s.Product)
                    .OrderBy(p => p.CompanyName)
                    .ToList();

                PartnersListBox.ItemsSource = partners
                    .Select(p => new PartnerViewModel(p))
                    .ToList();

                SalesDataGrid.ItemsSource = null;
                SalesHistoryHeader.Text = "Выберите партнёра для просмотра истории реализации";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка при загрузке списка партнёров:\n" + GetRoot(ex) +
                    "\n\nПроверьте подключение к базе данных.",
                    "Ошибка подключения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PartnersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PartnersListBox.SelectedItem is not PartnerViewModel vm)
            {
                SalesDataGrid.ItemsSource = null;
                SalesHistoryHeader.Text = "Выберите партнёра для просмотра истории реализации";
                return;
            }
            try
            {
                var sales = _context.PartnerSales
                    .Where(s => s.PartnerId == vm.Id)
                    .Include(s => s.Product)
                    .OrderByDescending(s => s.SaleDate)
                    .ToList();

                SalesDataGrid.ItemsSource = sales;
                SalesHistoryHeader.Text = "История реализации: " + vm.CompanyName;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке истории реализации:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PartnersListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenPartnerEditForm();
        }

        private void MenuAddPartner_Click(object sender, RoutedEventArgs e)
        {
            var win = new PartnerEditWindow(null);
            win.Owner = this;
            if (win.ShowDialog() == true) LoadPartners();
        }

        private void MenuEditPartner_Click(object sender, RoutedEventArgs e)
        {
            OpenPartnerEditForm();
        }

        private void MenuShowHistory_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersListBox.SelectedItem == null)
                MessageBox.Show("Выберите партнёра для просмотра истории реализации.",
                    "Подсказка", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void MenuDeletePartner_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersListBox.SelectedItem is not PartnerViewModel vm)
            {
                MessageBox.Show("Сначала выберите партнёра в списке.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show(
                    "Удалить партнёра «" + vm.CompanyName + "»?\n" +
                    "Все связанные записи истории реализации также будут удалены.\n\nЭто действие необратимо.",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;
            try
            {
                var p = _context.Partners.Find(vm.Id);
                if (p != null) { _context.Partners.Remove(p); _context.SaveChanges(); LoadPartners(); }
                MessageBox.Show("Партнёр успешно удалён.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении партнёра:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenPartnerEditForm()
        {
            if (PartnersListBox.SelectedItem is not PartnerViewModel vm)
            {
                MessageBox.Show("Сначала выберите партнёра из списка для редактирования.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var partner = _context.Partners.Include(p => p.PartnerType).FirstOrDefault(p => p.Id == vm.Id);
            if (partner == null)
            {
                MessageBox.Show("Не удалось найти данные партнёра.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var win = new PartnerEditWindow(partner);
            win.Owner = this;
            if (win.ShowDialog() == true) LoadPartners();
        }

        // ================================================================
        // РЕАЛИЗАЦИЯ
        // ================================================================

        private void MenuAddSale_Click(object sender, RoutedEventArgs e)
        {
            // Если партнёр уже выбран в списке — передаём его сразу
            int? preselectedPartnerId = null;
            if (PartnersListBox.SelectedItem is PartnerViewModel vm)
                preselectedPartnerId = vm.Id;

            var win = new SaleAddWindow(preselectedPartnerId);
            win.Owner = this;
            if (win.ShowDialog() == true)
            {
                // Обновляем историю и список (скидка могла измениться)
                LoadPartners();
                // Восстанавливаем выбор партнёра если он был
                if (preselectedPartnerId.HasValue)
                {
                    foreach (var item in PartnersListBox.Items)
                    {
                        if (item is PartnerViewModel pvm && pvm.Id == preselectedPartnerId.Value)
                        {
                            PartnersListBox.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        // ================================================================
        // ПРОДУКТЫ
        // ================================================================

        private void LoadProducts()
        {
            try
            {
                ProductsDataGrid.ItemsSource = _context.Products
                    .OrderBy(p => p.ProductType)
                    .ThenBy(p => p.ProductName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке продукции:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenProductEditForm();
        }

        private void MenuAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var win = new ProductEditWindow(null);
            win.Owner = this;
            if (win.ShowDialog() == true) LoadProducts();
        }

        private void MenuEditProduct_Click(object sender, RoutedEventArgs e)
        {
            OpenProductEditForm();
        }

        private void MenuDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is not Product product)
            {
                MessageBox.Show("Сначала выберите продукт в таблице.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (MessageBox.Show(
                    "Удалить продукт «" + product.ProductName + "» (арт. " + product.Article + ")?\n\n" +
                    "Если продукт используется в истории реализации, удаление будет запрещено.\nЭто действие необратимо.",
                    "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;
            try
            {
                var e2 = _context.Products.Find(product.Id);
                if (e2 != null) { _context.Products.Remove(e2); _context.SaveChanges(); LoadProducts(); }
                MessageBox.Show("Продукт успешно удалён.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) when (GetRoot(ex).Contains("partner_sales"))
            {
                MessageBox.Show(
                    "Нельзя удалить продукт «" + product.ProductName + "»: он используется в истории реализации.\n\n" +
                    "Сначала удалите все записи о продажах этого продукта.",
                    "Удаление запрещено", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении продукта:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenProductEditForm()
        {
            if (ProductsDataGrid.SelectedItem is not Product product)
            {
                MessageBox.Show("Сначала выберите продукт в таблице для редактирования.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var entity = _context.Products.Find(product.Id);
            if (entity == null)
            {
                MessageBox.Show("Не удалось найти продукт.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var win = new ProductEditWindow(entity);
            win.Owner = this;
            if (win.ShowDialog() == true) LoadProducts();
        }

        // ================================================================
        // ОБЩЕЕ
        // ================================================================

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Выйти из приложения?", "Выход",
                    MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                Application.Current.Shutdown();
        }

        private static string GetRoot(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex.Message;
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
