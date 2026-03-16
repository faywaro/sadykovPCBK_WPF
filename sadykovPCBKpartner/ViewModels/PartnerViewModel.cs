using System.ComponentModel;
using System.Linq;
using sadykovPCBKpartner.Models;
using sadykovPCBKpartner.Helpers;

namespace sadykovPCBKpartner.ViewModels
{
    /// <summary>
    /// ViewModel для отображения партнёра в списке главной формы.
    /// Рассчитывает скидку на основе истории продаж.
    /// </summary>
    public class PartnerViewModel : INotifyPropertyChanged
    {
        private readonly Partner _partner;

        public PartnerViewModel(Partner partner)
        {
            _partner = partner;
        }

        public int    Id          => _partner.Id;
        public string CompanyName => _partner.CompanyName;
        public string TypeName    => _partner.PartnerType?.TypeName ?? "—";
        public string Director    => _partner.DirectorName;
        public string Phone       => _partner.Phone;
        public int    Rating      => _partner.Rating;
        public string Email       => _partner.Email;
        public string LogoPath    => _partner.LogoPath ?? string.Empty;

        /// <summary>
        /// Суммарное количество реализованной продукции за весь период.
        /// </summary>
        public int TotalSalesQuantity =>
            _partner.Sales?.Sum(s => s.Quantity) ?? 0;

        /// <summary>
        /// Индивидуальная скидка партнёра в процентах.
        /// </summary>
        public int DiscountPercent =>
            DiscountCalculator.CalculateDiscount(TotalSalesQuantity);

        /// <summary>
        /// Отображаемая строка скидки, например «10%».
        /// </summary>
        public string DiscountDisplay => $"{DiscountPercent}%";

        /// <summary>
        /// Первая строка карточки: «Тип | Наименование»
        /// </summary>
        public string CardTitle => $"{TypeName} | {CompanyName}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
