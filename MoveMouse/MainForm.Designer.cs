namespace MoveMouse
{
    partial class MainForm
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
            pictureOverlay = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureOverlay).BeginInit();
            SuspendLayout();
            // 
            // pictureOverlay
            // 
            pictureOverlay.BackColor = Color.Transparent;
            pictureOverlay.Dock = DockStyle.Fill;
            pictureOverlay.Location = new Point(0, 0);
            pictureOverlay.Name = "pictureOverlay";
            pictureOverlay.Size = new Size(368, 249);
            pictureOverlay.TabIndex = 1;
            pictureOverlay.TabStop = false;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 249);
            Controls.Add(pictureOverlay);
            Name = "MainForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.Manual;
            Text = "Auto Mouse";
            ((System.ComponentModel.ISupportInitialize)pictureOverlay).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private PictureBox pictureOverlay;
    }
}
