using System;
using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class Enemy : CharacterBody3D
{
	private const float Speed = 1.4f;
	private const int Damage = 2;

	private RayCast3D _forwardRay;
	private RayCast3D _downwardRay;
	
	private Node3D _mesh;
	private CollisionShape3D _collisionShape;
	private Area3D _hitbox;
	
	private AnimationPlayer _animationPlayer;
	private AnimationPlayer _materialAnimationPlayer;

	private int _currentHealth = 20;
	
	private int _direction;
	private bool _facingRight = true;

	public override void _Ready()
	{
		_forwardRay = GetNode<RayCast3D>("%ForwardRay");
		_downwardRay = GetNode<RayCast3D>("%DownwardRay");
		
		_mesh = GetNode<Node3D>("%EnemyMesh");
		_collisionShape = GetNode<CollisionShape3D>("%EnemyCollisionShape");
		_hitbox = GetNode<Area3D>("%Hitbox");
		
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		_materialAnimationPlayer = GetNode<AnimationPlayer>("%MaterialAnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

		if (_currentHealth <= 0)
		{
			return;
		}

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta * 8;
		}

		if (_forwardRay.IsColliding() || !_downwardRay.IsColliding())
		{
			_facingRight = !_facingRight;
		}

		if (_facingRight)
		{
			_direction = 1;
			_mesh.Rotation = Vector3.Up * Mathf.Pi / 2;
			_collisionShape.Rotation = Vector3.Zero;
		}
		else
		{
			_direction = -1;
			_mesh.Rotation = Vector3.Down * Mathf.Pi / 2;
			_collisionShape.Rotation = Vector3.Down * Mathf.Pi;
		}

		velocity.X = _direction * Speed;

		Velocity = velocity;
		MoveAndSlide();
	}

	public void TakeDamage(int damage)
	{
		if (_currentHealth <= 0) return;
		
		_currentHealth -= damage;
		_materialAnimationPlayer.Play("Flash");

		if (_currentHealth <= 0)
		{
			_animationPlayer.Play("NPC_01_DEAD");
			_collisionShape.SetDeferred(CollisionShape3D.PropertyName.Disabled, true);
			_hitbox.SetMonitoring(false);
		}
		
	}

	private void OnArea3DBodyEntered(Node body)
	{
		if (body is not Player player) return;
		player.TakeDamage(Damage);
	}
}