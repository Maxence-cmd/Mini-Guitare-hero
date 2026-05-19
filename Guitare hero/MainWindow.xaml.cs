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

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        // Ferme la fenêtre
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Sélection des musiques
        private void Song1_Click(object sender, MouseButtonEventArgs e) { /* Ta logique */ }
        private void Song2_Click(object sender, MouseButtonEventArgs e) { /* Ta logique */ }
        private void Song3_Click(object sender, MouseButtonEventArgs e) { /* Ta logique */ }
        private void Song4_Click(object sender, MouseButtonEventArgs e) { /* Ta logique */ }

        // Niveau Expert (en plus de Easy/Medium/Hard déjà existants)
        private void Expert_Click(object sender, RoutedEventArgs e) { /* Ta logique */ }
    }

}