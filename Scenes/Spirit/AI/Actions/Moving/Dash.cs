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
        sm.enforceUpdate = true;
        if(self.move.inputDir != Vector3.Zero) rotDir = self.move.inputDir;
        else rotDir = -self.GlobalTransform.basis.z;
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(-self.GlobalTransform.basis.z, rotDir, 35f, 35f);
        if(seconds > endTime || (sm.nextState != null 
            && sm.nextState.name == STATE.AIR)) sm.finalFrame = true; //Unsure for how to better check if in air when frames are enforced
    }
    public override void Exit() {
        base.Exit();
    }
}