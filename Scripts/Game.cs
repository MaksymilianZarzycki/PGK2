using Godot;
using System;

public partial class Game : Node
{
	[Export]
	public PackedScene scene;
	[Export]
	public Node enviroment;
	[Export]
	public CanvasLayer pauseMenu;
	[Export]
	public CanvasLayer gameOverMenu;
	[Export]
	public Label scoreLabel;
	[Export]
	public Timer gameOverTimer;
	
	//[Export]
	public PackedScene mainMenu;
	
	public Player player;
	public bool paused = false;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = false;
		mainMenu = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		var unpacked = scene.Instantiate();
		enviroment.AddChild(unpacked);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause") && !gameOverMenu.Visible)
		{
			togglePause();
		}
		
		/*if(paused==false && GetTree().Paused == true){
			GetTree().Paused = false;
			pauseMenu.Visible = false;
			gameOverMenu.Visible = false;
		}*/
	}
	
	public void togglePause(){
		paused = !paused;
			if(paused){
				GetTree().Paused = true;
				pauseMenu.Visible = true;
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				GetTree().Paused = false;
				pauseMenu.Visible = false;
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
	}
	
	public void SetPlayer(Player player){
		this.player = player;
		player.PlayerDestroyed += on_player_destroyed;
	}
	
	public void _on_resume_button_up(){
		togglePause();
	}
	
	public void _on_exit_button_up(){
		GetTree().Paused = false;
		SceneManager.Instance.ChangeScene(mainMenu);
	}
	
	public void _on_restart_button_up(){
		enviroment.GetChild(0).QueueFree();
		var unpacked = scene.Instantiate();
		enviroment.AddChild(unpacked);
		
		paused = false;
		gameOverMenu.Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		GetTree().Paused = false;
	}
	
	public void on_player_destroyed(){
		gameOverTimer.Start();
	}
	
	public void on_game_over_timer_timeout(){
		scoreLabel.Text = "Score\n" + Globals.Instance.score.ToString();
		paused = true;
		GetTree().Paused = true;
		gameOverMenu.Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
