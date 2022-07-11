using Godot;
using System.Collections.Generic;

public class Air : State {
    public Air(StateMachine _parent) : base(_parent) {
        name = STATE.AIR;
    }
    public override void Enter() {
        base.Enter();
        self.snap = Vector3.Zero;
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 16f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}


    // if(self.IsOnFloor()){ //Switch out when godot physics says its on floor but enter air state when custom area says it's not on floor.
    //     self.events[MECHEVENT.LANDED].validate(self);
    // }