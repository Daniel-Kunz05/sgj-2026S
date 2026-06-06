using Godot;

public partial class Draggable : Area2D
{
    [Export] public Node2D OwnerParent;
    
    [Signal] public delegate void MouseClickEventHandler(Draggable draggable);
    [Signal] public delegate void DragStartEventHandler(Draggable draggable);
    [Signal] public delegate void DragEndEventHandler(Draggable draggable);
    [Signal] public delegate void AcceptedEventHandler(Draggable draggable);
    [Signal] public delegate void DeclinedEventHandler(Draggable draggable);

    public bool AnswerReceived;
    public bool AllowDragging = true;
    
    
    public void MoveTo(Vector2 worldPos)
    {
        OwnerParent.Position = worldPos;
    }


    public void OnMouseClick()
    {
        EmitSignalMouseClick(this);
    }

    public void OnDragStart()
    {
        EmitSignalDragStart(this);
    }

    public void OnDragEnd()
    {
        EmitSignalDragEnd(this);
    }

    public void Accept()
    {
        AnswerReceived = true;
        EmitSignalAccepted(this);
    }

    public void Decline()
    {
        AnswerReceived = true;
        EmitSignalDeclined(this);
    }

    public void Reset()
    {
        AnswerReceived = false;
    }

}
