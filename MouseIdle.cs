using MouseIdle.Properties;
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
        private ToolStripMenuItem autoRunMenuItem;
        private ToolStripMenuItem minimizeMenuItem;
        private Timer timer;
        private Random random;
        private int screenWidth;
        private int screenHeight;
        private int timeCounter = 30;
        private bool isStopped = true;
        private Point lastCursor;
        private Icon titleIcon;
        private const string filePath = "MouseIdle.dll.config";

        public MouseIdle()
        {
            InitializeComponent();
            CustomComponent();
            IdleLogic();
            StartupCheck();
        }

        #region Window Initialization
        private void CustomComponent()
        {
            try
            {
                byte[] fileData = Convert.FromBase64String(Properties.Resources.ConfigFile);

                File.WriteAllBytes(filePath, fileData);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating config file: " + ex.Message);
            }
            using (MemoryStream ms = new MemoryStream(Properties.Resources.TitleIcon))
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
            openMenuItem = new ToolStripMenuItem("Open", null, OpenMenuItem_Click);
            autoRunMenuItem = new ToolStripMenuItem("Auto Run", null, autoRunMenuItem_Click);
            minimizeMenuItem = new ToolStripMenuItem("Minimize On Startup", null, minimizeMenuItem_Click);
            exitMenuItem = new ToolStripMenuItem("Exit", null, ExitMenuItem_Click);
            // Finallize tray notify icon
            contextMenuStrip.Items.Add(openMenuItem);
            contextMenuStrip.Items.Add(autoRunMenuItem);
            contextMenuStrip.Items.Add(minimizeMenuItem);
            contextMenuStrip.Items.Add(exitMenuItem);
            notifyIcon.ContextMenuStrip = contextMenuStrip;
            // Trau notify icon double click
            notifyIcon.MouseDoubleClick += NotifyIcon_MouseDoubleClick;
            rbMoveRandom.Checked = Properties.Setting.Default.IsRandom;
            rbStayIdle.Checked = !Properties.Setting.Default.IsRandom;
            nudInterval.Value = Properties.Setting.Default.Interval;
            autoRunMenuItem.Checked = Properties.Setting.Default.IsAutoStart;
            minimizeMenuItem.Checked = Properties.Setting.Default.IsMinimize;
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
                notifyIcon.ShowBalloonTip(200, "Mouse Idle", "Mouse moves due to inactivity.", ToolTipIcon.Info);
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

        private void StartupCheck()
        {
            if (Properties.Setting.Default.IsAutoStart)
            {
                isStopped = false;
                timer.Start();
                SetThreadExecutionState(ES_CONTINUOUS | ES_SYSTEM_REQUIRED | ES_DISPLAY_REQUIRED);
                btnRun.Text = "Stop";
            }
            if (Properties.Setting.Default.IsMinimize)
            {
                this.Load += (sender, e) => { this.WindowState = FormWindowState.Minimized; this.Hide(); this.ShowInTaskbar = false; };
            }
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

        private void autoRunMenuItem_Click(object? sender, EventArgs e)
        {
            autoRunMenuItem.Checked = !autoRunMenuItem.Checked;
        }

        private void minimizeMenuItem_Click(object? sender, EventArgs e)
        {
            minimizeMenuItem.Checked = !minimizeMenuItem.Checked;
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
                this.ShowInTaskbar = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SetThreadExecutionState(ES_CONTINUOUS);
            Properties.Setting.Default.Interval = timeCounter;
            Properties.Setting.Default.IsRandom = rbMoveRandom.Checked;
            Properties.Setting.Default.IsAutoStart = autoRunMenuItem.Checked;
            Properties.Setting.Default.IsMinimize = minimizeMenuItem.Checked;
            Properties.Setting.Default.Save();
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating config file: " + ex.Message);
            }
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
            this.ShowInTaskbar = true;
        }
        /// <summary>
        /// Exit App
        /// </summary>
        private void ExitApp()
        {
            SetThreadExecutionState(ES_CONTINUOUS);
            Properties.Setting.Default.Interval = timeCounter;
            Properties.Setting.Default.IsRandom = rbMoveRandom.Checked;
            Properties.Setting.Default.IsAutoStart = autoRunMenuItem.Checked;
            Properties.Setting.Default.IsMinimize = minimizeMenuItem.Checked;
            Properties.Setting.Default.Save();
            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating config file: " + ex.Message);
            }
            notifyIcon.Dispose();
            Application.Exit();
        }
        #endregion
    }
}
