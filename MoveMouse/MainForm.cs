using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MoveMouse
{
    public partial class MainForm : Form
    {

        POINT initialPosition;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("User32.dll")]
        public static extern bool SetCursorPos(int x, int y);
        private int YMin;
        private int YMax;

        private int XMin;
        private int XMax;

        private int CurrentX;
        private int CurrentY;
        private Random random;
        private int XInc = 1;
        private int YInc = 1;
        private bool hasStarted;

        private System.Windows.Forms.Timer Timer;

        private int currentImage = 0;

        private List<Image> images;
        DateTime nextChangeTime;

        public MainForm()
        {
            InitializeComponent();
            GetCursorPos(out initialPosition);
            images = [];
            GenerateAnimationImages();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            random = Random.Shared;

            XInc = GetRandomIncrement();
            YInc = GetRandomIncrement();
            Timer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };
            Timer.Tick += Timer_Tick;

            nextChangeTime = DateTime.Now;

            Timer.Start();
        }

        private void GenerateAnimationImages()
        {
            var assembly = typeof(Program).Assembly;

            using var st = assembly.GetManifestResourceStream("MoveMouse.Homer.base.png")!;

            using var baseImage = Image.FromStream(st);
            var blankFrame = new Bitmap(baseImage.Width, baseImage.Height);
            using (var graphics = Graphics.FromImage(blankFrame))
            {
                graphics.DrawImage(baseImage, 0, 0);
            }
            images.Add(blankFrame);

            for (var i = 1; i <= 8; i++)
            {
                var frameName = $"MoveMouse.Homer.frame{i:00}.png";
                using var frameImage = assembly.GetManifestResourceStream(frameName)!;
                using var image = Image.FromStream(frameImage);

                var newImage = new Bitmap(baseImage.Width, baseImage.Height);
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.DrawImage(baseImage, 0, 0);
                    graphics.DrawImage(image, 0, 0);
                }
                images.Add(newImage);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            CloseApplication();
        }

        private int GetRandomIncrement()
        {
            return random.Next(1, 20);
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            GetCursorPos(out var position);
            if (hasStarted && position.X != CurrentX && position.Y != CurrentY)
            {
                CloseApplication();
            }

            pictureOverlay.Image = images[currentImage];
            currentImage++;
            if (currentImage >= images.Count)
            {
                currentImage = 0;
            }


            if (DateTime.Now > nextChangeTime)
            {
                var nextChange = TimeSpan.FromMilliseconds(random.Next(2000, 5000));
                nextChangeTime = DateTime.Now.Add(nextChange);

                var xPositive = random.Next() >= 0.5;
                var yPositive = random.Next() >= 0.5;
                var newX = GetRandomIncrement();
                var newY = GetRandomIncrement();
                if (!xPositive) newX = -newX;
                if (!yPositive) newY = -newY;

                XInc = newX;
                YInc = newY;
            }


            CurrentX += XInc;
            CurrentY += YInc;

            if (CurrentX > XMax)
            {
                CurrentX = XMax;
                XInc = -GetRandomIncrement();
            }
            if (CurrentX < XMin)
            {
                CurrentX = XMin;
                XInc = GetRandomIncrement();
            }
            if (CurrentY > YMax)
            {
                CurrentY = YMax;
                YInc = -GetRandomIncrement();
            }
            if (CurrentY < YMin)
            {
                CurrentY = YMin;
                YInc = GetRandomIncrement();
            }

            MoveMouse();

            hasStarted = true;
        }

        private void MoveMouse()
        {
            Input[] inputs =
            [
                new Input
                {
                    Type = (int) InputType.Mouse,
                    Union = new InputUnion
                    {
                        mi = new MouseInput
                        {
                            dx = XInc,
                            dy = YInc,
                            dwFlags = (uint)(MouseEventF.Move),
                            dwExtraInfo = GetMessageExtraInfo()
                        }
                    }
                }
            ];

            //Don't care for now
            _ = SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));

            SetCursorPos(CurrentX, CurrentY);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateScreen();
            SetWindowStartPosition();
            CurrentX = initialPosition.X;
            CurrentY = initialPosition.Y;
        }

        private void SetWindowStartPosition()
        {
            var newMinY = int.MaxValue;
            var newMaxY = int.MinValue;
            var newMinX = int.MaxValue;
            var newMaxX = int.MinValue;
            foreach (var s in Screen.AllScreens.Select(x => x.WorkingArea))
            {
                newMinY = Math.Min(newMinY, s.Top);
                newMaxY = Math.Max(newMaxY, s.Bottom);
                newMinX = Math.Min(newMinX, s.Left);
                newMaxX = Math.Max(newMaxX, s.Right);
            }

            var x = random.Next(newMinX, newMaxX - this.Width);
            var y = random.Next(newMinY, newMaxY - this.Height);

            this.Location = new Point(x, y);
        }

        private void SystemEvents_DisplaySettingsChanged(object? sender, EventArgs e)
        {
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            var newMinY = int.MaxValue;
            var newMaxY = int.MinValue;
            var newMinX = int.MaxValue;
            var newMaxX = int.MinValue;

            foreach (var s in Screen.AllScreens.Select(x => x.Bounds))
            {
                newMinY = Math.Min(newMinY, s.Top);
                newMaxY = Math.Max(newMaxY, s.Bottom);
                newMinX = Math.Min(newMinX, s.Left);
                newMaxX = Math.Max(newMaxX, s.Right);
            }

            YMin = Math.Min(YMin, newMinY);
            YMax = Math.Max(YMax, newMaxY);
            XMin = Math.Min(XMin, newMinX);
            XMax = Math.Max(XMax, newMaxX);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Control && e.Alt && e.Shift)
            {
                CloseApplication();
            }
        }

        private void CloseApplication()
        {
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            Timer.Tick -= Timer_Tick;
            Timer.Stop();
            SetCursorPos(initialPosition.X, initialPosition.Y);
            Application.Exit();
        }
    }
}
