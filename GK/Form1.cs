namespace GK
{
    public partial class Form1 : Form
    {
        FullScreen fullScreen;
        public Form1()
        {
            InitializeComponent();
            fullScreen = new FullScreen(this);
        }
    }
}
