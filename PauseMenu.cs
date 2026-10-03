using System.Collections.Generic;
using Godot;

public partial class PauseMenu : CanvasLayer
{
	[Export] private TextureButton _resumeButton;
	[Export] private TextureButton _settingsButton;
	[Export] private TextureButton _achivementsButton;

	[Export] private AudioStreamPlayer _clickPlayer;
	[Export] private AudioStreamPlayer _hoverPlayer;

	private const float NormalScale = 1.0f;
	private const float HoverScale = 1.1f;
	private const float PressedScale = 0.9f;
	private const float AnimDuration = 0.1f;

	private readonly Dictionary<TextureButton, Tween> _tweens = new();
	private TextureButton[] _buttons;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;

		_buttons = new[] { _resumeButton, _settingsButton, _achivementsButton };

		foreach (var button in _buttons)
			SetupButtonAnimation(button);

		_resumeButton.Pressed += Resume;
		_resumeButton.Pressed += OnClick;
		_resumeButton.MouseEntered += OnHover;

		_settingsButton.Pressed += OnClick;
		_achivementsButton.Pressed += OnClick;

		_settingsButton.MouseEntered += OnHover;
		_achivementsButton.MouseEntered += OnHover;
	}

	private void SetupButtonAnimation(TextureButton button)
	{
		// Scale from the center of the button instead of the top-left corner
		button.PivotOffset = button.Size / 2;
		button.Resized += () => button.PivotOffset = button.Size / 2;

		button.MouseEntered += () => AnimateScale(button, HoverScale);
		button.MouseExited += () => AnimateScale(button, NormalScale);
		button.ButtonDown += () => AnimateScale(button, PressedScale);
		button.ButtonUp += () => AnimateScale(button, button.IsHovered() ? HoverScale : NormalScale);
	}

	private void AnimateScale(TextureButton button, float target)
	{
		if (_tweens.TryGetValue(button, out var oldTween) && oldTween.IsValid())
			oldTween.Kill();

		var tween = CreateTween();
		tween.SetPauseMode(Tween.TweenPauseMode.Process); // keeps working while the game is paused
		tween.SetTrans(Tween.TransitionType.Quad);
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(button, "scale", Vector2.One * target, AnimDuration);

		_tweens[button] = tween;
	}

	private void ResetButtons()
	{
		foreach (var button in _buttons)
		{
			if (_tweens.TryGetValue(button, out var tween) && tween.IsValid())
				tween.Kill();

			button.Scale = Vector2.One * NormalScale;
		}
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (GetTree().Paused)
				Resume();
			else
				Pause();

			GetViewport().SetInputAsHandled();
		}
	}

	private void OnClick()
	{
		_clickPlayer.Play();
	}

	private void OnHover()
	{
		_hoverPlayer.Play();
	}

	private void Pause()
	{
		ResetButtons();
		GetTree().Paused = true;
		Visible = true;
		_resumeButton.GrabFocus();
	}

	private void Resume()
	{
		GetTree().Paused = false;
		Visible = false;
	}
}
