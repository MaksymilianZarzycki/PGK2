using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Signal]
	public delegate void EnemyDestroyedEventHandler();
	[Export]
	public Node3D plane;
	[Export]
	public Node3D planeDir;
	[Export]
	public Player target;
	[Export]
	public Machinegun machinegun;
	[Export]
	public PackedScene exposlion;
	
	public int hp;    
	public bool shoot = false;       
	
	public const float movementSpeed = 20f;
	public const float acceleration = 0.15f;
	public const float rotationSpeed = 0.03f;
	public float angleX = 0;

	public override void _Ready(){
		hp = Globals.Instance.enemyHealth;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (target.NativeInstance == IntPtr.Zero){
			QueueFree();
			return;
		}
		Vector3 velocity = Velocity;
		
		Vector3 lookDir = Vector3.Forward.Rotated(Vector3.Right,target.plane.GlobalRotation.X);
		lookDir = lookDir.Rotated(Vector3.Up,target.plane.GlobalRotation.Y);
		lookDir = target.GlobalPosition+(lookDir+target.Velocity/2);
		lookDir = GlobalPosition.DirectionTo(lookDir);
		Basis rotatedBasis = Basis.LookingAt(lookDir).Orthonormalized();
		
		Basis = Basis.Slerp(rotatedBasis, rotationSpeed*(movementSpeed/(velocity.Length()+movementSpeed))).Orthonormalized();
		
		
		//plane.Rotation = new Vector3(plane.Rotation.X, plane.Rotation.Y, 0);
		Vector3 turnDir3 = planeDir.GlobalPosition.DirectionTo(target.GlobalPosition);
		turnDir3 = turnDir3.Rotated(Vector3.Up, -plane.Rotation.Y);
		Vector2 turnDir2 = new Vector2(turnDir3.X, -turnDir3.Y).Normalized();
		turnDir2 = turnDir2.Rotated(Mathf.DegToRad(-90));
		
		if(planeDir.GlobalPosition.AngleTo(target.GlobalPosition) > Mathf.DegToRad(0.1)){
			if(Mathf.Abs(Mathf.AngleDifference(plane.Rotation.Z,-turnDir2.Angle())) < Mathf.DegToRad(160)){
				plane.RotateObjectLocal(Vector3.Forward,Mathf.LerpAngle(0,Mathf.AngleDifference(plane.Rotation.Z,-turnDir2.Angle()),rotationSpeed/2));
			}
		}
		else{
			plane.RotateObjectLocal(Vector3.Forward,Mathf.LerpAngle(0,Mathf.AngleDifference(-plane.Rotation.Z,0),rotationSpeed/4));
		}
		
		/*if(Mathf.DegToRad(10)<velocity.Normalized().AngleTo(lookDir))
			angleX = Mathf.LerpAngle(angleX,Vector3.Up.AngleTo(velocity.DirectionTo(lookDir)), 0.05f);
		else{
			angleX = Mathf.LerpAngle(angleX, 0, 0.01f);
		}
		plane.Rotation = new Vector3(plane.Rotation.X, plane.Rotation.Y, angleX);*/
		
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
		if(hp<=0){
			Node3D newExposlion = (Node3D)exposlion.Instantiate();
			GetParent().AddChild(newExposlion);
			newExposlion.LookAt(Velocity);
			newExposlion.GlobalPosition = GlobalPosition;
			EmitSignal(SignalName.EnemyDestroyed);
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
