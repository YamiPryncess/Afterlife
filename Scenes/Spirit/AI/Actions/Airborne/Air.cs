using Godot;
using System.Collections.Generic;

public class Air : State {
    public Air(StateMachine _parent) : base(_parent) {
        name = STATE.AIR;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 8f, 16f, 1);
        if(self.IsOnFloor()){ //Switch out when godot physics says its on floor but enter air state when custom area says it's not on floor.
            self.events[MECHEVENT.LANDED].validate(self);
        }
    }
    public override void Exit() {
        base.Exit();
    }
}