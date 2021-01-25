using Godot;
using System;

public class Game : Spatial
{
	bool isBoxyLocked = false;
	
	public override void _Process(float delta)
	{
//		if(isBoxyLocked)
//		{
//			MoveBoxy(delta);
//		}
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		if(Input.IsActionPressed("lock_and_move_boxy"))
		{
			isBoxyLocked = true;
			MoveBoxy();
		}
		
		if(Input.IsActionPressed("unlock_and_change_boxy"))
		{
			isBoxyLocked = false;
		}
	}
	
	public void MoveBoxy()
	{
		
		var boxy = this.GetChild<Spatial>(1);
		
	}

}
