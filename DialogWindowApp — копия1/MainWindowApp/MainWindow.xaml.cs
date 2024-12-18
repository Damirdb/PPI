using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace MainWindowApp
{
    // Конвертер для преобразования bool в Visibility
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Главный класс окна
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private double _scaleFactor = 1.0; // Начальный масштаб

        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (_scaleFactor != value)
                {
                    _scaleFactor = value;
                    OnPropertyChanged(nameof(ScaleFactor));
                }
            }
        }

        private bool _isImagesVisible = true; // Видимость изображений

        public bool IsImagesVisible
        {
            get => _isImagesVisible;
            set
            {
                if (_isImagesVisible != value)
                {
                    _isImagesVisible = value;
                    OnPropertyChanged(nameof(IsImagesVisible));
                }
            }
        }

        public ICommand ToggleImagesCommand { get; } // Команда для переключения видимости изображений

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Инициализация команды
            ToggleImagesCommand = new RelayCommand(() =>
            {
                IsImagesVisible = !IsImagesVisible;
            });
        }

        // Обработка нажатий клавиш Ctrl + Plus и Ctrl + Minus
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Add) ScaleFactor *= 1.1;      // Увеличить масштаб
                if (e.Key == Key.Subtract) ScaleFactor *= 0.9; // Уменьшить масштаб
            }
        }

        // Обновляем масштаб при изменении значения слайдера


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ComboBoxItem selectedItem)
            {
                try
                {
                    // Преобразуем значение масштаба
                    double scaleFactor = Convert.ToDouble(selectedItem.Tag, System.Globalization.CultureInfo.InvariantCulture);

                    // Изменение размера текста в текстовых блоках
                    HeaderText.FontSize = 24 * scaleFactor;
                    BodyText1.FontSize = 14 * scaleFactor;
                    BodyText2.FontSize = 14 * scaleFactor;
                    BodyText3.FontSize = 7 * scaleFactor;
                    BodyText4.FontSize = 25 * scaleFactor;
                    BodyText5.FontSize = 10 * scaleFactor;
                    BodyText6.FontSize = 7 * scaleFactor;

                    // Применение масштаба для кнопок
                    foreach (var child in LogicalTreeHelper.GetChildren(this))
                    {
                        if (child is Button button)
                        {
                            button.Width = 115 * scaleFactor;  // Базовая ширина кнопки
                            button.Height = 29 * scaleFactor;  // Базовая высота кнопки
                            button.FontSize = 12 * scaleFactor;  // Базовый размер шрифта
                        }
                    }

                    // Применение общего масштаба
                    this.LayoutTransform = new ScaleTransform(scaleFactor, scaleFactor);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка преобразования масштаба: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Пример кнопок, можно добавить логику для каждой
        private void ClinicButton_Click(object sender, RoutedEventArgs e) { }
        private void UslugButton_Click(object sender, RoutedEventArgs e) { }
        private void CareButton_Click(object sender, RoutedEventArgs e) { }
        private void ProductButton_Click(object sender, RoutedEventArgs e) { }
        private void ReviewsButton_Click(object sender, RoutedEventArgs e) { }
        private void ContactsButton_Click(object sender, RoutedEventArgs e) { }
        private void RecordButton_Click(object sender, RoutedEventArgs e) { }

        // Реализация интерфейса INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }

    // Реализация команды
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();

        public void Execute(object parameter) => _execute();

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}