using Godot;

public partial class CharacterBody2d : CharacterBody2D
{
	private const float Speed = 200.0f;
	private const float JumpVelocity = -300.0f;

	[Export] private AnimatedSprite2D _animatedSprite;

	private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        GD.Print("Game Started");
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		float directionX = 0f;
		if (Input.IsActionPressed("ui_left")) directionX -= 1f;
		if (Input.IsActionPressed("ui_right")) directionX += 1f;

		velocity.X = directionX * Speed;

		UpdateAnimation(directionX);

		Velocity = velocity;
		MoveAndSlide();
	}

	private void UpdateAnimation(float directionX)
	{
		if (_animatedSprite == null) return;

		string targetAnim;
		if (!IsOnFloor())
			targetAnim = Velocity.Y < 0f ? "Jump" : "Fall";
		else if (directionX != 0f)
			targetAnim = "Run";
		else
			targetAnim = "Idle";

		PlayAnimation(targetAnim);

		if (directionX != 0f)
		{
			_animatedSprite.FlipH = directionX < 0f;
		}
	}

	private void PlayAnimation(string animationName)
	{
		if (_animatedSprite.Animation != animationName)
		{
			_animatedSprite.Play(animationName);
		}
	}
}
