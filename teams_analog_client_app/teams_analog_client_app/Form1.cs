namespace teams_analog_client_app
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            VideoClient videoClient = new VideoClient(playerPictureBox);
        }
    }
}
