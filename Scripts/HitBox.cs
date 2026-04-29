using Godot;
using System;

public partial class HitBox : Area3D
{
	[Export]
	public int damage;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AreaEntered += _on_area_entered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public void _on_area_entered(Area3D area){
		if(area is HurtBox){
			area.Call("TakeDamage", damage);
		}
	}
}
