using Godot;

public partial class Player : CharacterBody3D
{
    public const float Speed = 10.0f;
    public const float JumpVelocity = 22.0f;

    private Node3D LittleAdventurerAndie;
    private PlayerAnimationTree AnimationTree;

    public override void _Ready()
    {
        LittleAdventurerAndie = GetNode<Node3D>("%LittleAdventurerAndie");
        AnimationTree = GetNode<PlayerAnimationTree>("%AnimationTree");
    }

    public override void _Process(double delta)
    {
        AnimationTree.Set("parameters/StateMachine/GroundMovement/blend_position", Mathf.Abs(Velocity.X));
        AnimationTree.Set("parameters/StateMachine/Airborne/blend_position", Velocity.Y);

        if (IsOnFloor())
        {
            AnimationTree.ChangeStateToNormal();
        }
        else
        {
            AnimationTree.ChangeStateToAirborne();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;

        // Rotate the character to face the direction of the movement.
        if (velocity.X != 0)
        {
            var faceRight = velocity.X > 0;
            LittleAdventurerAndie.Rotation = new Vector3(0, faceRight ? 0 : Mathf.Pi, 0);
        }

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta * 8;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Handle Movement.
        var direction = Input.GetAxis("Left", "Right");
        if (direction != 0)
        {
            velocity.X = direction * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, 1.0f);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
