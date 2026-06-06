using Godot;

public partial class DropReceiver : Area2D
{
    
    [Signal] public delegate void ReceivedDraggableEventHandler(DropReceiver me, Draggable draggable);

    public void Receive(Draggable draggable)
    {
        EmitSignalReceivedDraggable(this, draggable);
    }
}