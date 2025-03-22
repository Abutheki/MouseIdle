using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace MouseIdle
{
    public partial class MouseIdle : Form
    {
        [DllImport("kernel32.dll")]
        private static extern uint SetThreadExecutionState(uint esFlags);
        private const uint ES_CONTINUOUS = 0x80000000;
        private const uint ES_SYSTEM_REQUIRED = 0x00000001;
        private const uint ES_DISPLAY_REQUIRED = 0x00000002;

        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripMenuItem openMenuItem;
        private Timer timer;
        private Random random;
        private int screenWidth;
        private int screenHeight;
        private int timeCounter = 30;
        private bool isStopped = true;
        private Point lastCursor;
        private Icon titleIcon;

        public MouseIdle()
        {
            InitializeComponent();
            CustomComponent();
            IdleLogic();
        }

        #region Window Initialization
        private void CustomComponent()
        {
            using(MemoryStream ms = new MemoryStream(Properties.Resources.TitleIcon))
            {
                titleIcon = new Icon(ms);
            }
            notifyIcon = new NotifyIcon()
            {
                Icon = titleIcon,
                Text = "Mouse Idle",
                Visible = true
            };
            contextMenuStrip = new ContextMenuStrip();
            // Add item to context menu
            openMenuItem = new ToolStripMenuItem("Open");
            exitMenuItem = new ToolStripMenuItem("Exit");
            // Create context menu item click event
            openMenuItem.Click += OpenMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            // Finallize tray notify icon
            contextMenuStrip.Items.Add(openMenuItem);
            contextMenuStrip.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            // Trau notify icon double click
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
        }
        #endregion

        #region Main Logic
        private void IdleLogic()
        {
            random = new Random();
            if (Screen.PrimaryScreen != null)
            {
                screenWidth = Screen.PrimaryScreen.Bounds.Width;
                screenHeight = Screen.PrimaryScreen.Bounds.Height;
            }
            else
            {
                screenWidth = 800;
                screenHeight = 400;
            }
            timeCounter = (int)nudInterval.Value;
            timer = new Timer
            {
                Interval = 1000
            };
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (Cursor.Position.X == lastCursor.X && Cursor.Position.Y == lastCursor.Y)
            {
                --timeCounter;
            }
            else
            {
                timeCounter = (int)nudInterval.Value;
                lastCursor.X = Cursor.Position.X;
                lastCursor.Y = Cursor.Position.Y;
            }
            if (timeCounter < 0)
            {
                _ = MoveMouse();
                notifyIcon.ShowBalloonTip(1000, "Mouse Idle", "Mouse moves due to inactivity.", ToolTipIcon.Info);
            }
        }

        private async Task MoveMouse()
        {
            if (rbMoveRandom.Checked)
            {
                Cursor.Position = new Point(random.Next(screenHeight), random.Next(screenWidth));
            }
            if (rbStayIdle.Checked)
            {
                Cursor.Position = new Point(random.Next(screenHeight), random.Next(screenWidth));
                await Task.Delay(150);
                Cursor.Position = lastCursor;
                timeCounter = (int)nudInterval.Value;
            }
        }

        private void NudInteral_ValueChange(object sender, EventArgs e)
        {
            timeCounter = (int)nudInterval.Value;
        }

        #endregion

        #region Window Button
        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (isStopped)
            {
                isStopped = false;
                timer.Start();
                SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
                btnRun.Text = "Stop";
            }
            else
            {
                isStopped = true;
                timer.Stop();
                SetThreadExecutionState(ES_CONTINUOUS);
                btnRun.Text = "Start";
            }
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            ExitApp();
        }
        #endregion

        #region Tray Icon
        private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            ShowFormWindow();
        }

        private void OpenMenuItem_Click(object? sender, EventArgs e)
        {
            ShowFormWindow();
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            ExitApp();
        }
        #endregion

        #region Base Form Event Override
        protected override void OnResize(EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon.ShowBalloonTip(1000, "Mouse Idle", "Application minimized to tray.", ToolTipIcon.Info);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SetThreadExecutionState(ES_CONTINUOUS);
            notifyIcon.Dispose();
            base.OnFormClosing(e);
        }
        #endregion

        #region All Form Activities
        /// <summary>
        /// Show App Window
        /// </summary>
        private void ShowFormWindow()
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }
        /// <summary>
        /// Exit App
        /// </summary>
        private void ExitApp()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
            notifyIcon.Dispose();
            Application.Exit();
        }
        #endregion
    }
}
