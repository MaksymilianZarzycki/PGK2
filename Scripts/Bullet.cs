using Godot;
using System;

public partial class Bullet : HitBox
{
	[Export]
	public float speed;
	[Export]
	public float gravity;
	[Export]
	public PackedScene spark;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Right,GlobalRotation.X);
		direction = direction.Rotated(Vector3.Up,GlobalRotation.Y);
		Vector3 velocity = direction * speed * (float)delta;
		Position += velocity;
	}
	
	public void on_body_collision(Node3D body){
		Node3D newSpark = (Node3D)spark.Instantiate();
		GetParent().AddChild(newSpark);
		newSpark.GlobalPosition = GlobalPosition;
		QueueFree();
	}
	
	public void on_area_collision(Area3D area){
		Node3D newSpark = (Node3D)spark.Instantiate();
		GetParent().AddChild(newSpark);
		newSpark.GlobalPosition = GlobalPosition;
		QueueFree();
	}
	
	public void on_remove_timer_timeout(){
		QueueFree();
	}
}
