using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class SpikeTrap : Node3D
{

    private void OnArea3DBodyEntered(Node body)
    {
        if (body is Player player)
        {
            player.TakeDamage();
        }
    }
    
}