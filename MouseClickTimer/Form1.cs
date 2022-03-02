using Gma.System.MouseKeyHook;

namespace MouseClickTimer
{
    public partial class Form1 : Form
    {
        private readonly IKeyboardMouseEvents events;
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

            lblTimer.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    firstClick = !firstClick;
                }
            };

            MouseDown += (s, e) =>
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
            lblTimer.ForeColor = DefaultBackColor;

            while (!closing)
            {
                try
                {
                    Thread.Sleep(1);

                    TimeSpan timeSinceLastClick = DateTime.Now - lastClick;

                    Invoke(() => lblTimer.Text = timeSinceLastClick.ToString());

                    if (firstClick && lblTimer.ForeColor != Color.Orange)
                    {
                        BackColor = Color.Orange;
                        lblTimer.BackColor = Color.Orange;
                    }
                    else if (!firstClick && timeSinceLastClick < TimeSpan.FromSeconds(60) && lblTimer.ForeColor != Color.Red)
                    {
                        BackColor = Color.Red;
                        lblTimer.BackColor = Color.Red;
                    }
                    else if (!firstClick && timeSinceLastClick >= TimeSpan.FromSeconds(60) && lblTimer.ForeColor != Color.Green)
                    {
                        BackColor = Color.Green;
                        lblTimer.BackColor = Color.Green;
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