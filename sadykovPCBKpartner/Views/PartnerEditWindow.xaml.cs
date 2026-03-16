using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using sadykovPCBKpartner.Data;
using sadykovPCBKpartner.Models;

namespace sadykovPCBKpartner.Views
{
    /// <summary>
    /// Форма добавления / редактирования данных о партнёре.
    /// Также позволяет добавлять записи о реализации продукции.
    /// </summary>
    public partial class PartnerEditWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Partner? _editingPartner;
        private List<Product> _allProducts = new();

        public PartnerEditWindow(Partner? partner)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _editingPartner = partner;

            LoadPartnerTypes();
            LoadProducts();

            SaleDatePicker.SelectedDate = DateTime.Today;

            if (_editingPartner != null)
            {
                Title = "Редактирование партнёра";
                FormTitleBlock.Text = "Редактирование данных партнёра";
                FillFields(_editingPartner);
            }
            else
            {
                Title = "Добавление нового партнёра";
                FormTitleBlock.Text = "Новый партнёр";
            }
        }

        // ================================================================
        // ЗАГРУЗКА ДАННЫХ
        // ================================================================

        private void LoadPartnerTypes()
        {
            try
            {
                PartnerTypeComboBox.ItemsSource = _context.PartnerTypes
                    .OrderBy(t => t.TypeName)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке типов партнёров:\n" + GetRootMessage(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                _allProducts = _context.Products
                    .OrderBy(p => p.Article)
                    .ToList();
                ProductComboBox.ItemsSource = _allProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке списка продукции:\n" + GetRootMessage(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FillFields(Partner partner)
        {
            PartnerTypeComboBox.SelectedValue = partner.TypeId;
            CompanyNameTextBox.Text   = partner.CompanyName;
            LegalAddressTextBox.Text  = partner.LegalAddress;
            InnTextBox.Text           = partner.Inn;
            DirectorNameTextBox.Text  = partner.DirectorName;
            PhoneTextBox.Text         = partner.Phone;
            EmailTextBox.Text         = partner.Email;
            RatingTextBox.Text        = partner.Rating.ToString();
        }

        // ================================================================
        // ФИЛЬТРАЦИЯ ПРОДУКТОВ ПО АРТИКУЛУ
        // ================================================================

        private void ArticleFilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = ArticleFilterTextBox.Text.Trim();

            if (string.IsNullOrEmpty(filter))
            {
                ProductComboBox.ItemsSource = _allProducts;
            }
            else
            {
                var filtered = _allProducts
                    .Where(p => p.Article.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                ProductComboBox.ItemsSource = filtered;
            }

            if (ProductComboBox.SelectedItem is Product current &&
                ProductComboBox.ItemsSource is List<Product> list &&
                !list.Contains(current))
            {
                ProductComboBox.SelectedItem = null;
            }
        }

        // ================================================================
        // ДОБАВЛЕНИЕ ЗАПИСИ О РЕАЛИЗАЦИИ
        // ================================================================

        private void AddSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_editingPartner == null)
            {
                MessageBox.Show(
                    "Для добавления записи о реализации сначала сохраните нового партнёра.\n\n" +
                    "Нажмите «Сохранить», затем откройте форму редактирования партнёра снова.",
                    "Подсказка", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var errors = ValidateSaleFields();
            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "Ошибки при заполнении полей реализации:\n\n" + errors +
                    "Исправьте ошибки и повторите попытку.",
                    "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var product  = (Product)ProductComboBox.SelectedItem!;
                var quantity = int.Parse(QuantityTextBox.Text.Trim());
                var rawDate  = SaleDatePicker.SelectedDate!.Value.Date;
                var date     = DateTime.SpecifyKind(rawDate, DateTimeKind.Utc);

                // Используем новый контекст чтобы избежать конфликтов отслеживания
                using var ctx = new ApplicationDbContext();
                ctx.PartnerSales.Add(new PartnerSale
                {
                    PartnerId = _editingPartner.Id,
                    ProductId = product.Id,
                    Quantity  = quantity,
                    SaleDate  = date
                });
                ctx.SaveChanges();

                ArticleFilterTextBox.Text   = string.Empty;
                ProductComboBox.SelectedItem = null;
                QuantityTextBox.Text         = string.Empty;
                SaleDatePicker.SelectedDate  = DateTime.Today;

                MessageBox.Show(
                    "Запись о реализации добавлена:\n" +
                    product.Article + " — " + product.ProductName + "\n" +
                    "Количество: " + quantity.ToString("N0") + " ед.  Дата: " + date.ToString("dd.MM.yyyy"),
                    "Реализация добавлена", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Показываем корневую причину ошибки, а не общее сообщение EF Core
                MessageBox.Show(
                    "Ошибка при добавлении реализации:\n\n" + GetRootMessage(ex),
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string ValidateSaleFields()
        {
            var sb = new StringBuilder();

            if (ProductComboBox.SelectedItem == null)
                sb.AppendLine("• Необходимо выбрать продукт из списка.");

            if (!int.TryParse(QuantityTextBox.Text.Trim(), out int qty) || qty <= 0)
                sb.AppendLine("• Количество должно быть целым положительным числом.");

            if (SaleDatePicker.SelectedDate == null)
                sb.AppendLine("• Необходимо выбрать дату реализации.");

            return sb.ToString();
        }

        // ================================================================
        // СОХРАНЕНИЕ ПАРТНЁРА
        // ================================================================

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var errors = ValidatePartnerFields();
            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "Обнаружены ошибки при заполнении формы:\n\n" + errors +
                    "Исправьте указанные ошибки и попробуйте снова.",
                    "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_editingPartner == null)
                {
                    _context.Partners.Add(new Partner
                    {
                        TypeId       = (int)PartnerTypeComboBox.SelectedValue!,
                        CompanyName  = CompanyNameTextBox.Text.Trim(),
                        LegalAddress = LegalAddressTextBox.Text.Trim(),
                        Inn          = InnTextBox.Text.Trim(),
                        DirectorName = DirectorNameTextBox.Text.Trim(),
                        Phone        = PhoneTextBox.Text.Trim(),
                        Email        = EmailTextBox.Text.Trim(),
                        Rating       = int.Parse(RatingTextBox.Text.Trim())
                    });
                }
                else
                {
                    var partner = _context.Partners.Find(_editingPartner.Id);
                    if (partner == null)
                    {
                        MessageBox.Show("Партнёр не найден в базе данных. Возможно, он был удалён.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    partner.TypeId       = (int)PartnerTypeComboBox.SelectedValue!;
                    partner.CompanyName  = CompanyNameTextBox.Text.Trim();
                    partner.LegalAddress = LegalAddressTextBox.Text.Trim();
                    partner.Inn          = InnTextBox.Text.Trim();
                    partner.DirectorName = DirectorNameTextBox.Text.Trim();
                    partner.Phone        = PhoneTextBox.Text.Trim();
                    partner.Email        = EmailTextBox.Text.Trim();
                    partner.Rating       = int.Parse(RatingTextBox.Text.Trim());
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("partners_inn_key") == true)
            {
                MessageBox.Show(
                    "Партнёр с ИНН «" + InnTextBox.Text.Trim() + "» уже существует.\n" +
                    "Укажите другой ИНН и повторите сохранение.",
                    "Дублирующийся ИНН", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных партнёра:\n\n" + GetRootMessage(ex),
                    "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string ValidatePartnerFields()
        {
            var sb = new StringBuilder();

            if (PartnerTypeComboBox.SelectedValue == null)
                sb.AppendLine("• Необходимо выбрать тип партнёра.");

            if (string.IsNullOrWhiteSpace(CompanyNameTextBox.Text))
                sb.AppendLine("• Поле «Наименование компании» не может быть пустым.");

            if (string.IsNullOrWhiteSpace(LegalAddressTextBox.Text))
                sb.AppendLine("• Поле «Юридический адрес» не может быть пустым.");

            var inn = InnTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(inn))
                sb.AppendLine("• Поле «ИНН» не может быть пустым.");
            else if (!System.Text.RegularExpressions.Regex.IsMatch(inn, @"^\d{10,12}$"))
                sb.AppendLine("• ИНН должен содержать от 10 до 12 цифр.");

            if (string.IsNullOrWhiteSpace(DirectorNameTextBox.Text))
                sb.AppendLine("• Поле «ФИО директора» не может быть пустым.");

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
                sb.AppendLine("• Поле «Телефон» не может быть пустым.");

            var email = EmailTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(email))
                sb.AppendLine("• Поле «Email» не может быть пустым.");
            else if (!email.Contains('@') || !email.Contains('.'))
                sb.AppendLine("• Поле «Email» содержит некорректный адрес.");

            if (!int.TryParse(RatingTextBox.Text.Trim(), out int r) || r < 0)
                sb.AppendLine("• «Рейтинг» должен быть целым неотрицательным числом (≥ 0).");

            return sb.ToString();
        }

        // ================================================================
        // ВСПОМОГАТЕЛЬНЫЙ МЕТОД
        // ================================================================

        /// <summary>
        /// Раскрывает цепочку InnerException до корневой причины ошибки.
        /// Это необходимо, потому что EF Core оборачивает реальную ошибку БД
        /// в общее сообщение DbUpdateException.
        /// </summary>
        private static string GetRootMessage(Exception ex)
        {
            var current = ex;
            while (current.InnerException != null)
                current = current.InnerException;
            return current.Message;
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
