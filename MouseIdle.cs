using Timer = System.Windows.Forms.Timer;

namespace MouseIdle
{
    public partial class MouseIdle : Form
    {
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

        public MouseIdle()
        {
            InitializeComponent();
            CustomComponent();
            IdleLogic();
        }

        private void CustomComponent()
        {
            // Init tray notify icon
            notifyIcon = new NotifyIcon()
            {
                Icon = new Icon("TitleIcon.ico"),
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

        private void BtnRun_Click(object sender, EventArgs e)
        {
            if (isStopped)
            {
                isStopped = false;
                timer.Start();
                btnRun.Text = "Stop";
            }
            else
            {
                isStopped = true;
                timer.Stop();
                btnRun.Text = "Start";
            }
        }

        private void NudInteral_ValueChange(object sender, EventArgs e)
        {
            timeCounter = (int)nudInterval.Value;
        }

        private void NotifyIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void OpenMenuItem_Click(object? sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            Application.Exit();
        }

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
            notifyIcon.Dispose();
            base.OnFormClosing(e);
        }
    }
}
