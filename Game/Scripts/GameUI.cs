using Godot;

namespace Empty.DPlatformer.Game.Scripts;

public partial class GameUi : Control
{
	[Export] private Player _player;
	[Export] private PackedScene _heart;
	[Export] private Texture2D _emptyHeartTexture;

	private HBoxContainer _heartsContainer;
	private Control _gameOverScreen;
	private TextureButton _restartButton;

	public override void _Ready()
	{
		_player.PlayerHealthChanged += HandlePlayerHealthChange;
		_player.PlayerDied += HandlePlayerDeathUi;
		_player.PlayerReachedDoor += HandlePlayerDeathUi;

		_heartsContainer = GetNode<HBoxContainer>("%HeartsContainer");
		_gameOverScreen = GetNode<Control>("GameOverScreen");
		_restartButton = GetNode<TextureButton>("%RestartButton");

		_restartButton.Pressed += HandleRestartButtonPress;
	}

	private void HandlePlayerHealthChange(int maxHealth, int currentHealth)
	{
		foreach (var child in _heartsContainer.GetChildren())
		{
			child.QueueFree();	
		}
		
		for (var i = 0; i < maxHealth; i++)
		{
			var heart = _heart.Instantiate<TextureRect>();
			if (i >= currentHealth)
			{
				heart.SetTexture(_emptyHeartTexture);
			}
			
			_heartsContainer.AddChild(heart);
		}
	}
	
	private void HandlePlayerDeathUi()
	{
		GetTree().CreateTimer(1.5f).Timeout += () =>
		{
			_gameOverScreen.Visible = true;
		};
	}
	
	private void HandleRestartButtonPress()
	{
		CallDeferred(nameof(RestartGame));
	}

	private void RestartGame()
	{
		GetTree().ReloadCurrentScene();
	}
}