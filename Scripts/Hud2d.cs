using Godot;
using System;

public partial class Hud2d : CanvasLayer
{
	
	[Export]
	public CharacterBody3D player;
	[Export]
	public Label velocityLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocityLabel.Text = "Velocity: " + player.Velocity.Length().ToString() + "\nAltitude: " + player.Position.Y.ToString();
	}
}
