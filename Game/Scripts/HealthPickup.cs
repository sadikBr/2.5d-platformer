using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class HealthPickup : Node3D
{
    private const int RotationValue = 120;
    private Node3D _healthMesh;

    public override void _Ready()
    {
        _healthMesh = GetNode<Node3D>("HEAL_MESH");
    }

    public override void _Process(double delta)
    {
        _healthMesh.RotationDegrees += Vector3.Up * RotationValue * (float)delta;
    }

    private void OnArea3DBodyEntered(Node body)
    {
        if (body is not Player player) return;
        
        var healed = player.Heal();

        if (healed)
        {
            QueueFree();
        }
    }
    
}