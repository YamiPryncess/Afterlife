using Godot;
using System.Collections.Generic;

public class Sneak : State {
    Movement move;
    float speed = 4f;
    float maxSpeed = 10f;
    public Sneak(StateMachine _parent) : base(_parent) {
        name = STATE.SNEAK;
    }
    public override void Enter() {
        base.Enter();
         move = self.move;
    }
    public override void Update() {
        base.Update();
        self.move.calcMove(self.move.inputDir, self.move.inputDir, 4f, 8f);
    }
    public override void Exit() {
        base.Exit();
    }
}