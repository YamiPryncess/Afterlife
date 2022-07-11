using Godot;
using System.Collections.Generic;

public class Dash : State {
    Vector3 rotDir;
    float endTime = .4f;
    public Dash(StateMachine _parent) : base(_parent) {
        name = STATE.DASH;
    }
    public override void Enter() {
        base.Enter();
        locked = true;
        if(self.move.inputDir != Vector3.Zero) rotDir = self.move.inputDir;
        else rotDir = -self.GlobalTransform.basis.z;
    }
    public override void Update() {
        if(seconds > endTime || (next != null 
            && next.name == STATE.AIR)) {//Unsure for how to better check if in air when frames are enforced
                locked = false;
                next = new Walk(sm);
        }
        base.Update();
        self.move.calcMove(-self.GlobalTransform.basis.z, rotDir, 22f, 22f);
    }
    public override void Exit() {
        base.Exit();
    }
}