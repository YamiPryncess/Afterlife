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
    public Dictionary<STATE, int> priority {set; get;} //strictly for input priority
    public StateMachine(Spirit _self) {
        self = _self;
        animator = self.GetNode<AnimationPlayer>("Animator");
        currentState = new Idle(this);

        priority = new Dictionary<STATE, int>();
        priority.Add(STATE.IDLE, 0);

        priority.Add(STATE.WALK, 1);
        priority.Add(STATE.RUN, 1);

        priority.Add(STATE.ATTACK, 2);
        priority.Add(STATE.DASH, 2);
        priority.Add(STATE.JUMP, 2);
    }
    /*setNextState(State newState) 
    Constantly Called- Called multiple times in one idle process. The most recent prioric input when state finally accepts a change is what matters.
    State Changing- State may or may not (if prevented by enforceUpdate) change for multiple idle processes. 
    Priority- Some inputs will take priority over others. So if an early input has priority over later inputs, all later inputs will be denied.
    Enforce Update- Is set by animationPlayer atm & denies exit into a new state thus allowing for a loose buffer period for new incoming States.
    nextState- newStates will be buffered in the "nextState" var regardless of whether the "currentState.stage" exits or not.*/
    public void setNextState(State newState) { 
        if(nextState == null || isPrioric(newState, nextState)) {
            if(!enforceUpdate) currentState.stage = STAGE.EXIT; 
            nextState = newState;   
            //Can set a buffer frame counter here. Add delta to it and when it's greater than .166667 ms                          
        }
    }
    /*isPrioric()
    Made especially so if a delicate input like a 2 button input,
    for example Stance + Attack is buffered to cause a special attack
    Then Stance is let go of before buffering ends and attack is still held.
    The Stance + Attack special attack can take priority over the Lone Attack.
    Only works when comparing against something that is currently buffered.
    If nothing is buffered, ignore priority and buffer the newState!
    */
    public bool isPrioric(State newState, State nextState) {
        if(priority[newState.name] > priority[nextState.name]) {
            return true;                                       
        }
        return false;
    }
    public State enumToState(STATE newState) {
        switch (newState){
            case STATE.WALK:
                return new Walk(this);
            case STATE.IDLE:
                return new Idle(this);
            case STATE.ATTACK:
                return new Attack(this);
            case STATE.DASH:
                return new Dash(this);
            case STATE.RUN:
                return new Run(this);
            case STATE.JUMP:
                return new Jump(this);
            case STATE.DROP:
                return new Drop(this);
            case STATE.LEAP:
                return new Leap(this);
            case STATE.STANCE:
                return new Stance(this);
            case STATE.BREAK:
                return new Break(this);
            case STATE.NAVIGATE:
                return new Navigate(this);
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
public enum STATE {
    IDLE, NAVIGATE, NULL,
    WALK, DASH, RUN,
    JUMP, DROP, LEAP,
    STANCE, BREAK, 
    ATTACK
}