using NUnit.Framework;
using sadykovPCBKpartner.Helpers;

namespace sadykovPCBKpartner.Tests
{
    /// <summary>
    /// Модульные тесты для класса DiscountCalculator.
    /// Проверяют корректность расчёта индивидуальной скидки партнёра.
    /// </summary>
    [TestFixture]
    public class DiscountCalculatorTests
    {
        // ------- Граничные случаи: нулевое и отрицательное количество -------

        [Test]
        public void CalculateDiscount_ZeroQuantity_ReturnsZeroPercent()
        {
            int result = DiscountCalculator.CalculateDiscount(0);
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void CalculateDiscount_NegativeQuantity_ReturnsZeroPercent()
        {
            // Отрицательное значение по смыслу предметной области невозможно,
            // но метод должен возвращать 0, а не выбрасывать исключение.
            int result = DiscountCalculator.CalculateDiscount(-100);
            Assert.That(result, Is.EqualTo(0));
        }

        // ------- Диапазон 0 – 9 999: скидка 0% -------

        [Test]
        public void CalculateDiscount_LessThan10000_ReturnsZeroPercent()
        {
            Assert.That(DiscountCalculator.CalculateDiscount(9_999),  Is.EqualTo(0));
            Assert.That(DiscountCalculator.CalculateDiscount(1),      Is.EqualTo(0));
            Assert.That(DiscountCalculator.CalculateDiscount(5_000),  Is.EqualTo(0));
        }

        // ------- Граница 10 000: скидка 5% -------

        [Test]
        public void CalculateDiscount_Exactly10000_ReturnsFivePercent()
        {
            int result = DiscountCalculator.CalculateDiscount(10_000);
            Assert.That(result, Is.EqualTo(5));
        }

        // ------- Диапазон 10 000 – 49 999: скидка 5% -------

        [Test]
        public void CalculateDiscount_Between10000And49999_ReturnsFivePercent()
        {
            Assert.That(DiscountCalculator.CalculateDiscount(10_001),  Is.EqualTo(5));
            Assert.That(DiscountCalculator.CalculateDiscount(25_000),  Is.EqualTo(5));
            Assert.That(DiscountCalculator.CalculateDiscount(49_999),  Is.EqualTo(5));
        }

        // ------- Граница 50 000: скидка 10% -------

        [Test]
        public void CalculateDiscount_Exactly50000_ReturnsTenPercent()
        {
            int result = DiscountCalculator.CalculateDiscount(50_000);
            Assert.That(result, Is.EqualTo(10));
        }

        // ------- Диапазон 50 000 – 299 999: скидка 10% -------

        [Test]
        public void CalculateDiscount_Between50000And299999_ReturnsTenPercent()
        {
            Assert.That(DiscountCalculator.CalculateDiscount(50_001),   Is.EqualTo(10));
            Assert.That(DiscountCalculator.CalculateDiscount(150_000),  Is.EqualTo(10));
            Assert.That(DiscountCalculator.CalculateDiscount(299_999),  Is.EqualTo(10));
        }

        // ------- Граница 300 000: скидка 15% -------

        [Test]
        public void CalculateDiscount_Exactly300000_ReturnsFifteenPercent()
        {
            int result = DiscountCalculator.CalculateDiscount(300_000);
            Assert.That(result, Is.EqualTo(15));
        }

        // ------- Более 300 000: скидка 15% -------

        [Test]
        public void CalculateDiscount_MoreThan300000_ReturnsFifteenPercent()
        {
            Assert.That(DiscountCalculator.CalculateDiscount(300_001),   Is.EqualTo(15));
            Assert.That(DiscountCalculator.CalculateDiscount(1_000_000), Is.EqualTo(15));
        }

        // ------- Проверка всех возможных значений скидки -------

        [TestCase(0,         ExpectedResult = 0)]
        [TestCase(9_999,     ExpectedResult = 0)]
        [TestCase(10_000,    ExpectedResult = 5)]
        [TestCase(49_999,    ExpectedResult = 5)]
        [TestCase(50_000,    ExpectedResult = 10)]
        [TestCase(299_999,   ExpectedResult = 10)]
        [TestCase(300_000,   ExpectedResult = 15)]
        [TestCase(999_999,   ExpectedResult = 15)]
        public int CalculateDiscount_ParametrizedCases_ReturnsCorrectDiscount(int quantity)
        {
            return DiscountCalculator.CalculateDiscount(quantity);
        }
    }
}
