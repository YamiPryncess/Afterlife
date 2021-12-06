using Godot;
using System;

public class Hud : VBoxContainer {
    public Label title {set; get;}
    public ProgressBar healthProgress {set; get;}
    public ProgressBar phaseProgress {set; get;}
    
    public override void _Ready() {
        title = GetNode<Label>("Title");
        healthProgress = GetNode<ProgressBar>("Health/Progress");
        phaseProgress = GetNode<ProgressBar>("Phase/Progress");
    }
}
