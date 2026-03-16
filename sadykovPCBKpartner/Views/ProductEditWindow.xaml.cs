using System;
using System.Text;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using sadykovPCBKpartner.Data;
using sadykovPCBKpartner.Models;

namespace sadykovPCBKpartner.Views
{
    /// <summary>
    /// Форма добавления / редактирования продукта.
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly Product? _editingProduct;

        public ProductEditWindow(Product? product)
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _editingProduct = product;

            if (_editingProduct != null)
            {
                Title = "Редактирование продукта";
                FormTitleBlock.Text = "Редактирование продукта";
                FillFields(_editingProduct);
            }
            else
            {
                Title = "Добавление продукта";
                FormTitleBlock.Text = "Новый продукт";
            }
        }

        private void FillFields(Product product)
        {
            ArticleTextBox.Text     = product.Article;
            ProductNameTextBox.Text = product.ProductName;
            ProductTypeTextBox.Text = product.ProductType;
            MinPriceTextBox.Text    = product.MinPrice.ToString("F2");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var errors = Validate();
            if (errors.Length > 0)
            {
                MessageBox.Show(
                    "Обнаружены ошибки при заполнении формы:\n\n" + errors +
                    "\nИсправьте указанные ошибки и попробуйте снова.",
                    "Ошибки ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (_editingProduct == null)
                {
                    var newProduct = new Product
                    {
                        Article     = ArticleTextBox.Text.Trim(),
                        ProductName = ProductNameTextBox.Text.Trim(),
                        ProductType = ProductTypeTextBox.Text.Trim(),
                        MinPrice    = decimal.Parse(MinPriceTextBox.Text.Trim())
                    };
                    _context.Products.Add(newProduct);
                }
                else
                {
                    var entity = _context.Products.Find(_editingProduct.Id);
                    if (entity == null)
                    {
                        MessageBox.Show("Продукт не найден в базе данных. Возможно, он был удалён.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    entity.Article     = ArticleTextBox.Text.Trim();
                    entity.ProductName = ProductNameTextBox.Text.Trim();
                    entity.ProductType = ProductTypeTextBox.Text.Trim();
                    entity.MinPrice    = decimal.Parse(MinPriceTextBox.Text.Trim());
                }

                _context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (DbUpdateException ex) when (
                ex.InnerException?.Message.Contains("products_article_key") == true)
            {
                MessageBox.Show(
                    "Продукт с артикулом «" + ArticleTextBox.Text.Trim() + "» уже существует.\n" +
                    "Укажите другой артикул и повторите сохранение.",
                    "Дублирующийся артикул", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении продукта:\n" + ex.Message,
                    "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
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

            if (string.IsNullOrWhiteSpace(ArticleTextBox.Text))
                sb.AppendLine("• Поле «Артикул» не может быть пустым.");

            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
                sb.AppendLine("• Поле «Наименование» не может быть пустым.");

            if (string.IsNullOrWhiteSpace(ProductTypeTextBox.Text))
                sb.AppendLine("• Поле «Тип продукта» не может быть пустым.");

            var priceText = MinPriceTextBox.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(priceText,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal price) || price < 0)
                sb.AppendLine("• «Минимальная цена» должна быть неотрицательным числом (например: 850 или 850.00).");

            return sb.ToString();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context.Dispose();
            base.OnClosed(e);
        }
    }
}
