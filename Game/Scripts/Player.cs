using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void PlayerHealthChangedEventHandler(int maxHealth, int currentHealth);

    [Signal]
    public delegate void PlayerDiedEventHandler();
    
    [Export] private PackedScene _groundSmokeParticles;
    
    private const float Speed = 10.0f;
    private const float JumpVelocity = 22.0f;
    private const int MaxJumpCount = 6;
    private int _currentJumpCount;

    private const int MaxHealth = 3;
    private int _currentHealth;
    private bool _controllable = true;
    private bool _isInvincible;

    private Node3D _littleAdventurerAndie;
    private PlayerAnimationTree _animationTree;
    private AnimationPlayer _materialAnimationPlayer;
    
    private GpuParticles3D _footstepParticles;
    private GpuParticles3D _playerHealParticles;

    public override void _Ready()
    {
        _littleAdventurerAndie = GetNode<Node3D>("%LittleAdventurerAndie");
        _animationTree = GetNode<PlayerAnimationTree>("%AnimationTree");
        _materialAnimationPlayer = GetNode<AnimationPlayer>("%MaterialAnimationPlayer");
        
        _footstepParticles = GetNode<GpuParticles3D>("%Footstep_VFX");
        _playerHealParticles = GetNode<GpuParticles3D>("%HEAL_Player_VFX");

        _currentHealth = MaxHealth;
        CallDeferred(nameof(EmitHealthChangedSignal), MaxHealth, _currentHealth);
    }

    public override void _Process(double delta)
    {
        HandleMovementParticles();
        
        _animationTree.Set("parameters/StateMachine/GroundMovement/blend_position", Mathf.Abs(Velocity.X));
        _animationTree.Set("parameters/StateMachine/Airborne/blend_position", Velocity.Y);

        if (IsOnFloor())
        {
            _animationTree.ChangeStateToNormal();
        }
        else
        {
            _animationTree.ChangeStateToAirborne();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var velocity = Velocity;
        
        if (!_controllable)
        {
            velocity += GetGravity() * (float)delta * 8;
            velocity.X = Mathf.MoveToward(Velocity.X, 0, 1.0f);

            Velocity = velocity;
            MoveAndSlide();
            return;
        }

        // Rotate the character to face the direction of the movement.
        if (velocity.X != 0)
        {
            var faceRight = velocity.X > 0;
            _littleAdventurerAndie.Rotation = new Vector3(0, faceRight ? 0 : Mathf.Pi, 0);
        }

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta * 8;
        }
        else
        {
            _currentJumpCount = MaxJumpCount;
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("Jump") && (IsOnFloor() || _currentJumpCount > 0))
        {
            velocity.Y = JumpVelocity;
            _currentJumpCount -= 1;
            PlayGroundSmokeParticles();
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

    public void TakeDamage()
    {
        if (_currentHealth == 0 || _isInvincible) return;
        
        _currentHealth -= 1;
        EmitHealthChangedSignal(MaxHealth, _currentHealth);
        
        _controllable = false;
        _isInvincible = true;

        if (_currentHealth <= 0)
        {
            _animationTree.ChangeStateToDead();
            EmitSignal(SignalName.PlayerDied);
        }
        else
        {
            _animationTree.Set("parameters/OneShotHurt/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            _materialAnimationPlayer.Play("Flash");
            GetTree().CreateTimer(0.9f).Timeout += () => { _controllable = true; };
            GetTree().CreateTimer(1.9f).Timeout += () =>
            {
                _materialAnimationPlayer.Play("RESET");
                _isInvincible = false;
            };
        }
    }
    
    public bool Heal()
    {
        if (_currentHealth >= MaxHealth) return false;
        
        _currentHealth += 1;
        _materialAnimationPlayer.Play("Heal");
        _playerHealParticles.Restart();
        EmitSignal(SignalName.PlayerHealthChanged, MaxHealth, _currentHealth);
        return true;
    }

    private void HandleMovementParticles()
    {
        _footstepParticles.Emitting = IsOnFloor() && Velocity.X != 0;

        if (IsOnFloor() && _animationTree.IsStateAirborne())
        {
            PlayGroundSmokeParticles();
        }
    }

    private void PlayGroundSmokeParticles()
    {
        var groundSmokeParticles = _groundSmokeParticles.Instantiate<GpuParticles3D>();
        var rootNode = GetTree().GetRoot().GetNode<Node3D>("Main");

        rootNode.AddChild(groundSmokeParticles);
        groundSmokeParticles.GlobalPosition = GlobalPosition + new Vector3(0f, 0.3f, 0.2f);
        groundSmokeParticles.Restart();

        GetTree().CreateTimer(0.6).Timeout += () => { groundSmokeParticles.QueueFree(); };
    }

    private void EmitHealthChangedSignal(int maxHealth, int newHealth)
    {
        EmitSignal(SignalName.PlayerHealthChanged, maxHealth, newHealth);
    }

}