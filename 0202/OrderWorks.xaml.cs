using System;
using System.Linq;
using System.Windows;

namespace _0202
{
    public partial class OrderWorks : Window
    {
        MDK0202Entities db = new MDK0202Entities();
        public int? EditingId;

        public OrderWorks()
        {
            InitializeComponent();

            TypePartner.ItemsSource = db.ТипПартнера_.ToList();
            TypePartner.DisplayMemberPath = "Наименование";
            TypePartner.SelectedValuePath = "Код";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (EditingId.HasValue)
            {
                var p = db.Партнеры_.FirstOrDefault(x => x.Код == EditingId.Value);

                if (p != null)
                {
                    TypePartner.SelectedValue = p.Тип_партнера;
                    PartnerName.Text = p.Наименование_партнера;
                    DirectorFIO.Text = p.Директор;
                    Address.Text = p.Юридический_адрес_партнера;
                    Rating.Text = p.Рейтинг?.ToString() ?? "0";
                    Phone.Text = p.Телефон_партнера;
                    Email.Text = p.Электронная_почта_партнера;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Owner.Show();
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Проверки
            if (TypePartner.SelectedValue == null)
            {
                MessageBox.Show("Выберите тип партнера!");
                return;
            }

            if (string.IsNullOrWhiteSpace(PartnerName.Text))
            {
                MessageBox.Show("Введите наименование партнера!");
                return;
            }

            if (!int.TryParse(Rating.Text, out int rating) || rating < 0)
            {
                MessageBox.Show("Рейтинг должен быть целым неотрицательным числом!");
                return;
            }

            if (!Phone.Text.All(c => char.IsDigit(c) || c == ' '))
            {
                MessageBox.Show("Телефон может содержать только цифры и пробелы!");
                return;
            }
            if (string.IsNullOrWhiteSpace(Email.Text))
            {
                MessageBox.Show("Введите Email!");
                return;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(
                    Email.Text,
                    @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$"))
            {
                MessageBox.Show("Некорректный формат Email!");
                return;
            }

            try
            {
                Партнеры_ p;

                if (EditingId.HasValue)
                {
                    p = db.Партнеры_.First(x => x.Код == EditingId.Value);
                }
                else
                {
                    p = new Партнеры_();
                    db.Партнеры_.Add(p);
                }

                p.Тип_партнера = (int)TypePartner.SelectedValue;
                p.Наименование_партнера = PartnerName.Text.Trim();
                p.Директор = DirectorFIO.Text.Trim();
                p.Юридический_адрес_партнера = Address.Text.Trim();
                p.Рейтинг = rating;
                p.Телефон_партнера = Phone.Text.Trim();
                p.Электронная_почта_партнера = Email.Text.Trim();

                db.SaveChanges();

                MessageBox.Show("Заявка сохранена!");

                new MainWindow().Show();
                this.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения данных! Проверьте все введенные данные");
            }
        }

    }
}
