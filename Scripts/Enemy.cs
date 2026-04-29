using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export]
	public Node3D plane;
	[Export]
	public Player target;
	[Export]
	public Machinegun machinegun;
	
	public int hp = 100;    
	public bool shoot = false;       
	
	public const float movementSpeed = 15f;
	public const float acceleration = 0.1f;
	public const float rotationSpeed = 0.03f;
	public float angleX = 0;

	public override void _PhysicsProcess(double delta)
	{
		if (target.NativeInstance == IntPtr.Zero){
			QueueFree();
			return;
		}
		Vector3 velocity = Velocity;
		
		Vector3 lookDir = Vector3.Forward.Rotated(Vector3.Right,target.plane.GlobalRotation.X);
		lookDir = lookDir.Rotated(Vector3.Up,target.plane.GlobalRotation.Y);
		lookDir = target.GlobalPosition+(lookDir*5);
		lookDir = GlobalPosition.DirectionTo(lookDir);
		Basis rotatedBasis = Basis.LookingAt(lookDir).Orthonormalized();
		
		Basis = Basis.Slerp(rotatedBasis, rotationSpeed).Orthonormalized();
		
		if(Mathf.DegToRad(10)<velocity.Normalized().AngleTo(lookDir))
			angleX = Mathf.LerpAngle(angleX,Vector3.Up.AngleTo(velocity.DirectionTo(lookDir)), 0.05f);
		else{
			angleX = Mathf.LerpAngle(angleX, 0, 0.01f);
		}
		plane.Rotation = new Vector3(plane.Rotation.X, plane.Rotation.Y, angleX);
		
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Right,plane.GlobalRotation.X);
		direction = direction.Rotated(Vector3.Up,plane.GlobalRotation.Y);
		velocity = velocity.MoveToward(direction * movementSpeed,acceleration);

		Velocity = velocity;
		MoveAndSlide();
		
		if(shoot){
			machinegun.shoot();
		}
	}
	
	public void on_hurt_box_hit(int damage){
		hp -= damage;
		GD.Print("Enemy hit");
		if(hp<=0){
			QueueFree();
		}
	}
	
	public void on_attack_trigger_body_entered(Node3D body){
		if(body == target){
			shoot = true;
		}
	}
	
	public void on_attack_trigger_body_exited(Node3D body){
		if(body == target){
			shoot = false;
		}
	}
}
