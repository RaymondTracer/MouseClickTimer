using Gma.System.MouseKeyHook;

namespace MouseClickTimer
{
    public partial class Form1 : Form
    {
        private readonly IKeyboardMouseEvents KMEvents;
        private DateTime LastClick = DateTime.Now;
        private TimeSpan TimeSinceLastClick => DateTime.Now - LastClick;
        private bool IsClosing;
        private bool FirstClick = true;

        public Form1()
        {
            Shown += (s, e) => Task.Run(UpdateLabel);
            FormClosing += (s, e) => IsClosing = true;

            KMEvents = Hook.GlobalEvents();
            KMEvents.MouseDown += Events_MouseDown;

            InitializeComponent();
            Location = new(-8, 0);

            LblTimer.MouseDown += Form_MouseDown;
            MouseDown += Form_MouseDown;
        }

        private void Form_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                FirstClick = !FirstClick;
            }
        }

        private void Events_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.XButton1 && (TimeSinceLastClick >= TimeSpan.FromSeconds(60) || FirstClick))
            {
                LastClick = DateTime.Now;
                FirstClick = false;
            }
        }

        private void UpdateLabel()
        {
            LblTimer.ForeColor = DefaultBackColor;

            while (!IsClosing)
            {
                try
                {
                    Thread.Sleep(1);

                    Invoke(() => LblTimer.Text = TimeSinceLastClick.ToString());

                    if (FirstClick && BackColor != Color.Orange)
                    {
                        BackColor = Color.Orange;
                        LblTimer.BackColor = Color.Orange;
                    }
                    else if (!FirstClick && TimeSinceLastClick < TimeSpan.FromSeconds(60) && BackColor != Color.Red)
                    {
                        BackColor = Color.Red;
                        LblTimer.BackColor = Color.Red;
                    }
                    else if (!FirstClick && TimeSinceLastClick >= TimeSpan.FromSeconds(60) && BackColor != Color.Green)
                    {
                        BackColor = Color.Green;
                        LblTimer.BackColor = Color.Green;
                    }
                }
                catch (Exception e)
                {
                    if (!IsClosing)
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