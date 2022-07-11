using Godot;
using System;

public class PlayMode : Spatial {
    public override void _Ready() {
        Input.SetMouseMode(Input.MouseMode.Confined);
    }
    public override void _Process(float delta) {
        if (Input.IsActionPressed("ui_cancel")) {
            Input.SetMouseMode(Input.MouseMode.Visible);
        }
        if ((Input.IsActionPressed("leftClick") || 
                Input.IsActionPressed("middleClick") || 
                Input.IsActionPressed("rightClick")) 
                && Input.GetMouseMode() == Input.MouseMode.Visible) {
            Input.SetMouseMode(Input.MouseMode.Confined);
            GetTree().SetInputAsHandled();
        }
        if (Input.IsActionJustPressed("fullscreen")) {
            OS.WindowFullscreen = !OS.WindowFullscreen;
        }
    }
}
