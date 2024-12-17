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

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel(); // Установка DataContext на ViewModel
            this.KeyDown += new KeyEventHandler(Window_KeyDown);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Обработка нажатия Ctrl + Plus или Ctrl + Minus для изменения масштаба
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Add) // Увеличение масштаба
                {
                    ((MainViewModel)DataContext).ScaleFactor *= 1.1;
                }
                else if (e.Key == Key.Subtract) // Уменьшение масштаба
                {
                    ((MainViewModel)DataContext).ScaleFactor *= 0.9;
                }
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double letterSpacing = e.NewValue;

            // Применяем межбуквенный интервал ко всем текстовым блокам
            ApplyLetterSpacing(HeaderText, letterSpacing);
            ApplyLetterSpacing(BodyText1, letterSpacing);
            ApplyLetterSpacing(BodyText2, letterSpacing);
        }
        private void ApplyLetterSpacing(TextBlock textBlock, double spacing)
        {
            // Создаем TextEffect для изменения межбуквенного интервала
            textBlock.TextEffects = new TextEffectCollection
    {
        new TextEffect
        {
            PositionStart = 0,
            PositionCount = textBlock.Text.Length,
            Transform = new TranslateTransform(spacing, 0)
        }
    };
        }

        // ViewModel с необходимыми свойствами и командами
        public class MainViewModel : INotifyPropertyChanged
        {
            private double _scaleFactor = 1.0;
            private bool _isImagesVisible = true;

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

            public ICommand ToggleImagesCommand { get; }

            public event PropertyChangedEventHandler PropertyChanged;

            public MainViewModel()
            {
                ToggleImagesCommand = new RelayCommand(ToggleImages);
            }

            private void ToggleImages()
            {
                IsImagesVisible = !IsImagesVisible;
            }

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Команда для обработки действий
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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                var selectedItem = comboBox.SelectedItem as ComboBoxItem;
                MessageBox.Show("Вы выбрали: " + selectedItem.Content.ToString());
            }
        }
    }
}
