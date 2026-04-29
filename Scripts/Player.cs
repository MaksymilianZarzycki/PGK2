using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Signal]
	public delegate void PlayerDestroyedEventHandler();
	
	[ExportGroup("Plane")]
	[Export]
	public Node3D plane;
	[Export]
	public Machinegun gun;
	[ExportGroup("Camera")]
	[Export]
	public Node3D cameraAxisY;
	[Export]
	public Node3D cameraAxisX;
	[Export]
	public Camera3D camera;
	[Export]
	public Timer cameraResetTimer;
	[ExportGroup("Hud")]
	[Export]
	public Node3D hud3D;
	[Export]
	public Node3D horizon;
	[Export]
	public Node3D pointer;
	[ExportGroup("Game")]
	[Export]
	public Node mapRoot;
	//[Export]
	//public PackedScene line;
	
	public const float movementSpeed = 15f;
	public const float acceleration = 0.15f;
	//public const float rotationSpeed = 0.02f;
	public const float yawSpeed = 0.01f;
	public const float pitchSpeed = 0.02f;
	public const float rollSpeed = 0.05f;
	public const float passiveRotation = 0.01f;
	public float rollAngle = 0;
	
	public float pitchInputAxis;
	public float rollInputAxis;
	public float yawInputAxis;

	public float cameraSensitivity = 0.2f;
	public bool cameraManual = false;
	
	public int hp = 100;
	public bool invincible = true;
	
	public override void _Ready(){
		if(mapRoot.GetParent().GetParent() is Game){
			mapRoot.GetParent().GetParent().Call("SetPlayer",this);
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		//INPUT
		pitchInputAxis = Input.GetAxis("PitchDown", "PitchUp");
		rollInputAxis = Input.GetAxis("RollLeft", "RollRight");
		yawInputAxis = Input.GetAxis("YawRight", "YawLeft");
		
		//ROTATION
		plane.RotateObjectLocal(Vector3.Up, yawInputAxis*yawSpeed);
		plane.RotateObjectLocal(Vector3.Forward, rollInputAxis*rollSpeed);
		plane.RotateObjectLocal(Vector3.Right, pitchInputAxis*pitchSpeed);
		
		
		float rotAngle = plane.Rotation.Z;
		float downRot = -passiveRotation*Mathf.Cos(rotAngle);
		plane.RotateObjectLocal(Vector3.Right, passiveRotation);
		plane.Rotate(Vector3.Right.Rotated(Vector3.Up,plane.Rotation.Y), downRot);
		
		//MOVEMENT
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Right,plane.GlobalRotation.X);
		direction = direction.Rotated(Vector3.Up,plane.GlobalRotation.Y);
		velocity = velocity.MoveToward(direction * movementSpeed,acceleration);
		/*if(velocity.Length()<4){
			velocity = velocity.MoveToward(Vector3.Down * movementSpeed, 2f);
		}*/
		
		//CAMERA
		if(Input.IsActionJustPressed("ManualCamera")){
			cameraManual = !cameraManual;	
		}
		if(!cameraManual){
			/*
			cameraAxisY.Rotation = new Vector3(0,Mathf.LerpAngle(cameraAxisY.Rotation.Y,plane.Rotation.Y,0.2f),0);
			cameraAxisX.Rotation = new Vector3(Mathf.LerpAngle(cameraAxisY.Rotation.X,plane.Rotation.X,0.6f),0,0);
			*/
			
			cameraAxisY.RotateY(Mathf.AngleDifference(cameraAxisY.Rotation.Y,plane.Rotation.Y)*0.2f);			
			cameraAxisX.RotateX(Mathf.AngleDifference(cameraAxisX.Rotation.X,plane.Rotation.X)*0.6f);
		}
		
		//HUD
		pointer.Rotation = plane.Rotation;
		//hud3D.GlobalPosition = new Vector3(hud3D.GlobalPosition.X,camera.GlobalPosition.Y,hud3D.GlobalPosition.Z);
		horizon.Rotation = new Vector3(0,plane.Rotation.Y,0);
		
		Velocity = velocity;
		if(hp>0){
			MoveAndSlide();
		}
		
		//COLLISION
		if(GetSlideCollisionCount()>0){
			on_hurt_box_hit(hp);
		}
		
		//COMBAT
		if(Input.IsActionPressed("ShootPrimary")){
			gun.shoot();	
		}
	}
	
	public override void _Input(InputEvent @event){
		if(@event is InputEventMouseMotion motion && cameraManual){
			cameraAxisY.RotateY(Mathf.DegToRad(-motion.Relative.X * cameraSensitivity));			
			cameraAxisX.RotateX(Mathf.DegToRad(-motion.Relative.Y * cameraSensitivity));

			//cameraResetTimer.Start();
		}
	}

	public void _on_camera_reset_timer_timeout(){
		//cameraManual = false;
	}
	
	public void _on_invincibility_timeout(){
		invincible = false;
	}
	
	public void on_hurt_box_hit(int damage){
		if(!invincible){
			hp -= damage;
		}
		if(hp<=0){
			var pos = camera.GlobalTransform;
			cameraAxisX.RemoveChild(camera);
			GetParent().AddChild(camera);
			camera.GlobalTransform = pos;
			
			EmitSignal(SignalName.PlayerDestroyed);
			QueueFree();
		}
	}
}
