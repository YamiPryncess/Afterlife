using Godot;
using System.Collections.Generic;
public class Navigate : State {
    public Navigate(StateMachine _parent) : base(_parent) {
        name = STATE.NAVIGATE;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        Spirit myTarget = self.reality.target;
        if(myTarget == null) {
            fail();
        } else {
            Vector3 targetOrigin = myTarget.GlobalTransform.origin;
            Vector3 myOrigin = self.GlobalTransform.origin;
            if(myOrigin.DistanceTo(targetOrigin) < 3) {
                succeed();
            } else {
                self.inputDir = (targetOrigin - myOrigin).Normalized();
                self.velocity = self.inputDir * self.speed;
            }    
        }
    }
    public override void Exit() {
        base.Exit();
    }
}