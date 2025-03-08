namespace MouseIdle
{
    partial class MouseIdle
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MouseIdle));
            rbMoveRandom = new RadioButton();
            rbStayIdle = new RadioButton();
            nudInterval = new NumericUpDown();
            lbInterval = new Label();
            btnRun = new Button();
            btnExit = new Button();
            ((System.ComponentModel.ISupportInitialize)nudInterval).BeginInit();
            SuspendLayout();
            // 
            // rbMoveRandom
            // 
            rbMoveRandom.Location = new Point(12, 12);
            rbMoveRandom.Name = "rbMoveRandom";
            rbMoveRandom.Size = new Size(125, 25);
            rbMoveRandom.TabIndex = 0;
            rbMoveRandom.TabStop = true;
            rbMoveRandom.Text = "Move Random";
            rbMoveRandom.UseVisualStyleBackColor = true;
            // 
            // rbStayIdle
            // 
            rbStayIdle.Location = new Point(147, 12);
            rbStayIdle.Name = "rbStayIdle";
            rbStayIdle.Size = new Size(125, 25);
            rbStayIdle.TabIndex = 1;
            rbStayIdle.TabStop = true;
            rbStayIdle.Text = "Stay Like Idle";
            rbStayIdle.UseVisualStyleBackColor = true;
            // 
            // nudInterval
            // 
            nudInterval.Location = new Point(147, 50);
            nudInterval.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            nudInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            nudInterval.Name = "nudInterval";
            nudInterval.Size = new Size(125, 25);
            nudInterval.TabIndex = 2;
            nudInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            nudInterval.ValueChanged += NudInteral_ValueChange;
            // 
            // lbInterval
            // 
            lbInterval.Location = new Point(12, 50);
            lbInterval.Name = "lbInterval";
            lbInterval.Size = new Size(125, 25);
            lbInterval.TabIndex = 3;
            lbInterval.Text = "Interval Threshold";
            // 
            // btnRun
            // 
            btnRun.Location = new Point(12, 81);
            btnRun.Name = "btnRun";
            btnRun.Size = new Size(125, 25);
            btnRun.TabIndex = 4;
            btnRun.Text = "Start";
            btnRun.UseVisualStyleBackColor = true;
            btnRun.Click += BtnRun_Click;
            // 
            // btnExit
            // 
            btnExit.Location = new Point(147, 81);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(125, 25);
            btnExit.TabIndex = 5;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += BtnExit_Click;
            // 
            // MouseIdle
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(284, 111);
            Controls.Add(btnExit);
            Controls.Add(btnRun);
            Controls.Add(lbInterval);
            Controls.Add(nudInterval);
            Controls.Add(rbStayIdle);
            Controls.Add(rbMoveRandom);
            Font = new Font("Palatino Linotype", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "MouseIdle";
            RightToLeft = RightToLeft.No;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Mouse Idle";
            ((System.ComponentModel.ISupportInitialize)nudInterval).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private RadioButton rbMoveRandom;
        private RadioButton rbStayIdle;
        private NumericUpDown nudInterval;
        private Label lbInterval;
        private Button btnRun;
        private Button btnExit;
    }
}
