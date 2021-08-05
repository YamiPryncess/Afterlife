using Godot;
using System.Collections.Generic;

public class StateMachine {
    public STAGE stage {set; get;}
    public State currentState {set; get;}
    public State nextState {set; get;}
    public Spirit self {set; get;}
    public AnimationPlayer animator {set; get;}
    public bool updated {set; get;} = false;
    public bool finalFrame {set; get;} = false;
    public bool enforceUpdate {set; get;} = false;
    public BSIGNAL sSignal {set; get;} = BSIGNAL.FAIL;
    public StateMachine(Spirit _self) {
        self = _self;
        animator = self.GetNode<AnimationPlayer>("Animator");
        currentState = new Idle(this);
    }
    public void setNextState(State _nextState) {
        if(!enforceUpdate) currentState.stage = STAGE.EXIT;
        nextState = _nextState;
    }
    public State enumToState(STATE newState) {
        switch (newState){
            case STATE.MOVE:
                return new Move(this);
            case STATE.IDLE:
                return new Idle(this);
            case STATE.ATTACK:
                return new Attack(this);
            case STATE.NULL:
                return null;
            default: 
                return null;
      }
    }
    public void process(float delta) {
        updated = false;
        STAGE stage = currentState.process(delta);
        //runs either enter->update, update, update->exit, or exit alone.
        if(stage == STAGE.EXIT) {
            if(nextState == null) nextState = new Idle(this);
            currentState = nextState;
            nextState = null;
            if(updated == false) {
                stage = currentState.process(delta);
            } 
        }
    }
    public void cancel() { finalFrame = true; animator.Stop(true); }
}