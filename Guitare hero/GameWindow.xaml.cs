using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Guitare_hero
{
    public partial class GameWindow : Window
    {
        DispatcherTimer spawnTimer = new();
        Random random = new();

        List<Note> notes = new();
        private MediaPlayer player = new MediaPlayer();
        private string musicPath;
        int score = 0;
        double speed = 5;
        int laneCount = 5;

        // ── Positions des couloirs adaptées au Canvas de 480px centré ──
        // Canvas centré sur 480px → couloirs espacés de 96px, départ à 48
        double[] lanes = { 48, 144, 240, 336, 432 };

        // ── Touches corrigées pour correspondre aux boutons XAML (A Z E R T) ──
        Key[] keys = { Key.A, Key.Z, Key.E, Key.R, Key.T };

        // ── Couleurs harmonisées avec le thème néon du XAML ──
        Brush[] colors =
        {
            new SolidColorBrush(Color.FromRgb(0,   255, 136)), // Vert
            new SolidColorBrush(Color.FromRgb(255,  45, 120)), // Rouge/Rose
            new SolidColorBrush(Color.FromRgb(255, 230,   0)), // Jaune
            new SolidColorBrush(Color.FromRgb(0,   212, 255)), // Bleu
            new SolidColorBrush(Color.FromRgb(255, 140,   0))  // Orange
        };

        // ── hitLineY adapté à la nouvelle hauteur de la zone de jeu ──
        // Fenêtre 850 - barre titre 52 - zone touches 110 = 688 de zone jeu
        // On place la ligne à 90% de la zone de jeu
        double hitLineY = 580;
        DateTime lastFrameTime = DateTime.Now;
        string difficulty;

        public GameWindow(string selectedDifficulty, string song)
        {
            InitializeComponent();

            difficulty = selectedDifficulty;
            musicPath = song;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyDifficulty(difficulty);
            DrawLanes();
            if (!string.IsNullOrEmpty(musicPath))
            {
                player.Open(new Uri(musicPath, UriKind.Relative));
                player.Play();
            }
            // ── CompositionTarget.Rendering pour la boucle de jeu (inchangé) ──
            CompositionTarget.Rendering += GameLoop;

            spawnTimer.Tick += SpawnNote;
            spawnTimer.Start();

            Focus();
        }

        void ApplyDifficulty(string diff)
        {
            if (diff == "Facile")
            {
                speed = 4;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(1000);
            }
            else if (diff == "Moyen")
            {
                speed = 6;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(700);
            }
            else if (diff == "Difficile")
            {
                speed = 9;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(400);
            }
            else if (diff == "Expert")
            {
                speed = 13;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(250);
            }
        }

        void DrawLanes()
        {
            double laneWidth = 480.0 / laneCount; // 96px par couloir

            for (int i = 0; i < laneCount; i++)
            {
                // ── Séparateurs entre couloirs ──
                if (i > 0)
                {
                    Rectangle sep = new()
                    {
                        Width = 1,
                        Height = GameCanvas.ActualHeight > 0 ? GameCanvas.ActualHeight : 700,
                        Fill = new SolidColorBrush(Color.FromArgb(80, 100, 100, 180))
                    };
                    Canvas.SetLeft(sep, laneWidth * i);
                    Canvas.SetTop(sep, 0);
                    GameCanvas.Children.Add(sep);
                }

                // ── Zone de frappe en bas de chaque couloir ──
                Rectangle hitZone = new()
                {
                    Width = laneWidth - 4,
                    Height = 20,
                    Fill = colors[i],
                    Opacity = 0.4,
                    RadiusX = 6,
                    RadiusY = 6
                };

                Canvas.SetLeft(hitZone, laneWidth * i + 2);
                Canvas.SetTop(hitZone, hitLineY);
                GameCanvas.Children.Add(hitZone);
            }
        }

        void SpawnNote(object? sender, EventArgs e)
        {
            int lane = random.Next(0, laneCount);
            double laneWidth = 480.0 / laneCount;
            double noteSize = 55;

            Ellipse ellipse = new()
            {
                Width = noteSize,
                Height = noteSize,
                Fill = colors[lane],
                // ── Effet de halo sur les notes ──
                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = ((SolidColorBrush)colors[lane]).Color,
                    BlurRadius = 16,
                    ShadowDepth = 0,
                    Opacity = 0.9
                }
            };

            double x = laneWidth * lane + (laneWidth - noteSize) / 2;
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, -60);

            GameCanvas.Children.Add(ellipse);

            notes.Add(new Note
            {
                Shape = ellipse,
                Lane = lane,
                Y = -60
            });
        }

        void GameLoop(object? sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            double deltaTime = (now - lastFrameTime).TotalSeconds;
            lastFrameTime = now;

            // ── Clamp deltaTime pour éviter les sauts si la fenêtre est gelée ──
            if (deltaTime > 0.1) deltaTime = 0.1;

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                notes[i].Y += speed * 60 * deltaTime;
                Canvas.SetTop(notes[i].Shape, notes[i].Y);

                // ── Hauteur de sortie adaptée à la zone de jeu ──
                if (notes[i].Y > hitLineY + 80)
                {
                    GameCanvas.Children.Remove(notes[i].Shape);
                    notes.RemoveAt(i);

                    score -= 50;
                    if (score < 0) score = 0;
                    ScoreText.Text = $"{score}";
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            for (int lane = 0; lane < laneCount; lane++)
            {
                if (e.Key == keys[lane])
                {
                    CheckHit(lane);
                    break;
                }
            }
        }

        void CheckHit(int lane)
        {
            for (int i = notes.Count - 1; i >= 0; i--)
            {
                Note note = notes[i];

                if (note.Lane == lane &&
                    note.Y > hitLineY - 55 &&
                    note.Y < hitLineY + 45)
                {
                    score += 100;
                    ScoreText.Text = $"{score}";

                    GameCanvas.Children.Remove(note.Shape);
                    notes.RemoveAt(i);
                    return;
                }
            }

            // Mauvaise touche
            score -= 25;
            if (score < 0) score = 0;
            ScoreText.Text = $"{score}";
        }

        // ── Handlers fenêtre ──
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class Note
    {
        public Ellipse Shape { get; set; } = null!;
        public int Lane { get; set; }
        public double Y { get; set; }
    }
}
