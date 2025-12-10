using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class PlayerAnimationTree : AnimationTree
{

    private string _playerState = "Normal";

    public void ChangeStateToAirborne()
    {
        if (_playerState == "Dead") return;
        
        _playerState = "Airborne";
    }

    public void ChangeStateToNormal()
    {
        if (_playerState == "Dead") return;
        
        _playerState = "Normal";
    }

    public void ChangeStateToDead()
    {
        _playerState = "Dead";
    }
    
    public bool IsStateAirborne() { return _playerState == "Airborne"; }

}