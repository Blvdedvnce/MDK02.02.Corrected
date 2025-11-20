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
    public partial class ProductsHelp : Window
    {
        MDK0202Entities db = new MDK0202Entities();

        // ID партнёра, для которого показываем продукцию
        public int PartnerId { get; set; }

        public ProductsHelp()
        {
            InitializeComponent();
            Loaded += ProductsHelp_Loaded;
        }

        private void ProductsHelp_Loaded(object sender, RoutedEventArgs e)
        {
            LoadProductsForPartner();
        }

        private void LoadProductsForPartner()
        {
            // Получаем список продуктов для данного партнёра
            var partnerProducts = db.ПродуктыПартнера_
                                    .Where(pp => pp.КодПартнера == PartnerId)
                                    .ToList();

            // Создаем объект для отображения
            var displayList = partnerProducts.Select(pp =>
            {
                var product = db.Продукция_.FirstOrDefault(p => p.Код == pp.КодПродукции);
                if (product == null) return null;

                return new
                {
                    Name = product.Наименование_продукции ?? "",
                    Quantity = pp.Количество_продукции ?? 0,
                    MinPrice = product.Минимальная_стоимость_для_партнера ?? 0
                };
            })
            .Where(x => x != null)
            .ToList();

            ProductsList.ItemsSource = displayList;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
