using Godot;
using System;

public partial class Globals : Node
{
	public static Globals Instance { get; private set; }
	
	public int playerHealth;
	public int enemyAmount;
	public int enemyHealth;
	public bool enemyRespawn;
	public int obstacleAmount;
	public int spawnArea;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
