using Godot;
using System;

public partial class SetupMenu : Control
{
	[Export]
	public SpinBox playerHealth;
	[Export]
	public SpinBox enemyAmount;
	[Export]
	public SpinBox enemyHealth;
	[Export]
	public CheckButton enemyRespawn;
	[Export]
	public SpinBox obstacleAmount;
	[Export]
	public SpinBox spawnArea;
	
	[Export]
	public string startScenePath;
	[Export]
	public string mainScenePath;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void on_start_button_up(){
		Globals.Instance.playerHealth = (int)playerHealth.Value;
		Globals.Instance.enemyAmount = (int)enemyAmount.Value;
		Globals.Instance.enemyHealth = (int)enemyHealth.Value;
		Globals.Instance.enemyRespawn = enemyRespawn.ToggleMode;
		Globals.Instance.obstacleAmount = (int)obstacleAmount.Value;
		Globals.Instance.spawnArea = (int)spawnArea.Value;
		SceneManager.Instance.ChangeScene(ResourceLoader.Load<PackedScene>(startScenePath));
	}
	
	public void on_back_button_up(){
		SceneManager.Instance.ChangeScene(ResourceLoader.Load<PackedScene>(mainScenePath));
	}
}
