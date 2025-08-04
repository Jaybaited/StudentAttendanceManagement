using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAttendanceManagement
{
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();
        }

        private void LoadingScreen_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Simulate loading progress
            if (panelSlider.Width < panelBarBackground.Width)
            {
                panelSlider.Width += 8; // Slide speed
            }
            else
            {
                timer1.Stop();
                // Show LoginPage and close LoadingScreen
                LoginPage loginForm = new LoginPage();
                loginForm.Show();
                this.Hide();
            }
        }
    }
    }
