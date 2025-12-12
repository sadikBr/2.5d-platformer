using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class SpikeTrap : Node3D
{

    private const int Damage = 1;

    private void OnArea3DBodyEntered(Node body)
    {
        if (body is not Player player) return;
        
        player.TakeDamage(Damage);
    }
    
}