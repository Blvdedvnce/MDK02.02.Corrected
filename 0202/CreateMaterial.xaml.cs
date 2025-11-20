using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _0202
{
    /// <summary>
    /// Логика взаимодействия для CreateMaterial.xaml
    /// </summary>
    public partial class CreateMaterial : Window
    {
        MDK0202Entities db = new MDK0202Entities();

        public CreateMaterial()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            TypeProduct.ItemsSource = db.ТипПродукции_
                .Select(p => new { p.Код, Name = p.Тип_продукции })
                .ToList();
            TypeProduct.DisplayMemberPath = "Name";
            TypeProduct.SelectedValuePath = "Код";

            TypeMaterial.ItemsSource = db.Тип_Материала_
                .Select(m => new { m.Код, Name = m.Тип_материала })
                .ToList();
            TypeMaterial.DisplayMemberPath = "Name";
            TypeMaterial.SelectedValuePath = "Код";

        }

        private void Calc_Click(object sender, RoutedEventArgs e)
        {
            if (TypeProduct.SelectedValue == null || TypeMaterial.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип продукции и материал!");
                return;
            }

            int productTypeId = (int)TypeProduct.SelectedValue;
            int materialTypeId = (int)TypeMaterial.SelectedValue;

            if (!int.TryParse(RequiredCount.Text, out int requiredCount) || requiredCount <= 0)
            {
                MessageBox.Show("Введите корректное количество продукции!");
                return;
            }

            if (!int.TryParse(InStock.Text, out int inStock) || inStock < 0)
            {
                MessageBox.Show("Введите корректное количество на складе!");
                return;
            }

            if (!double.TryParse(Param1.Text, out double param1) || param1 <= 0 ||
                !double.TryParse(Param2.Text, out double param2) || param2 <= 0)
            {
                MessageBox.Show("Введите корректные параметры!");
                return;
            }

            int neededMaterial = CalculateRequiredMaterial(productTypeId, materialTypeId, requiredCount, inStock, param1, param2);
            if (neededMaterial == -1)
            {
                MessageBox.Show("Ошибка данных: тип продукции или материала не найден!");
                return;
            }

            Result.Content = $"Количество материала: {neededMaterial}";
        }
        private int CalculateRequiredMaterial(int productTypeId, int materialTypeId, int requiredCount, int inStock, double param1, double param2)
        {
            // Получаем коэффициент типа продукции
            var productType = db.ТипПродукции_.FirstOrDefault(p => p.Код == productTypeId);
            if (productType == null) return -1;
            double productCoeff = productType.Коэффициент_типа_продукции ?? 1; // если null, считаем 1

            // Получаем процент брака материала
            var materialType = db.Тип_Материала_.FirstOrDefault(m => m.Код == materialTypeId);
            if (materialType == null) return -1;
            double defectPercent = materialType.Процент_брака_материала ?? 0; // в процентах

            // Определяем сколько продукции нужно произвести
            int toProduce = Math.Max(requiredCount - inStock, 0);

            // Расчет материала на одну единицу
            double materialPerUnit = param1 * param2 * productCoeff;

            // Учитываем брак
            materialPerUnit *= (1 + defectPercent / 100.0);

            // Итоговое количество материала
            int totalMaterial = (int)Math.Ceiling(materialPerUnit * toProduce);

            return totalMaterial;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow back = new MainWindow();
            back.Show();
            this.Close();
        }
    }
}
