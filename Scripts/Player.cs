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
	public Node3D planeAxisZ;
	[Export]
	public Machinegun gun;
	[Export]
	public Node3D planeDir;
	[Export]
	public Timer invincibilityTimer;
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
	[Export]
	public Node3D target;
	[ExportGroup("Game")]
	[Export]
	public Node mapRoot;
	//[Export]
	//public PackedScene line;
	
	public const float movementSpeed = 20f;
	public const float acceleration = 0.15f;
	public const float rotationSpeed = 0.03f;
	/*public const float yawSpeed = 0.01f;
	public const float pitchSpeed = 0.02f;
	public const float rollSpeed = 0.05f;
	public const float passiveRotation = 0.01f;
	public float rollAngle = 0;*/
	
	public float pitchInputAxis;
	public float rollInputAxis;
	public float yawInputAxis;

	public float cameraSensitivity = 0.2f;
	public bool cameraManual = true;
	
	public int hp;
	public bool invincible = true;
	public float angleX = 0;
	
	public override void _Ready(){
		hp = Globals.Instance.playerHealth;
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
		
		/*
		//ROTATION
		plane.RotateObjectLocal(Vector3.Up, yawInputAxis*yawSpeed);
		plane.RotateObjectLocal(Vector3.Forward, rollInputAxis*rollSpeed);
		plane.RotateObjectLocal(Vector3.Right, pitchInputAxis*pitchSpeed);
		
		
		float rotAngle = plane.Rotation.Z;
		float downRot = -passiveRotation*Mathf.Cos(rotAngle);
		plane.RotateObjectLocal(Vector3.Right, passiveRotation);
		plane.Rotate(Vector3.Right.Rotated(Vector3.Up,plane.Rotation.Y), downRot);
		*/
		
		//MOVEMENT
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Right,plane.GlobalRotation.X);
		direction = direction.Rotated(Vector3.Up,plane.GlobalRotation.Y);
		velocity = velocity.MoveToward(direction * movementSpeed,acceleration);
		/*if(velocity.Length()<4){
			velocity = velocity.MoveToward(Vector3.Down * movementSpeed, 2f);
		}*/
		
		//Experimental
		//Vector3 lookDir = Vector3.Forward.Rotated(Vector3.Right,plane.GlobalRotation.X);
		//lookDir = lookDir.Rotated(Vector3.Up,plane.GlobalRotation.Y);
		//Vector3 lookDir = target.GlobalPosition;
		//lookDir = plane.GlobalPosition.DirectionTo(lookDir);
		//Basis rotatedBasis = Basis.LookingAt(lookDir).Orthonormalized();
		
		plane.Rotation = new Vector3(plane.Rotation.X, plane.Rotation.Y, 0);
		Vector3 turnDir3;
		if(cameraManual){
			turnDir3 = planeDir.GlobalPosition.DirectionTo(target.GlobalPosition);
			plane.RotateObjectLocal(Vector3.Up, Mathf.LerpAngle(0, Mathf.AngleDifference(plane.Rotation.Y,cameraAxisY.Rotation.Y), rotationSpeed*(movementSpeed/(velocity.Length()+movementSpeed))));
			plane.RotateObjectLocal(Vector3.Right, Mathf.LerpAngle(0, Mathf.AngleDifference(plane.Rotation.X,cameraAxisX.Rotation.X), rotationSpeed*(movementSpeed/(velocity.Length()+movementSpeed))));
		}
		else{
			turnDir3 = Vector3.Up;//GlobalPosition.DirectionTo(planeDir.GlobalPosition);
		}
		turnDir3 = turnDir3.Rotated(Vector3.Up, -plane.Rotation.Y);
		Vector2 turnDir2 = new Vector2(turnDir3.X, -turnDir3.Y).Normalized();
		turnDir2 = turnDir2.Rotated(Mathf.DegToRad(-90));
		
		
		if(planeDir.GlobalPosition.AngleTo(target.GlobalPosition) > Mathf.DegToRad(0.1)){
			if(Mathf.Abs(Mathf.AngleDifference(planeAxisZ.Rotation.Z,-turnDir2.Angle())) < Mathf.DegToRad(160)){
				planeAxisZ.RotateObjectLocal(Vector3.Forward,Mathf.LerpAngle(0,Mathf.AngleDifference(planeAxisZ.Rotation.Z,-turnDir2.Angle()),rotationSpeed/2));
			}
		}
		else{
			planeAxisZ.RotateObjectLocal(Vector3.Forward,Mathf.LerpAngle(0,Mathf.AngleDifference(-planeAxisZ.Rotation.Z,0),rotationSpeed/4));
		}
		
		//CAMERA
		if(Input.IsActionJustPressed("ManualCamera")){
			cameraManual = !cameraManual;	
		}
		 camera.Fov = 75 + Velocity.Length();
		
		//HUD
		pointer.Rotation = new Vector3(plane.Rotation.X,plane.Rotation.Y,planeAxisZ.Rotation.Z);
		//hud3D.GlobalPosition = new Vector3(hud3D.GlobalPosition.X,camera.GlobalPosition.Y,hud3D.GlobalPosition.Z);
		horizon.Rotation = new Vector3(0,plane.Rotation.Y,0);
		
		Velocity = velocity;
		if(hp>0){
			MoveAndSlide();
		}
		
		//COLLISION
		if(GetSlideCollisionCount()>0 && !invincible){
			on_hurt_box_hit(100);
			invincible = true;
			invincibilityTimer.WaitTime = 1;
			invincibilityTimer.Start();
		}
		
		//COMBAT
		if(Input.IsActionPressed("ShootPrimary")){
			gun.shoot();	
		}
	}
	
	public override void _Input(InputEvent @event){
		if(@event is InputEventMouseMotion motion){
			cameraAxisY.RotateY(Mathf.DegToRad(-motion.Relative.X * cameraSensitivity));			
			cameraAxisX.RotateX(Mathf.DegToRad(-motion.Relative.Y * cameraSensitivity));
			cameraAxisX.Rotation = new Vector3(Mathf.Clamp(cameraAxisX.Rotation.X,Mathf.DegToRad(-85),Mathf.DegToRad(85)), 0, 0);
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
