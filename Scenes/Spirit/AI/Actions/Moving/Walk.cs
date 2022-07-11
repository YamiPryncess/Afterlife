using Godot;
using System.Collections.Generic;

public class Walk : State {
    public Walk(StateMachine _parent) : base(_parent) {
        name = STATE.WALK;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
        //GD.Print(animator.GetAnimationList());
        if(animator.AssignedAnimation != "Punch"){
            if(self.move.inputDir == Vector3.Zero) animator.Play("Idle");
            else animator.Play("Walk");
        }
    }
    public override void Exit() {
        base.Exit();
    }
}