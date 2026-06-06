using Godot;

public interface IToolTippable
{
    void SetupToolTipConnection()
    {
        if (this is Control c)
        {
            c.MouseEntered += () => TooltipManager.instance?.ShowToolTip(ToolTipText);
            c.MouseExited += () => TooltipManager.instance?.HideToolTip();
        } else if (this is ModuleBody moduleBody)
        {
            Area2D area2D = (Area2D)moduleBody.FindChild("Area2D");

            area2D.MouseEntered  += () => TooltipManager.instance?.ShowToolTip(ToolTipText);
            area2D.MouseExited += () => TooltipManager.instance?.HideToolTip();
        }
        else
        {
            GD.PrintErr("Tooltippable is not a compatible type");
        }
    }

    string ToolTipText { get; }
}