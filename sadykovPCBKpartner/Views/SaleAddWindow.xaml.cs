using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using sadykovPCBKpartner.Data;
using sadykovPCBKpartner.Models;

namespace sadykovPCBKpartner.Views
{
    /// <summary>
    /// Форма добавления записи о реализации продукции.
    /// Отдельное окно, доступное через меню «Реализация».
    /// </summary>
    public partial class SaleAddWindow : Window
    {
        private List<Product> _allProducts = new();

        public SaleAddWindow(int? preselectedPartnerId = null)
        {
            InitializeComponent();
            SaleDatePicker.SelectedDate = DateTime.Today;
            LoadPartners(preselectedPartnerId);
            LoadProducts();
        }

        private void LoadPartners(int? preselectedId)
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                var partners = ctx.Partners
                    .OrderBy(p => p.CompanyName)
                    .ToList();

                PartnerComboBox.ItemsSource = partners;

                // Если передан ID — выбираем партнёра сразу
                if (preselectedId.HasValue)
                    PartnerComboBox.SelectedItem =
                        partners.FirstOrDefault(p => p.Id == preselectedId.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке партнёров:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using var ctx = new ApplicationDbContext();
                _allProducts = ctx.Products.OrderBy(p => p.Article).ToList();
                ProductComboBox.ItemsSource = _allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке продукции:\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ArticleFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = ArticleFilterTextBox.Text.Trim();
            var filtered = string.IsNullOrEmpty(filter)
                ? _allProducts
                : _allProducts
                    .Where(p => p.Article.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            ProductComboBox.ItemsSource = filtered;

            // Сбрасываем выбор если выбранный продукт вышел из фильтра
            if (ProductComboBox.SelectedItem is Product cur && !filtered.Contains(cur))
                ProductComboBox.SelectedItem = null;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var errors = Validate();
            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "Обнаружены ошибки:\n\n" + errors + "Исправьте их и повторите попытку.",
                    "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var partner  = (Partner)PartnerComboBox.SelectedItem!;
                var product  = (Product)ProductComboBox.SelectedItem!;
                var quantity = int.Parse(QuantityTextBox.Text.Trim());

                // ВАЖНО: PostgreSQL требует дату в UTC.
                // Конвертируем Local -> UTC через DateTime.SpecifyKind
                var localDate = SaleDatePicker.SelectedDate!.Value.Date;
                var utcDate   = DateTime.SpecifyKind(localDate, DateTimeKind.Utc);

                using var ctx = new ApplicationDbContext();
                ctx.PartnerSales.Add(new PartnerSale
                {
                    PartnerId = partner.Id,
                    ProductId = product.Id,
                    Quantity  = quantity,
                    SaleDate  = utcDate
                });
                ctx.SaveChanges();

                MessageBox.Show(
                    "Запись о реализации успешно добавлена!\n\n" +
                    "Партнёр: " + partner.CompanyName + "\n" +
                    "Продукт: " + product.Article + " — " + product.ProductName + "\n" +
                    "Количество: " + quantity.ToString("N0") + " ед.\n" +
                    "Дата: " + localDate.ToString("dd.MM.yyyy"),
                    "Реализация добавлена", MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении реализации:\n\n" + GetRoot(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string Validate()
        {
            var sb = new StringBuilder();

            if (PartnerComboBox.SelectedItem == null)
                sb.AppendLine("• Необходимо выбрать партнёра.");

            if (ProductComboBox.SelectedItem == null)
                sb.AppendLine("• Необходимо выбрать продукт.");

            if (!int.TryParse(QuantityTextBox.Text.Trim(), out int qty) || qty <= 0)
                sb.AppendLine("• Количество должно быть целым положительным числом (больше 0).");

            if (SaleDatePicker.SelectedDate == null)
                sb.AppendLine("• Необходимо выбрать дату реализации.");

            return sb.ToString();
        }

        private static string GetRoot(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex.Message;
        }
    }
}
