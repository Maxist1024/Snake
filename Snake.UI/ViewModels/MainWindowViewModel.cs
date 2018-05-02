using Snake.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Snake.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties

        #region Private
        private MySnake _snake;
        private int _directionX;
        private int _directionY;
        private SnakePart _food;


        #endregion //Private properties

        #region Private
        public SnakePart Food
        {
            get { return _food; }
            set { _food = value; }
        }
        public int DirectionY
        {
            get { return _directionY; }
            set { _directionY = value; }
        }
        public int DirectionX
        {
            get { return _directionX; }
            set { _directionX = value; }
        }
        public Grid MainGrid { get; set; }
        public MySnake Snake
        {
            get { return _snake; }
            set { _snake = value; }
        }
        #endregion //Public properties

        #endregion //Properties

        #region Fields

        #region Private fields

        private int _partsToAdd;

        #endregion //Private fields

        #region Public fields

        public DispatcherTimer dispatcherTimer;

        #endregion //Public fields

        #endregion //Fields

        #region Constructors
        public MainWindowViewModel()
        {
            Snake = new MySnake();
            InitTimer();
        }

        #endregion //Constructors

        #region Private methods
   
        private bool CheckCollision()
        {
            if (CheckBoardCollision())
                return true;
           // if (CheckItselfCollision())
           //     return true;
            return false;
        }
        private bool CheckBoardCollision(int SIZE = 10)
        {
            if (Snake.Head.X < 0 || Snake.Head.X > MainGrid.Width / SIZE)
                return true;
            if (Snake.Head.Y < 0 || Snake.Head.Y > MainGrid.Height / SIZE)
                return true;
            return false;
        }
        private bool CheckFood(int SIZE = 10)
        {
            Random rand = new Random();
            if (Snake.Head.X == Food.X && Snake.Head.Y == Food.Y)
            {
                _partsToAdd += 20;
                for (int i = 0; i < 20; i++)
                {
                    int x = rand.Next(0, (int)(MainGrid.Width / SIZE));
                    int y = rand.Next(0, (int)(MainGrid.Height / SIZE));
                    if (IsFieldFree(x, y))
                    {
                        Food.X = x;
                        Food.Y = y;
                        return true;
                    }
                }
                for (int i = 0; i < MainGrid.Width / SIZE; i++)
                    for (int j = 0; j < MainGrid.Height / SIZE; j++)
                    {
                        if (IsFieldFree(i, j))
                        {
                            Food.X = i;
                            Food.Y = j;
                            return true;
                        }
                    }
                EndGame();
            }
            return false;
        }
        private bool CheckItselfCollision()
        {
            foreach (SnakePart snakePart in Snake.Parts)
            {
                if (Snake.Head.X == snakePart.X && Snake.Head.Y == snakePart.Y)
                    return true;
            }
            return false;
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        #endregion //Private methods

        #region Public methods

        public void InitSnake()
        {
            MainGrid.Children.Add(Snake.Head.Rect);
            foreach (SnakePart snakePart in Snake.Parts)
                MainGrid.Children.Add(snakePart.Rect);
            Snake.RedrawSnake();
        }
        public void MoveSnake()
        {
            int snakePartCount = Snake.Parts.Count;
            if (_partsToAdd > 0)
            {
                SnakePart newPart = new SnakePart(Snake.Parts[Snake.Parts.Count - 1].X,
                Snake.Parts[Snake.Parts.Count - 1].Y);
                MainGrid.Children.Add(newPart.Rect);
                Snake.Parts.Add(newPart);
                _partsToAdd--;
            }
            for (int i = snakePartCount - 1; i >= 1; i--)
            {
                Snake.Parts[i].X = Snake.Parts[i - 1].X;
                Snake.Parts[i].Y = Snake.Parts[i - 1].Y;
            }
            Snake.Parts[0].X = Snake.Head.X;
            Snake.Parts[0].Y = Snake.Head.Y;
            Snake.Head.X += DirectionX;
            Snake.Head.Y += DirectionY;
            if (CheckCollision())
                EndGame();
            else
            {
                if (CheckFood())
                    RedrawFood();
                Snake.RedrawSnake();
            }
        }
        public void InitFood()
        {
            Food = new SnakePart(10, 10);
            Food.Rect.Width = Food.Rect.Height = 10;
            Food.Rect.Fill = Brushes.Blue;
            MainGrid.Children.Add(Food.Rect);
            Grid.SetColumn(Food.Rect, Food.X);
            Grid.SetRow(Food.Rect, Food.Y);
        }
        public bool IsFieldFree(int x, int y)
        {
            if (Snake.Head.X == x && Snake.Head.Y == y)
                return false;
            foreach (SnakePart snakePart in Snake.Parts)
            {
                if (snakePart.X == x && snakePart.Y == y)
                    return false;
            }
            return true;
        }
        public void EndGame()
        {
            dispatcherTimer.Stop();
            MessageBox.Show("KONIEC GRY");
        }
        public void RedrawFood()
        {
            Grid.SetColumn(Food.Rect, Food.X);
            Grid.SetRow(Food.Rect, Food.Y);
        }
        public void InitTimer()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50);
        }

        #endregion //Public methods
    }
}
