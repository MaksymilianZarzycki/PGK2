using Godot;
using System;

public partial class SceneManager : Node
{
	public static SceneManager Instance { get; private set; }
	
	[Signal] public delegate void SceneChangeEventHandler();
	
	public Node currentScene;
	
	public override void _Ready()
	{
		Instance = this;
		currentScene = GetTree().GetRoot().GetChild(GetTree().GetRoot().GetChildCount()-1);
	}
	
	public void ChangeScene(PackedScene scene)
	{
		EmitSignal("SceneChange");
		currentScene.QueueFree();
		currentScene = scene.Instantiate();
		GetTree().GetRoot().AddChild(currentScene);
	}
}
