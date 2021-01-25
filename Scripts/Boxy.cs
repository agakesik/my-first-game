using Godot;
using System;

public class Boxy : Spatial
{	
	int chosenBoxIndex = 0;
	Godot.Collections.Array boxesAndPositions = new Godot.Collections.Array();
	bool isBoxChosen = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ListBoxesPositions();
		GD.Print($"List of positions of boxes:");
		for (int i=0; i<boxesAndPositions.Count; i++)
		{
			GD.Print(boxesAndPositions[i]);
		}
		CreateChosenBoxIndicator(chosenBoxIndex);
		
		GD.Print($"index wektora: {boxesAndPositions.IndexOf(new Vector3(0,0,0))}");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
	}
	
	public override void _Input(InputEvent inputEvent)
	{
		if (Input.IsKeyPressed((int)KeyList.Key1))
		{
			ChangeChosenBoxIndicatorParent(chosenBoxIndex, 1);
			chosenBoxIndex = 1;
			
		}
				
		if(Input.IsActionPressed("box_choose") && isBoxChosen)
		{
			ChangeStatusToChooseBox();
			
		}
		if(Input.IsActionPressed("box_move") && !isBoxChosen)
		{
			ChangeStatusToMoveBox();
		}
		
		// moving box
		if (Input.IsActionPressed("z_minus"))
		{
			MoveOrChooseBox(new Vector3(-1,0,0));
			
		}
		if (Input.IsActionPressed("z_plus"))
		{
			MoveOrChooseBox( new Vector3(1,0,0));
		}
		if (Input.IsActionPressed("x_plus"))
		{
			MoveOrChooseBox(new Vector3(0,0,1));
		}
		if (Input.IsActionPressed("x_minus"))
		{
			MoveOrChooseBox(new Vector3(0,0,-1));
		}
		if (Input.IsActionPressed("y_plus"))
		{
			MoveOrChooseBox(new Vector3(0,1,0));
		}
		if (Input.IsActionPressed("y_minus"))
		{
			MoveOrChooseBox(new Vector3(0,-1,0));
		}
	}
	
	private void ChangeStatusToChooseBox()
	{
		isBoxChosen = false;
		GD.Print("currently: choose box");
	}
	
	private void ChangeStatusToMoveBox()
	{
		var currentChosenBox = GetBox(chosenBoxIndex);
		var chosenBoxIndicator = GetChosenBoxIndicator(chosenBoxIndex);
		var indicatorPositionRelatedToParent = chosenBoxIndicator.Translation;
		var indicatorPositionGlobal = indicatorPositionRelatedToParent + currentChosenBox.Translation;
		
		if(!isThereBoxToChoose(indicatorPositionGlobal))
		{
			GD.Print("no box to choose");
			GD.Print("currently: choose box");
			return;
			
		}
		else
		{
			ChangeChosenBox(indicatorPositionGlobal);
			isBoxChosen = true;	
			GD.Print("currently: move box");
		}
	}
	
	private bool isThereBoxToChoose(Vector3 indicatorPositionGlobal)
	{
		var chosenBoxIndicator = GetChosenBoxIndicator(chosenBoxIndex);
		if(boxesAndPositions.Contains(indicatorPositionGlobal))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	
	private void  ChangeChosenBox(Vector3 indicatorPositionGlobal)
	{
		var chosenBoxIndicator = GetChosenBoxIndicator(chosenBoxIndex);
		int newChosenBoxIndex = boxesAndPositions.IndexOf(indicatorPositionGlobal);
		ChangeChosenBoxIndicatorParent(chosenBoxIndex, newChosenBoxIndex);
		chosenBoxIndex = newChosenBoxIndex;
	}
	
	private void ListBoxesPositions()
	{
		var arrayOfChildren = this.GetChildren();
		for(int i=0; i<arrayOfChildren.Count-1; i++)
		{
			var child = (Spatial)arrayOfChildren[i];
			GD.Print($"{child.Name} -position: {child.Translation}");
			boxesAndPositions.Add(child.Translation);
		}
	}
	
	private void MoveOrChooseBox(Vector3 direction)
	{
		if(isBoxChosen)
		{
			MoveChosenBox(direction);
		}
		else
		{
			MoveChosenBoxIndicatorIndicator(direction);
		}
	}
	
	private void MoveChosenBox(Vector3 direction)
	{
		var chosenBox = GetBox(chosenBoxIndex);
		Vector3 positionChange = FirstFreeSpot(chosenBox.Translation, direction);
		boxesAndPositions[chosenBoxIndex] = positionChange;
		chosenBox.Translation = positionChange;
	}
	
	
	private Vector3 FirstFreeSpot(Vector3 currentPosition, Vector3 direction)
	{
		bool isSpotFree = false;
		Vector3 positionChange = currentPosition + direction;
		while(!isSpotFree)
		{
			if(!boxesAndPositions.Contains(positionChange))
			{
				isSpotFree = true;
				break;
			}
			else
			{
				positionChange += direction;
			}
		}
		return positionChange;
	}
	
	private void MoveChosenBoxIndicatorIndicator(Vector3 direction)
	{
		var chosenBoxIndicator = GetChosenBoxIndicator(chosenBoxIndex);
		chosenBoxIndicator.Translation += direction;
	}
	
	
	private void CreateChosenBoxIndicator(int chosenBoxIndex)
	{
		var  chosenBoxIndicatorScene = (PackedScene)GD.Load("res://Assets/ChosenBoxIndicator.tscn");
		var chosenBoxIndicator = (MeshInstance)chosenBoxIndicatorScene.Instance();
		var chosenBox = GetBox(chosenBoxIndex);
		chosenBox.AddChild(chosenBoxIndicator);
	}
	
//	private void DeleteChosenBoxIndicator(int previousChosenBoxIndex)
//	{
//		var previousChosenBox = this.GetChild<Spatial>(previousChosenBoxIndex);
//		var previousChosenBoxIndicator = previousChosenBox.GetChild<MeshInstance>(2);
//		previousChosenBoxIndicator.QueueFree();
//	}

	private void ChangeChosenBoxIndicatorParent(int previousChosenBoxIndex, int newChosenBoxIndex)
	{
		var previousChosenBox = GetBox(previousChosenBoxIndex);
		var newChosenBox = GetBox(newChosenBoxIndex);
		var chosenBoxIndicator = GetChosenBoxIndicator(previousChosenBoxIndex);
		previousChosenBox.RemoveChild(chosenBoxIndicator);
		newChosenBox.AddChild(chosenBoxIndicator);
		chosenBoxIndicator.Translation = new Vector3(0,0,0);
	}
	
	private MeshInstance GetChosenBoxIndicator(int currentChosenBoxIndex)
	{
		var currentChosenBox = GetBox(currentChosenBoxIndex);
		var chosenBoxIndicator = currentChosenBox.GetChild<MeshInstance>(2);
		return chosenBoxIndicator;
	}
	
	private Spatial GetBox(int boxIndex)
	{
		var box = this.GetChild<Spatial>(boxIndex);
		return box;
	}
}
