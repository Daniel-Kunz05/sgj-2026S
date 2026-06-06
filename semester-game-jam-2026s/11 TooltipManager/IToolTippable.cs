using Godot;

public interface IToolTippable
{
    void SetupToolTipConnection()
    {
        if (this is Control c)
        {
            c.MouseEntered += () => TooltipManager.instance?.ShowToolTip(c, ToolTipText);
            c.MouseExited += () => TooltipManager.instance?.HideToolTip(c);
        } else if (this is ModuleBody moduleBody)
        {
            Area2D area2D = (Area2D)moduleBody.FindChild("Area2D");

            area2D.MouseEntered  += () => TooltipManager.instance?.ShowToolTip(area2D, ToolTipText);
            area2D.MouseExited += () => TooltipManager.instance?.HideToolTip(area2D);
        }
        else
        {
            GD.PrintErr("Tooltippable is not a compatible type");
        }
    }

    string ToolTipText { get; }
}