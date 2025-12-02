using Godot;

public partial class PlayerAnimationTree : AnimationTree
{

    private string PlayerState = "Normal";

    public void ChangeStateToAirborne()
    {
        PlayerState = "Airborne";
    }

    public void ChangeStateToNormal()
    {
        PlayerState = "Normal";
    }

    public void ChangeStateToDead()
    {
        PlayerState = "Dead";
    }

}
