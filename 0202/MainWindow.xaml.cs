using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace _0202
{
    public partial class MainWindow : Window
    {
        MDK0202Entities db = new MDK0202Entities();

        public MainWindow()
        {
            InitializeComponent();
            LoadPartners();
            ListBoxPartners.MouseDoubleClick += ListBoxPartners_MouseDoubleClick;
        }

        private void LoadPartners()
        {
            var partners = db.Партнеры_.ToList();

            var displayList = partners.Select(p => new
            {
                p.Код,
                PartnerDisplay = $"{GetPartnerTypeName((int)p.Тип_партнера)} | {p.Наименование_партнера ?? ""}",
                Address = p.Юридический_адрес_партнера ?? "",
                Phone = p.Телефон_партнера ?? "",
                Rating = p.Рейтинг ?? 0,
                CostText = $"{CalculatePartnerCost(p.Код):F2} р"
            }).ToList();

            ListBoxPartners.ItemsSource = displayList;
        }
        private string GetPartnerTypeName(int typeId)
        {
            var type = db.ТипПартнера_.FirstOrDefault(t => t.Код == typeId);
            return type?.Наименование ?? "Неизвестно";
        }
        private void ListBoxPartners_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBoxPartners.SelectedItem == null)
                return;
            var item = ListBoxPartners.SelectedItem;
            int id = (int)item.GetType().GetProperty("Код").GetValue(item);
            // Открываем окно редактирования
            OrderWorks win = new OrderWorks();
            win.EditingId = id;
            win.Owner = this;
            win.Show();
            this.Hide();
        }

        // РАСЧЁТ СТОИМОСТИ 
        private double CalculatePartnerCost(int partnerId)
        {
            // Берем все продукты этого партнёра
            var items = db.ПродуктыПартнера_.Where(x => x.КодПартнера == partnerId).ToList();

            double total = 0;

            foreach (var i in items)
            {
                var product = db.Продукция_.FirstOrDefault(p => p.Код == i.КодПродукции);

                if (product == null)
                    continue;

                double quantity = i.Количество_продукции ?? 0;
                double minPrice = product.Минимальная_стоимость_для_партнера ?? 0;

                double posCost = quantity * minPrice;
                if (posCost < 0) posCost = 0;

                total += posCost;
            }

            return Math.Round(total, 2);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OrderWorks win = new OrderWorks();
            win.Owner = this;
            win.Show();
            this.Hide(); 
        }

        public void RefreshPartners()
        {
            LoadPartners();
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ListBoxPartners.SelectedItem == null)
                return;

            var item = ListBoxPartners.SelectedItem;
            int partnerId = (int)item.GetType().GetProperty("Код").GetValue(item);

            ProductsHelp win = new ProductsHelp();
            win.PartnerId = partnerId;
            win.Owner = this;
            win.Show();
            this.Hide();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            CreateMaterial Ame = new CreateMaterial();
            Ame.Show();
            this.Close();
        }
    }
}
