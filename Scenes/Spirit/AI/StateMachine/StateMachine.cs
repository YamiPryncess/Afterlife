using Godot;
using System.Collections.Generic;

public class StateMachine {
    public STAGE stage {set; get;}
    public State currentState {set; get;}
    public State nextState {set; get;}
    public Spirit self {set; get;}
    public AnimationPlayer animator {set; get;}
    public bool finalFrame {set; get;} = false;
    public bool enforceUpdate {set; get;} = false;
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
        //currentState.process() runs either (1)enter->update, (2)update, (3)update->exit, or (4)exit alone.
        switch(currentState.exited) {
            case true: //if true then Previous frame (3) both updated and exited.
                restate();
                currentState.process(delta); //(1) updating a new state
            break;
            case false:
                if(currentState.stage == STAGE.EXIT) {
                    currentState.process(delta);//(4)exiting currentState without update.
                    if(currentState.exited) {
                        restate();
                        currentState.process(delta);//(1)make sure 1 update still happens for the frame.
                    }
                } else {
                    currentState.process(delta); //(1)/(2)/(3) updates alone or with entry or exit. 
                }
            break;
        }
    }
    public void restate() {
        if(nextState == null) nextState = new Idle(this);
        currentState = nextState;
        nextState = null;
    }
    public void cancel() { finalFrame = true; animator.Stop(true); }
}