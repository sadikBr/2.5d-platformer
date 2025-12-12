using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class Door : Node3D
{

    private void OnArea3DBodyEntered(Node body)
    {
        if (body is not Player player) return;
        player.ReachedTheDoor();
    }
    
}