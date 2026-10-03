using Godot;

public partial class HeightHud : CanvasLayer
{
	[Export] private HeightTracker _tracker;
	[Export] private Label _heightLabel;
	[Export] private Label _toastLabel;

	public override void _Ready()
	{
		_toastLabel.Visible = false;

		_tracker.HeightChanged += OnHeightChanged;
		_tracker.MilestoneReached += OnMilestoneReached;
	}

	private void OnHeightChanged(float current, float best)
	{
		// FloorToInt: "100 m" only shows once you've truly reached 100 or any other milestone
		_heightLabel.Text = $"Height: {Mathf.FloorToInt(current)} m\nBest: {Mathf.FloorToInt(best)} m";
	}

	private void OnMilestoneReached(int meters)
	{
		GD.Print($"Milestone reached: {meters} m"); // the badge system will hook in here later

		_toastLabel.Text = $"{meters} m reached!";
		_toastLabel.Visible = true;

		// hide the message after 3 seconds
		GetTree().CreateTimer(3.0).Timeout += () => _toastLabel.Visible = false;
	}
}
