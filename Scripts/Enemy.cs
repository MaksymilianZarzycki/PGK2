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
	public CharacterBody3D target;
	[Export]
	public Machinegun machinegun;
	[Export]
	public PackedScene exposlion;
	[Export]
	public CpuParticles3D engineTrail;
	
	public int maxHp; 
	public int hp;    
	public bool shoot = false;       
	
	public const float movementSpeed = 20f;
	public const float acceleration = 0.15f;
	public const float rotationSpeed = 0.03f;
	public float angleX = 0;
	
	StandardMaterial3D white;
	StandardMaterial3D grey;

	public override void _Ready(){
		maxHp = hp = Globals.Instance.enemyHealth;
		white = new StandardMaterial3D() { AlbedoColor = new Color(1, 1, 1), ShadingMode = 0 };
		grey = new StandardMaterial3D() { AlbedoColor = new Color(0.5f, 0.5f, 0.5f), ShadingMode = 0 };
		engineTrail.Mesh.SurfaceSetMaterial(0, white);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (target.NativeInstance == IntPtr.Zero){
			target = this;
		}
		Vector3 velocity = Velocity;
		
		Vector3 lookDir = target.GlobalPosition+target.Velocity/2;
		lookDir = GlobalPosition.DirectionTo(lookDir);
		if(lookDir == Vector3.Zero){
			lookDir = Vector3.Forward;
		}
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
		
		if(hp>maxHp/2){
			engineTrail.Mesh.SurfaceSetMaterial(0, white);
		}
		else
		{
			engineTrail.Mesh.SurfaceSetMaterial(0, grey);
		}
		
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
