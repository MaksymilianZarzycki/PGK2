using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[ExportGroup("Plane")]
	[Export]
	public Node3D plane;
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
	public Node3D hud;
	[Export]
	public Node3D pointer;
	[Export]
	public PackedScene line;
	
	public const float movementSpeed = 10f;
	//public const float rotationSpeed = 0.02f;
	public const float yawSpeed = 0.01f;
	public const float pitchSpeed = 0.02f;
	public const float rollSpeed = 0.05f;
	public const float passiveRotation = 0.01f;
	public float rollAngle = 0;
	
	public float pitchInputAxis;
	public float rollInputAxis;
	public float yawInputAxis;

	public float cameraSensitivity = 0.1f;
	public bool cameraManual = false;
	
	public override void _Ready(){
		
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;
		
		pitchInputAxis = Input.GetAxis("PitchDown", "PitchUp");
		rollInputAxis = Input.GetAxis("RollLeft", "RollRight");
		yawInputAxis = Input.GetAxis("YawRight", "YawLeft");
		
		plane.RotateObjectLocal(Vector3.Up, yawInputAxis*yawSpeed);
		plane.RotateObjectLocal(Vector3.Forward, rollInputAxis*rollSpeed);
		plane.RotateObjectLocal(Vector3.Right, pitchInputAxis*pitchSpeed);
		
		//plane.RotateObjectLocal(Vector3.Right, passiveRotation);
		float rotAngle = plane.Rotation.Z;
		float downRot = -passiveRotation*Mathf.Cos(rotAngle);
		plane.RotateObjectLocal(Vector3.Right, passiveRotation);
		plane.Rotate(Vector3.Right.Rotated(Vector3.Up,plane.Rotation.Y), downRot);
		
		
		Vector3 direction = Vector3.Forward.Rotated(Vector3.Right,plane.GlobalRotation.X);
		direction = direction.Rotated(Vector3.Up,plane.GlobalRotation.Y);
		velocity = velocity.MoveToward(direction * movementSpeed, 0.15f);
		if(velocity.Length()<4){
			velocity = velocity.MoveToward(Vector3.Down * movementSpeed, 2f);
		}
		
		if(!cameraManual){
			cameraAxisY.Rotation = new Vector3(0,plane.Rotation.Y,0);
			cameraAxisX.Rotation = new Vector3(plane.Rotation.X,0,0);
		}
		
		pointer.Rotation = plane.Rotation;
		hud.GlobalPosition = new Vector3(hud.GlobalPosition.X,camera.GlobalPosition.Y,hud.GlobalPosition.Z);
		hud.Rotation = new Vector3(0,plane.Rotation.Y,0);
		
		Velocity = velocity;
		MoveAndSlide();
	}
	
	public override void _Input(InputEvent @event){
		if(@event is InputEventMouseMotion motion){
			cameraAxisY.RotateY(Mathf.DegToRad(-motion.Relative.X * cameraSensitivity));			
			cameraAxisX.RotateX(Mathf.DegToRad(-motion.Relative.Y * cameraSensitivity));
			cameraManual = true;
			cameraResetTimer.Start();
		}
	}

	public void _on_camera_reset_timer_timeout(){
		cameraManual = false;
	}
}
