using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Events
{
    public delegate void SnakeEvents();

    public enum Direction
    {
        Left, Right, Up, Down
    }

    class Coord
    {
        private int x;
        private int y;

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                y = value;
            }
        }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Show()
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine('\x0002');
        }
    }

    class Snake
    {
        private Direction direction;
        private List<Coord> coords;
        private Coord tail;
        private int maxX;
        private int maxY;
        private int appleX;
        private int appleY;
        public event SnakeEvents eatTail;
        public event SnakeEvents eatApple;
        public event SnakeEvents takeBorder;

        public Coord Tail
        {
            get
            {
                return tail;
            }
        }
        public int Length
        {
            get
            {
                return coords.Count;
            }
        }

        public bool Alive
        {
            get;
            set;
        }
        public int XHead
        {
            get
            {
                return coords[0].X;
            }
            private set
            {
                if (value < 0 || value > MaxX)
                    throw new ArgumentOutOfRangeException();
                coords[0].X = value;
            }
        }

        public int YHead
        {
            get
            {
                return coords[0].Y;
            }
            private set
            {
                if (value < 0 || value > MaxY)
                    throw new ArgumentOutOfRangeException();
                coords[0].Y = value;
            }
        }

        public int MaxX
        {
            get
            {
                return maxX;
            }
            set
            {
                if (value < 5)
                    throw new ArgumentOutOfRangeException();
                maxX = value;
            }
        }

        public int MaxY
        {
            get
            {
                return maxY;
            }
            set
            {
                if (value < 5)
                    throw new ArgumentOutOfRangeException();
                maxY = value;
            }
        }

        public int AppleX
        {
            get
            {
                return appleX;
            }
            set
            {
                if (value <= 0 || value >= MaxX - 1)
                    throw new ArgumentOutOfRangeException();
                appleX = value;
            }
        }

        public int AppleY
        {
            get
            {
                return appleY;
            }
            set
            {
                if (value <= 0 || value >= MaxY - 1)
                    throw new ArgumentOutOfRangeException();
                appleY = value;
            }
        }

        public Snake(int x, int y, int maxX, int maxY)
        {
            coords = new List<Coord>();
            coords.Add(new Coord(x, y));
            coords.Add(new Coord(x - 1, y));
            MaxX = maxX;
            MaxY = maxY;
            Alive = true;
            direction = Direction.Right;
            tail = coords[Length - 1];
        }
        public void Start()
        {
            Show();
            List<Coord> temp;
            while (Alive && Length != MaxX * MaxY / 10)
            {
                temp = new List<Coord>(coords);
                if (Console.KeyAvailable == false)
                {

                }
                else
                {
                    ConsoleKeyInfo cKey = Console.ReadKey();
                    if (cKey.Key == ConsoleKey.LeftArrow && direction != Direction.Right)
                    {
                        direction = Direction.Left;
                    }
                    else if (cKey.Key == ConsoleKey.RightArrow && direction != Direction.Left)
                    {
                        direction = Direction.Right;
                    }
                    else if (cKey.Key == ConsoleKey.UpArrow && direction != Direction.Down)
                    {
                        direction = Direction.Up;
                    }
                    else if (cKey.Key == ConsoleKey.DownArrow && direction != Direction.Up)
                    {
                        direction = Direction.Down;
                    }
                }
                tail = coords[Length - 1];
                Move(temp);
                Console.Clear();
                CheckPosition();
                Show();
                Thread.Sleep(250);
            }
            if (Alive)
            {
                Console.SetCursorPosition(MaxX + 5, MaxY / 2);
                Console.WriteLine("Вы выиграли!!");
            }
        }

        private void Move(List<Coord> temp)
        {
            switch (direction)
            {
                case Direction.Left:
                    coords[0] = new Coord(XHead - 1, YHead);
                    break;
                case Direction.Right:
                    coords[0] = new Coord(XHead + 1, YHead);
                    break;
                case Direction.Up:
                    coords[0] = new Coord(XHead, YHead - 1);
                    break;
                case Direction.Down:
                    coords[0] = new Coord(XHead, YHead + 1);
                    break;
            }
            for (int i = 1; i < Length; i++)
            {
                coords[i] = temp[i - 1];
            }
        }

        private void CheckPosition()
        {
            if (XHead == 0 || XHead == MaxX - 1 || YHead == 0 || YHead == MaxY - 1)
            {
                if (takeBorder != null)
                    takeBorder();
            }
            else if (XHead == AppleX && YHead == AppleY)
            {
                if (eatApple != null)
                    eatApple();
            }
            else if (InSnake())
            {
                if (eatTail != null)
                    eatTail();
            }
        }

        private bool InSnake()
        {
            for (int i = 1; i < coords.Count; i++)
            {
                if (XHead == coords[i].X && YHead == coords[i].Y)
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Coord el)
        {
            if (el != null)
                coords.Add(el);
        }

        public void Show()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < MaxY; i++)
            {
                for (int j = 0; j < MaxX; j++)
                {
                    if (i == 0 || i == MaxY - 1 || j == 0 || j == MaxX - 1)
                    {
                        Console.Write(Convert.ToChar(0x2593));
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                }
                Console.WriteLine();
            }
            Console.SetCursorPosition(AppleX, AppleY);
            Console.WriteLine('\x0024');
            foreach (var val in coords)
            {
                val.Show();
            }
        }
    }

    class Game
    {
        private Snake snake;

        public Game(int x, int y)
        {
            snake = new Snake(x / 2, y / 2, x, y);
            Random r = new Random();
            snake.AppleX = r.Next(1, x - 2);
            snake.AppleY = r.Next(1, y - 2);
            Console.SetCursorPosition(snake.AppleX, snake.AppleY);
            Console.Write('\x0024');
        }

        public void StartGame()
        {
            snake.Start();
        }

        public void EndGame()
        {
            snake.Alive = false;            
            Console.SetCursorPosition(snake.MaxX + 5, snake.MaxY / 2);
            Console.WriteLine("Вы проиграли!!");
        }

        public void AppleEated()
        {
            snake.Add(snake.Tail);
            Random r = new Random();
            snake.AppleX = r.Next(1, snake.MaxX - 2);
            snake.AppleY = r.Next(1, snake.MaxY - 2);
            Console.SetCursorPosition(snake.AppleX, snake.AppleY);
            Console.Write('\x0024');
        }
        static void Main(string[] args)
        {
            Game game = new Game(20, 15);
            game.snake.eatApple += game.AppleEated;
            game.snake.takeBorder += game.EndGame;
            game.snake.eatTail += game.EndGame;
            game.StartGame();
        }
    }
}
