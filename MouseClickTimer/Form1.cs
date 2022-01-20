using Gma.System.MouseKeyHook;

using System.Diagnostics;

namespace MouseClickTimer
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents events;
        private DateTime lastClick = DateTime.Now;
        private bool closing;
        private bool firstClick = true;

        public Form1()
        {
            Shown += (s, e) => Task.Run(UpdateLabel);
            FormClosing += (s, e) => closing = true;

            events = Hook.GlobalEvents();
            events.MouseClick += Events_MouseClick;

            InitializeComponent();

            label1.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    firstClick = !firstClick;
                }
            };
        }

        private void Events_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1 && ((DateTime.Now - lastClick) >= TimeSpan.FromSeconds(60) || firstClick))
            {
                lastClick = DateTime.Now;
                firstClick = false;
            }
        }

        private void UpdateLabel()
        {
            while (!closing)
            {
                try
                {
                    Thread.Sleep(1);

                    TimeSpan timeSinceLastClick = DateTime.Now - lastClick;

                    Invoke(() => label1.Text = timeSinceLastClick.ToString());

                    if (firstClick && label1.ForeColor != Color.Orange)
                    {
                        label1.ForeColor = Color.Orange;
                    }
                    else if (!firstClick && timeSinceLastClick < TimeSpan.FromSeconds(60) && label1.ForeColor != Color.Red)
                    {
                        label1.ForeColor = Color.Red;
                    }
                    else if (!firstClick && timeSinceLastClick >= TimeSpan.FromSeconds(60) && label1.ForeColor != Color.Green)
                    {
                        label1.ForeColor = Color.Green;
                    }
                }
                catch (Exception e)
                {
                    if (!closing)
                    {
                        if (File.Exists(@".\error.txt"))
                        {
                            File.Delete(@".\error.txt");
                        }

                        File.WriteAllText(@".\error.txt", e.ToString());

                        MessageBox.Show(e.Message, "Error logged to error.txt");
                    }
                }
            }
        }
    }
}