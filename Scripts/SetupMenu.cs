using Godot;
using System;

public partial class SetupMenu : Control
{
	[Export]
	public LineEdit playerHealth;
	[Export]
	public LineEdit enemyAmount;
	[Export]
	public LineEdit enemyHealth;
	[Export]
	public CheckButton enemyRespawn;
	[Export]
	public LineEdit obstacleAmount;
	[Export]
	public LineEdit spawnArea;
	
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
		Globals.Instance.playerHealth = playerHealth.Text.ToInt();
		Globals.Instance.enemyAmount = enemyAmount.Text.ToInt();
		Globals.Instance.enemyHealth = enemyHealth.Text.ToInt();
		Globals.Instance.enemyRespawn = enemyRespawn.ToggleMode;
		Globals.Instance.obstacleAmount = obstacleAmount.Text.ToInt();
		Globals.Instance.spawnArea = spawnArea.Text.ToInt();
		SceneManager.Instance.ChangeScene(ResourceLoader.Load<PackedScene>(startScenePath));
	}
	
	public void on_back_button_up(){
		SceneManager.Instance.ChangeScene(ResourceLoader.Load<PackedScene>(mainScenePath));
	}
}
