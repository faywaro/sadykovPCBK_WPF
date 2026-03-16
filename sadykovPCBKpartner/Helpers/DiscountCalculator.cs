namespace sadykovPCBKpartner.Helpers
{
    /// <summary>
    /// Вспомогательный класс для расчёта индивидуальной скидки партнёра.
    /// Скидка зависит от суммарного количества реализованной продукции за весь период работы.
    /// </summary>
    public static class DiscountCalculator
    {
        // Пороговые значения количества проданной продукции
        private const int ThresholdLevel1 = 10_000;
        private const int ThresholdLevel2 = 50_000;
        private const int ThresholdLevel3 = 300_000;

        // Процентные значения скидок
        private const int DiscountLevel0 = 0;
        private const int DiscountLevel1 = 5;
        private const int DiscountLevel2 = 10;
        private const int DiscountLevel3 = 15;

        /// <summary>
        /// Рассчитывает размер скидки (в процентах) для партнёра
        /// на основании суммарного количества реализованной продукции.
        /// </summary>
        /// <param name="totalQuantity">Общее количество реализованной продукции</param>
        /// <returns>Скидка в процентах: 0, 5, 10 или 15</returns>
        public static int CalculateDiscount(int totalQuantity)
        {
            if (totalQuantity >= ThresholdLevel3)
                return DiscountLevel3;

            if (totalQuantity >= ThresholdLevel2)
                return DiscountLevel2;

            if (totalQuantity >= ThresholdLevel1)
                return DiscountLevel1;

            // Менее 10 000 единиц — скидка отсутствует
            return DiscountLevel0;
        }
    }
}
