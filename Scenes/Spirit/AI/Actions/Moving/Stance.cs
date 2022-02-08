using Godot;
using System.Collections.Generic;

public class Stance : State {
    Vector3 rotDir;
    public Stance(StateMachine _parent) : base(_parent) {
        name = STATE.STANCE;
    }
    public override void Enter() {
        base.Enter();
        if(self.move.inputDir != Vector3.Zero) rotDir = self.move.inputDir;
        else rotDir = -self.GlobalTransform.basis.z;
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, rotDir, 8f, 16f);
    }
    public override void Exit() {
        base.Exit();
    }
}