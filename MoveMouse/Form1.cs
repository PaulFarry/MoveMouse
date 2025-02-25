using System.ComponentModel;
using System.Runtime.InteropServices;

namespace MoveMouse
{
    public partial class Form1 : Form
    {


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

        public Form1()
        {
            InitializeComponent();
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += SystemEvents_DisplaySettingsChanged;
            random = Random.Shared;

            XInc = GetRandom();
            YInc = GetRandom();
            Timer = new System.Windows.Forms.Timer
            {
                Interval = 50
            };
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= SystemEvents_DisplaySettingsChanged;
            Timer.Stop();

            base.OnClosing(e);
        }

        private int GetRandom()
        {
            return random.Next(1, 20);
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            GetCursorPos(out var position);
            if (hasStarted && position.X != CurrentX && position.Y != CurrentY)
            {
                Timer.Stop();
            }


            CurrentX += XInc;
            CurrentY += YInc;
            if (CurrentX > XMax)
            {
                CurrentX = XMax;
                XInc = -GetRandom();
            }
            if (CurrentX < XMin)
            {
                CurrentX = XMin;
                XInc = GetRandom();
            }
            if (CurrentY > YMax)
            {
                CurrentY = YMax;
                YInc = -GetRandom();
            }
            if (CurrentY < YMin)
            {
                CurrentY = YMin;
                YInc = GetRandom();
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

            SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));

            SetCursorPos(CurrentX, CurrentY);

        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            UpdateScreen();
            CurrentX = ((XMax - XMin) / 2) + XMin;
            CurrentY = ((YMax - YMin) / 2) + YMin;
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
            if (e.Control && e.Alt && e.Shift)
            {
                Application.Exit();
            }
            base.OnKeyDown(e);
        }
    }
}
