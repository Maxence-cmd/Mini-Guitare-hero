using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Guitare_hero
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Easy_Click(object sender, RoutedEventArgs e)
        {
            OpenGame("Facile");
        }

        private void Medium_Click(object sender, RoutedEventArgs e)
        {
            OpenGame("Moyen");
        }

        private void Hard_Click(object sender, RoutedEventArgs e)
        {
            OpenGame("Difficile");
        }

        private void OpenGame(string difficulty)
        {
            GameWindow game = new GameWindow(difficulty);
            game.Show();
            Close();
        }
    }
}