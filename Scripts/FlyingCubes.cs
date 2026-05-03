using Godot;
using System;

public partial class FlyingCubes : Node3D
{
	[Export]
	public PackedScene cube;
	[Export]
	public PackedScene enemy;
	[Export]
	public float spread = 100;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Globals.Instance.score = 0;
		spread = Globals.Instance.spawnArea;
		for(int i =0; i<Globals.Instance.obstacleAmount; i++){
			Node3D newCube = (Node3D)cube.Instantiate();
			AddChild(newCube);
			newCube.GlobalPosition = new Vector3((float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread));
		}
		
		for(int i =0; i<Globals.Instance.enemyAmount; i++){
			Enemy newEnemy = (Enemy)enemy.Instantiate();
			newEnemy.ProcessMode = Node.ProcessModeEnum.Disabled;
			AddChild(newEnemy);
			newEnemy.target = ((Game)GetParent().GetParent()).player;
			newEnemy.GlobalPosition = new Vector3((float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread));
			newEnemy.EnemyDestroyed += on_enemy_destroyed;
			newEnemy.ProcessMode = Node.ProcessModeEnum.Inherit;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void on_enemy_destroyed(){
		Globals.Instance.score ++;
		if(Globals.Instance.enemyRespawn){
			Enemy newEnemy = (Enemy)enemy.Instantiate();
			newEnemy.ProcessMode = Node.ProcessModeEnum.Disabled;
			AddChild(newEnemy);
			newEnemy.target = ((Game)GetParent().GetParent()).player;
			newEnemy.GlobalPosition = new Vector3((float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread),(float)GD.RandRange(-spread, spread));
			newEnemy.EnemyDestroyed += on_enemy_destroyed;
			newEnemy.ProcessMode = Node.ProcessModeEnum.Inherit;
		}
	}
}
