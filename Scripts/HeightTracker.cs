using Godot;
using System.Collections.Generic;

public partial class HeightTracker : Node
{
	[Signal] public delegate void HeightChangedEventHandler(float currentMeters, float bestMeters);
	[Signal] public delegate void MilestoneReachedEventHandler(int meters);

	[Export] private Node2D _target; // the player (node in "Main" scene)
	[Export] private float _pixelsPerMeter = 50f; // hopw many pixels play needs to pass for 1 meter
	[Export] private int[] _milestones = { 100 }; // i can add more in inspector for HeightTracker

	private float _startY;
	private float _bestMeters;
	private readonly HashSet<int> _reached = new();

	public override void _Ready()
	{
		_startY = _target.GlobalPosition.Y; // meter counter
	}

	public override void _Process(double delta)
	{
		// stops negative heights if the player ever ends up below the spawn point
		float current = Mathf.Max(0f, (_startY - _target.GlobalPosition.Y) / _pixelsPerMeter);

		if (current > _bestMeters)
			_bestMeters = current;

		EmitSignal(SignalName.HeightChanged, current, _bestMeters);

		foreach (int milestone in _milestones)
		{
			if (_bestMeters >= milestone && _reached.Add(milestone))
				EmitSignal(SignalName.MilestoneReached, milestone);
		}
	}
}
