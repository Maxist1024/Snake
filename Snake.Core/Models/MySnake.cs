using System.Collections.Generic;
using System.Windows.Controls;

namespace Snake.Core.Models
{
    public class MySnake
    {
        public SnakePart Head { get; private set; }
        public List<SnakePart> Parts { get; private set; }

        public MySnake()
        {
            Head = new SnakePart(20, 0, 15);
            Head.Rect.Width = Head.Rect.Height = 10;
            Head.Rect.Fill = System.Windows.Media.Brushes.Red;
            Parts = new List<SnakePart>();
            Parts.Add(new SnakePart(19, 0));
            Parts.Add(new SnakePart(18, 0));
            Parts.Add(new SnakePart(17, 0));
            Parts.Add(new SnakePart(16, 0));
            Parts.Add(new SnakePart(15, 0));
            Parts.Add(new SnakePart(14, 0));
            Parts.Add(new SnakePart(13, 0));
            Parts.Add(new SnakePart(12, 0));
            Parts.Add(new SnakePart(11, 0));
            Parts.Add(new SnakePart(10, 0));
        }

        public void RedrawSnake()
        {
            Grid.SetColumn(Head.Rect, Head.X);
            Grid.SetRow(Head.Rect, Head.Y);
            foreach (SnakePart snakePart in Parts)
            {
                Grid.SetColumn(snakePart.Rect, snakePart.X);
                Grid.SetRow(snakePart.Rect, snakePart.Y);
            }
        }
    }
}
