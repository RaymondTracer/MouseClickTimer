using Gma.System.MouseKeyHook;

namespace MouseClickTimer
{
    public partial class Form1 : Form
    {
        private readonly IKeyboardMouseEvents KMEvents;

        private DateTime LastClick = DateTime.Now;
        private TimeSpan TimeSinceLastClick => DateTime.Now - LastClick;

        private readonly TimeSpan TimeThreshold = TimeSpan.FromSeconds(60);

        private bool IsClosing;
        private bool FirstClick = true;

        public Form1()
        {
            Shown += (_, _) => Task.Run(UpdateLabel);
            FormClosing += (_, _) => IsClosing = true;

            KMEvents = Hook.GlobalEvents();
            KMEvents.MouseDown += HandleMouseDown;

            InitializeComponent();
            Location = new(-8, 0);

            LblTimer.MouseDown += HandleMouseDown;
            MouseDown += HandleMouseDown;
        }

        private void HandleMouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is Control)
            {
                if (e.Button == MouseButtons.Right)
                {
                    FirstClick = !FirstClick;
                }
            }
            else
            {
                if (e.Button == MouseButtons.XButton1 && (TimeSinceLastClick >= TimeThreshold || FirstClick))
                {
                    LastClick = DateTime.Now;
                    FirstClick = false;
                }
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
                        BackColor = LblTimer.BackColor = Color.Orange;
                    }
                    else if (!FirstClick && TimeSinceLastClick < TimeThreshold && BackColor != Color.Red)
                    {
                        BackColor = LblTimer.BackColor = Color.Red;
                    }
                    else if (!FirstClick && TimeSinceLastClick >= TimeThreshold && BackColor != Color.Green)
                    {
                        BackColor = LblTimer.BackColor = Color.Green;
                    }
                }
                catch (Exception e)
                {
                    if (!IsClosing)
                    {
                        if (File.Exists(@"error.txt"))
                        {
                            File.Delete(@"error.txt");
                        }

                        File.WriteAllText("error.txt", e.ToString());
                        MessageBox.Show(e.Message, "Error logged to error.txt");
                    }
                }
            }
        }
    }
}