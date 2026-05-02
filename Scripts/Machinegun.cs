using Godot;
using System;

public partial class Machinegun : Node3D
{
	[Export]
	public PackedScene bullet;
	[Export]
	public Timer cooldown;
	[Export]
	public Node3D parent;
	
	[Export]
	public int targetLayer = 1;
	[Export]
	public float spread = 1f;
	
	bool canShoot = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	
	public void shoot(){
		if(canShoot)
		{
			canShoot = false;
			cooldown.Start();
			Area3D newBullet = (Area3D)bullet.Instantiate();
			parent.GetParent().AddChild(newBullet);
			newBullet.GlobalPosition = GlobalPosition;
			newBullet.Rotate(Vector3.Right,GlobalRotation.X + (float)Mathf.DegToRad(GD.RandRange(-spread, spread)));
			newBullet.Rotate(Vector3.Up,GlobalRotation.Y + (float)Mathf.DegToRad(GD.RandRange(-spread, spread)));
			newBullet.CollisionLayer = 0;
			newBullet.CollisionMask = 0;
			newBullet.SetCollisionLayerValue(targetLayer,true);
			newBullet.SetCollisionMaskValue(targetLayer,true);
		}
	}
	
	public void on_cooldown_timer_timeout(){
		canShoot = true;
	}
}
