using Godot;
using System;

public partial class Hud2d : CanvasLayer
{
	
	[Export]
	public Player player;
	[Export]
	public Label velocityLabel;
	[Export]
	public ProgressBar hpBar;
	[Export]
	public ProgressBar speedBar;
	[Export]
	public Label hpLabel;
	[Export]
	public Label speedLabel;
	[Export]
	public Label scoreLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		hpBar.Value = Mathf.Lerp(hpBar.Value,((float)player.hp/(float)player.maxHp)*100,0.1f);
		speedBar.Value = (player.Velocity.Length()/player.movementSpeed)*100;
		hpLabel.Text = player.hp.ToString();
		if(player.hp>player.maxHp/2){
			hpBar.SelfModulate = new Color(0, 1, 0);
		}
		else
		{
			hpBar.SelfModulate = new Color(1, 0.5f, 0.3f);
		}
		speedLabel.Text = ((int)(player.Velocity.Length()*20*3.6f)).ToString() + "km/h";
		scoreLabel.Text = Globals.Instance.score.ToString();
	}
}
