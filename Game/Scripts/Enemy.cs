using System;
using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class Enemy : CharacterBody3D
{
	private const float Speed = 1.4f;

	private RayCast3D _forwardRay;
	private RayCast3D _downwardRay;
	
	private Node3D _mesh;
	private CollisionShape3D _collisionShape;
	private AnimationPlayer _animationPlayer;
	
	private int _direction;
	private bool _facingRight = true;

	public override void _Ready()
	{
		_forwardRay = GetNode<RayCast3D>("%ForwardRay");
		_downwardRay = GetNode<RayCast3D>("%DownwardRay");
		
		_mesh = GetNode<Node3D>("%EnemyMesh");
		_collisionShape = GetNode<CollisionShape3D>("%EnemyCollisionShape");
		_animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		var velocity = Velocity;

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
}