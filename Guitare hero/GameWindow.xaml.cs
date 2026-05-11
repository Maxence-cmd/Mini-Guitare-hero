using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Media;

namespace Guitare_hero
{
    public partial class GameWindow : Window
    {
        DispatcherTimer gameTimer = new();
        DateTime lastFrameTime = DateTime.Now;
        DispatcherTimer spawnTimer = new();
        Random random = new();

        List<Note> notes = new();

        int score = 0;
        double speed = 5;
        int laneCount = 5;

        double[] lanes = { 130, 260, 390, 520, 650 };
        Key[] keys = { Key.D, Key.F, Key.G, Key.J, Key.K };

        Brush[] colors =
        {
            Brushes.Red,
            Brushes.Yellow,
            Brushes.Green,
            Brushes.Blue,
            Brushes.Purple
        };

        double hitLineY = 680;
        string difficulty;

        public GameWindow(string selectedDifficulty)
        {
            InitializeComponent();
            difficulty = selectedDifficulty;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ApplyDifficulty(difficulty);
            DrawLanes();

            CompositionTarget.Rendering += GameLoop;

            spawnTimer.Tick += SpawnNote;
            spawnTimer.Start();

            Focus();
        }

        void ApplyDifficulty(string difficulty)
        {
            if (difficulty == "Facile")
            {
                speed = 4;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(1000);
            }
            else if (difficulty == "Moyen")
            {
                speed = 6;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(700);
            }
            else if (difficulty == "Difficile")
            {
                speed = 9;
                spawnTimer.Interval = TimeSpan.FromMilliseconds(400);
            }
        }

        void DrawLanes()
        {
            for (int i = 0; i < laneCount; i++)
            {
                Rectangle lane = new()
                {
                    Width = 90,
                    Height = 850,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 2
                };

                Canvas.SetLeft(lane, lanes[i] - 45);
                Canvas.SetTop(lane, 0);
                GameCanvas.Children.Add(lane);

                Rectangle hitZone = new()
                {
                    Width = 90,
                    Height = 25,
                    Fill = colors[i]
                };

                Canvas.SetLeft(hitZone, lanes[i] - 45);
                Canvas.SetTop(hitZone, hitLineY);
                GameCanvas.Children.Add(hitZone);
            }
        }

        void SpawnNote(object? sender, EventArgs e)
        {
            int lane = random.Next(0, laneCount);

            Ellipse ellipse = new()
            {
                Width = 55,
                Height = 55,
                Fill = colors[lane]
            };

            Canvas.SetLeft(ellipse, lanes[lane] - 27);
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

            for (int i = notes.Count - 1; i >= 0; i--)
            {
                notes[i].Y += speed * 60 * deltaTime;
                Canvas.SetTop(notes[i].Shape, notes[i].Y);

                if (notes[i].Y > 850)
                {
                    GameCanvas.Children.Remove(notes[i].Shape);
                    notes.RemoveAt(i);

                    score -= 50;
                    ScoreText.Text = $"Score : {score}";
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
                    ScoreText.Text = $"Score : {score}";

                    GameCanvas.Children.Remove(note.Shape);
                    notes.RemoveAt(i);
                    return;
                }
            }

            score -= 25;
            ScoreText.Text = $"Score : {score}";
        }
    }

    public class Note
    {
        public Ellipse Shape { get; set; } = null!;
        public int Lane { get; set; }
        public double Y { get; set; }
    }
}