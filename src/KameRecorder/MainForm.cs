using KameRecorder.Abstractions;

namespace KameRecorder;

public partial class MainForm : Form
{
	private readonly IRecorderService _recorderService;
	
	public MainForm(IRecorderService recorderService)
	{
		InitializeComponent();
		
		_recorderService = recorderService;
	}

	private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		_recorderService.Stop();
	}

	private void MainForm_Shown(object sender, EventArgs e)
	{
		_recorderService.Start();
	}
}