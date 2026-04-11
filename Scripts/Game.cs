using Godot;
using System;

public partial class Game : Node
{
	[Export]
	public Node enviroment;
	[Export]
	public CanvasLayer pauseMenu;
	[Export]
	public CanvasLayer gameOverMenu;
	
	//[Export]
	public PackedScene mainMenu;
	
	public bool paused = false;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mainMenu = ResourceLoader.Load<PackedScene>("res://Scenes/main.tscn");
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause"))
		{
			togglePause();
		}
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
	
	public void _on_resume_button_up(){
		togglePause();
	}
	
	public void _on_exit_button_up(){
		GetTree().Paused = false;
		SceneManager.Instance.ChangeScene(mainMenu);
	}
}
