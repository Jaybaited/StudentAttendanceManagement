using System;
using System.Drawing;
using System.Drawing.Drawing2D; // Added for GraphicsPath
using System.Windows.Forms; // Added to resolve Panel and PictureBox errors

namespace StudentAttendanceManagement
{
    partial class LoginPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginPage));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pbShowPass = new System.Windows.Forms.PictureBox();
            this.panelPasswordUnderline = new System.Windows.Forms.Panel();
            this.labelPassword = new System.Windows.Forms.Label();
            this.panelUsernameUnderline = new System.Windows.Forms.Panel();
            this.labelUsername = new System.Windows.Forms.Label();
            this.btnLogin = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pbClose = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShowPass)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(70)))), ((int)(((byte)(200)))));
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(38, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(893, 584);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panel2.Controls.Add(this.pbShowPass);
            this.panel2.Controls.Add(this.panelPasswordUnderline);
            this.panel2.Controls.Add(this.labelPassword);
            this.panel2.Controls.Add(this.panelUsernameUnderline);
            this.panel2.Controls.Add(this.labelUsername);
            this.panel2.Controls.Add(this.btnLogin);
            this.panel2.Controls.Add(this.textBoxPassword);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.textBoxUsername);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(359, 584);
            this.panel2.TabIndex = 6;
            // 
            // pbShowPass
            // 
            this.pbShowPass.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbShowPass.Image = ((System.Drawing.Image)(resources.GetObject("pbShowPass.Image")));
            this.pbShowPass.Location = new System.Drawing.Point(293, 312);
            this.pbShowPass.Name = "pbShowPass";
            this.pbShowPass.Size = new System.Drawing.Size(32, 31);
            this.pbShowPass.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbShowPass.TabIndex = 11;
            this.pbShowPass.TabStop = false;
            this.pbShowPass.Click += new System.EventHandler(this.pbShowPass_Click_1);
            // 
            // panelPasswordUnderline
            // 
            this.panelPasswordUnderline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.panelPasswordUnderline.Location = new System.Drawing.Point(40, 345);
            this.panelPasswordUnderline.Name = "panelPasswordUnderline";
            this.panelPasswordUnderline.Size = new System.Drawing.Size(286, 1);
            this.panelPasswordUnderline.TabIndex = 10;
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Font = new System.Drawing.Font("Tw Cen MT", 9.75F);
            this.labelPassword.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelPassword.Location = new System.Drawing.Point(40, 300);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 15);
            this.labelPassword.TabIndex = 9;
            this.labelPassword.Text = "Password";
            // 
            // panelUsernameUnderline
            // 
            this.panelUsernameUnderline.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.panelUsernameUnderline.Location = new System.Drawing.Point(40, 275);
            this.panelUsernameUnderline.Name = "panelUsernameUnderline";
            this.panelUsernameUnderline.Size = new System.Drawing.Size(286, 1);
            this.panelUsernameUnderline.TabIndex = 8;
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Font = new System.Drawing.Font("Tw Cen MT", 9.75F);
            this.labelUsername.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.labelUsername.Location = new System.Drawing.Point(40, 230);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(59, 15);
            this.labelUsername.TabIndex = 7;
            this.labelUsername.Text = "Username";
            // 
            // btnLogin
            // 
            this.btnLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(122)))), ((int)(((byte)(90)))), ((int)(((byte)(255)))));
            this.btnLogin.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogin.Font = new System.Drawing.Font("Tw Cen MT", 14.25F, System.Drawing.FontStyle.Bold);
            this.btnLogin.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.btnLogin.Location = new System.Drawing.Point(40, 400);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(286, 40);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            this.btnLogin.Paint += new System.Windows.Forms.PaintEventHandler(this.btnLogin_Paint);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBoxPassword.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxPassword.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxPassword.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxPassword.Location = new System.Drawing.Point(40, 320);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(286, 22);
            this.textBoxPassword.TabIndex = 4;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tw Cen MT", 36F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(242)))));
            this.label1.Location = new System.Drawing.Point(36, 120);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 55);
            this.label1.TabIndex = 1;
            this.label1.Text = "Login";
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textBoxUsername.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxUsername.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.textBoxUsername.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.textBoxUsername.Location = new System.Drawing.Point(40, 250);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(286, 22);
            this.textBoxUsername.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tw Cen MT", 9.75F);
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(40, 180);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(142, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Enter your account details";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(357, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(541, 584);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            // 
            // pbClose
            // 
            this.pbClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbClose.Image = ((System.Drawing.Image)(resources.GetObject("pbClose.Image")));
            this.pbClose.Location = new System.Drawing.Point(938, 3);
            this.pbClose.Name = "pbClose";
            this.pbClose.Size = new System.Drawing.Size(35, 35);
            this.pbClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbClose.TabIndex = 1;
            this.pbClose.TabStop = false;
            this.pbClose.Click += new System.EventHandler(this.pbClose_Click);
            // 
            // LoginPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(979, 644);
            this.Controls.Add(this.pbClose);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginPage";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginPage";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.LoginPage_Paint);
            this.Resize += new System.EventHandler(this.LoginPage_Resize);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShowPass)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxUsername; // Renamed
        private System.Windows.Forms.TextBox textBoxPassword; // Renamed
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelUsername; // New
        private System.Windows.Forms.Label labelPassword; // New
        private System.Windows.Forms.Panel panelUsernameUnderline; // New
        private System.Windows.Forms.Panel panelPasswordUnderline; // New

        // Existing Paint event handler for btnLogin
        private void btnLogin_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            System.Windows.Forms.Button btn = (System.Windows.Forms.Button)sender;
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddArc(0, 0, 20, 20, 180, 90);
            path.AddArc(btn.Width - 20, 0, 20, 20, 270, 90);
            path.AddArc(btn.Width - 20, btn.Height - 20, 20, 20, 0, 90);
            path.AddArc(0, btn.Height - 20, 20, 20, 90, 90);
            path.CloseAllFigures();
            btn.Region = new System.Drawing.Region(path);
        }

        // New Paint event handler for panel1 to apply rounded corners
        private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Panel panel = (Panel)sender;
            GraphicsPath path = new GraphicsPath();
            int radius = 30; // You can adjust this value for more or less curve

            path.AddArc(0, 0, radius * 2, radius * 2, 180, 90); // Top-left
            path.AddArc(panel.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90); // Top-right
            path.AddArc(panel.Width - radius * 2, panel.Height - radius * 2, radius * 2, radius * 2, 0, 90); // Bottom-right
            path.AddArc(0, panel.Height - radius * 2, radius * 2, radius * 2, 90, 90); // Bottom-left
            path.CloseAllFigures();

            panel.Region = new Region(path);
        }

        // New Paint event handler for pictureBox1 to apply rounded corners
        private void pictureBox1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            GraphicsPath path = new GraphicsPath();
            int radius = 30; // You can adjust this value for more or less curve

            // The image in the reference has a rounded top-right corner and possibly bottom-right.
            // Adjusting arcs to match that visual.
            path.AddArc(pictureBox.Width - radius * 2, 0, radius * 2, radius * 2, 270, 90); // Top-right
            path.AddLine(pictureBox.Width, radius, pictureBox.Width, pictureBox.Height - radius); // Right side
            path.AddArc(pictureBox.Width - radius * 2, pictureBox.Height - radius * 2, radius * 2, radius * 2, 0, 90); // Bottom-right
            path.AddLine(pictureBox.Width - radius, pictureBox.Height, radius, pictureBox.Height); // Bottom side
            path.AddLine(0, pictureBox.Height, 0, 0); // Left side
            path.AddLine(0, 0, pictureBox.Width - radius, 0); // Top side (straight until top-right arc starts)

            path.CloseAllFigures();

            pictureBox.Region = new Region(path);
        }

        // New Paint event handler for the LoginPage form to draw the shadow for panel1
        private void LoginPage_Paint(object sender, PaintEventArgs e)
        {
            // Ensure panel1 is initialized and visible before attempting to draw its shadow
            if (this.panel1 != null && this.panel1.Visible)
            {
                // Get the bounds of panel1 relative to the form
                Rectangle panelBounds = this.panel1.Bounds;

                // Define shadow properties for a softer, more diffused look, expanded for a larger shadow
                int shadowSpread = 40; // Increased spread for a softer, wider blur
                int shadowOffset = 15;  // Base offset for shadows
                Color shadowColor = Color.FromArgb(30, 0, 0, 0); // Lighter, more transparent black (appears gray)
                int shadowRadius = 30; // Match the panel's corner radius

                // Set high quality rendering for smoother shadow
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // --- Draw Right/Bottom Shadow ---
                for (int i = 0; i < shadowSpread; i++)
                {
                    // Calculate current opacity, fading out as the shadow gets further from the panel
                    // Using a quadratic fade for a more natural falloff
                    double opacityFactor = 1.0 - Math.Pow((double)i / shadowSpread, 1.5); // Adjusted power for fade
                    int currentOpacity = (int)(shadowColor.A * opacityFactor);
                    if (currentOpacity < 0) currentOpacity = 0;

                    Color currentShadowColor = Color.FromArgb(currentOpacity, shadowColor.R, shadowColor.G, shadowColor.B);

                    using (SolidBrush currentShadowBrush = new SolidBrush(currentShadowColor))
                    {
                        // Create a GraphicsPath for each blur step, slightly offset
                        using (GraphicsPath blurPath = new GraphicsPath())
                        {
                            // The blur rectangle is offset to the right and bottom
                            Rectangle blurRect = new Rectangle(
                                panelBounds.X + (int)(shadowOffset * ((double)i / shadowSpread)),
                                panelBounds.Y + (int)(shadowOffset * ((double)i / shadowSpread)),
                                panelBounds.Width,
                                panelBounds.Height
                            );

                            // Add rounded corners to the blur path to match the panel's shape
                            blurPath.AddArc(blurRect.X, blurRect.Y, shadowRadius * 2, shadowRadius * 2, 180, 90); // Top-left
                            blurPath.AddArc(blurRect.Right - shadowRadius * 2, blurRect.Y, shadowRadius * 2, shadowRadius * 2, 270, 90); // Top-right
                            blurPath.AddArc(blurRect.Right - shadowRadius * 2, blurRect.Bottom - shadowRadius * 2, shadowRadius * 2, shadowRadius * 2, 0, 90); // Bottom-right
                            blurPath.AddArc(blurRect.X, blurRect.Bottom - shadowRadius * 2, shadowRadius * 2, shadowRadius * 2, 90, 90); // Bottom-left
                            blurPath.CloseAllFigures();

                            // Fill the current blur path
                            e.Graphics.FillPath(currentShadowBrush, blurPath);
                        }
                    }
                }

                // --- Draw Left Shadow (without top offset) ---
                // This loop draws a shadow to the left, starting from the top edge of the panel.
                for (int i = 0; i < shadowSpread; i++)
                {
                    double opacityFactor = 1.0 - Math.Pow((double)i / shadowSpread, 1.5);
                    int currentOpacity = (int)(shadowColor.A * opacityFactor);
                    if (currentOpacity < 0) currentOpacity = 0;

                    Color currentShadowColor = Color.FromArgb(currentOpacity, shadowColor.R, shadowColor.G, shadowColor.B);

                    using (SolidBrush currentShadowBrush = new SolidBrush(currentShadowColor))
                    {
                        using (GraphicsPath blurPath = new GraphicsPath())
                        {
                            // The blur rectangle is offset to the left, but not upwards
                            Rectangle blurRect = new Rectangle(
                                panelBounds.X - (int)(shadowOffset * ((double)i / shadowSpread)), // Negative offset for X (left)
                                panelBounds.Y, // No negative offset for Y (starts at panel's top)
                                panelBounds.Width,
                                panelBounds.Height
                            );

                            // Add rounded corners to the blur path to match the panel's shape
                            blurPath.AddArc(blurRect.X, blurRect.Y, shadowRadius * 2, shadowRadius * 2, 180, 90); // Top-left
                            blurPath.AddArc(blurRect.Right - shadowRadius * 2, blurRect.Y, shadowRadius * 2, shadowRadius * 2, 270, 90); // Top-right
                            blurPath.AddArc(blurRect.Right - shadowRadius * 2, blurRect.Bottom - shadowRadius * 2, shadowRadius * 2, shadowRadius * 2, 0, 90); // Bottom-right
                            blurPath.AddArc(blurRect.X, blurRect.Bottom - shadowRadius * 2, shadowRadius * 2, shadowRadius * 2, 90, 90); // Bottom-left
                            blurPath.CloseAllFigures();

                            // Fill the current blur path
                            e.Graphics.FillPath(currentShadowBrush, blurPath);
                        }
                    }
                }
            }
        }

        // New method to center panel1 within the form
        private void CenterPanel1()
        {
            if (this.panel1 != null)
            {
                int x = (this.ClientSize.Width - this.panel1.Width) / 2;
                int y = (this.ClientSize.Height - this.panel1.Height) / 2;
                this.panel1.Location = new Point(x, y);
            }
        }

        // Event handler for form Load
        private void LoginPage_Load(object sender, EventArgs e)
        {
            CenterPanel1(); // Center panel1 when the form loads
        }

        // Event handler for form Resize
        private void LoginPage_Resize(object sender, EventArgs e)
        {
            // Recenter panel1 when the form is resized (including maximize/restore)
            if (this.WindowState == FormWindowState.Maximized || this.WindowState == FormWindowState.Normal)
            {
                CenterPanel1();
            }
        }

        private PictureBox pbShowPass;
        private PictureBox pbClose;

        // Event handler for Forgot Password link


        // Event handler for Sign Up link
    }
}
