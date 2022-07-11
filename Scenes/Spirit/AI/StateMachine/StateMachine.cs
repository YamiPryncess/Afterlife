using Godot;
using System.Collections.Generic;
using System.Linq;

public class StateMachine {
    public STAGE stage {set; get;}
    public Dictionary<STATETYPE, State> activeStates;
    public State aiState;
    public Dictionary<STATETYPE, Dictionary<MECHEVENT, Event>> mechEvents {set; get;}
    public Spirit self {set; get;}
    public AnimationPlayer animator {set; get;}
    public Dictionary<STATE, int> priority {set; get;} //strictly for input priority
    public StateMachine(Spirit _self, Dictionary<STATETYPE, Dictionary<MECHEVENT,Event>> _mechEvents) {
        self = _self;
        mechEvents = _mechEvents;
        animator = self.GetNode<AnimationPlayer>("Goxidette/AnimationPlayer");
        //animator = self.GetNode<AnimationPlayer>("Animator");
        activeStates = new Dictionary<STATETYPE, State>();
        activeStates[STATETYPE.MOTION] = new Walk(this);
        activeStates[STATETYPE.ACTION] = new Reserved(this);
        activeStates[STATETYPE.PHASE] = new Physical(this);
        aiState = new Reserved(this);

        priority = new Dictionary<STATE, int>();
        //priority.Add(STATE.IDLE, 0);

        priority.Add(STATE.WALK, 1);
        priority.Add(STATE.RUN, 1);

        priority.Add(STATE.ATTACK, 2);
        priority.Add(STATE.DASH, 2);
        priority.Add(STATE.JUMP, 2);
        
        priority.Add(STATE.AIR, 3);
    }
    /*setNextState(State newState) 
    Constantly Called- Called multiple times in one idle process. The most recent prioric input when state finally accepts a change is what matters.
    State Changing- State may or may not (if prevented by enforceUpdate) change for multiple idle processes. 
    Priority- Some inputs will take priority over others. So if an early input has priority over later inputs, all later inputs will be denied.
    Enforce Update- Is set by animationPlayer atm & denies exit into a new state thus allowing for a loose buffer period for new incoming States.
    nextState- newStates will be buffered in the "nextState" var regardless of whether the "currentState.stage" exits or not.*/
    public void setNextState(State newState, State activeState, STATETYPE stateType = STATETYPE.AI) {
        if(activeState.next == null || isPrioric(newState, activeState.next)) {
        if(!activeState.locked && activeState.stage != STAGE.ENTER) activeState.stage = STAGE.EXIT; 
        activeState.next = newState;
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
    public bool isPrioric(State newState, State queuedState) {
        if((priority.ContainsKey(newState.name) && priority.ContainsKey(queuedState.name)) 
            && (priority[newState.name] > priority[queuedState.name])) {
            return true;                                       
        }
        return false;
    }
    public State enumToState(STATE newState) {
        switch (newState) {
            case STATE.WALK:
                return new Walk(this);
            case STATE.WAIT:
                return new Wait(this);
            case STATE.ATTACK:
                return new Attack(this);
            case STATE.DASH:
                return new Dash(this);
            case STATE.RUN:
                return new Run(this);
            case STATE.JUMP:
                return new Jump(this);
            case STATE.AIR:
                return new Air(this);
            case STATE.DROP:
                return new Drop(this);
            case STATE.LEAP:
                return new Leap(this);
            case STATE.STANCE:
                return new Stance(this);
            case STATE.BREAK:
                return new Break(this);
            case STATE.FLOAT:
                return new Float(this);
            case STATE.SKIP:
                return new Skip(this);
            case STATE.SLIDE:
                return new Slide(this);
            case STATE.SNEAK:
                return new Sneak(this);
            case STATE.NAVIGATE:
                return new Navigate(this);
            case STATE.NULL:
                return null;
            default:
                return null;
      }
    }
    public void process(float delta) {
        Dictionary<STATETYPE, State> newStates = new Dictionary<STATETYPE, State>();
        foreach(KeyValuePair<STATETYPE, State> state in activeStates){
            // if(self.player == 1 && state.Key == STATETYPE.ACTION) GD.Print("Current:", state.Value.name, 
            // "| Stage: ", state.Value.stage, " Attack: ", self.GetNode<Catalyst>("Catalyst").state);
            //GD.Print("Running: ", state.Value.stateType, " Current:", state.Value.name, " Next:", state.Value.next.name);
            newStates.Add(state.Key, frameTransitions(state.Value, state.Key, delta));
        }
        activeStates = newStates;
    }
    public void aiProcess(float delta) {
        aiState = frameTransitions(aiState, STATETYPE.AI, delta);
    }
    public State frameTransitions(State state, STATETYPE type, float delta) {
        //currentState.process() runs either (1)enter->update, (2)update, (3)update->exit, or (4)exit alone.
        if(state.exited) {//if true then Previous frame (3) both updated and exited.
                state = restate(state, type);
                state.process(delta, type); //(1) updating a new state
        } else if(!state.exited && state.stage == STAGE.EXIT) {
            state.process(delta, type);//(4)exiting currentState without update.
            if(state.exited) {//If it doesn't exit then don't restate. (Forgot why)
                state = restate(state, type);
                state.process(delta, type);//(1)make sure 1 update still happens for the frame.
            }
        } else if(!state.exited) {
            state.process(delta, type); //(1)/(2)/(3) updates alone or with entry or exit. 
        }
        return state;
    }
    public State restate(State state, STATETYPE type) {
        if(state.next == null) state.next = new Reserved(this); //Prevents error but bypasses state transition logic. Could I return; instead? Unsure.
        return state.next;
    }
    //Auxiliary Event Methods
    public void runEvent(MECHEVENT newEvent, STATETYPE stateType = STATETYPE.ALL) {
        if(stateType == STATETYPE.ALL) {
            foreach (KeyValuePair<STATETYPE, Dictionary<MECHEVENT, Event>> eventList in mechEvents) {
                if(eventList.Value.ContainsKey(newEvent)) {
                    eventList.Value[newEvent].validate(self);
                }
            }
        } else {
            mechEvents[stateType][newEvent].validate(self);
        }
    }
    //Auxiliary State Methods
    public bool nextIsTrue(STATETYPE stateType = STATETYPE.ALL) {
        bool isTrue = false;
        if(stateType == STATETYPE.ALL) {
            foreach(KeyValuePair<STATETYPE, State> state in activeStates){
                if(state.Value.next != null) {
                    isTrue = true;
                }
            }
        } else if(stateType == STATETYPE.AI && aiState.next != null) {
            isTrue = true;
        } else if (activeStates[stateType].next != null) {
            isTrue = true;
        }
        return isTrue;
    }
    public void statesLock(bool setTo, STATETYPE stateType = STATETYPE.ALL) {
        if(stateType == STATETYPE.ALL) {
            foreach(KeyValuePair<STATETYPE, State> state in activeStates){
                state.Value.locked = setTo;    
            }
        } else if(stateType == STATETYPE.AI) {
            aiState.locked = setTo;
        } else {
            activeStates[stateType].locked = setTo;
        }
    }
    public bool framesZero(STATETYPE stateType = STATETYPE.ALL) {
        bool isTrue = false;
        if(stateType == STATETYPE.ALL) {
            foreach(KeyValuePair<STATETYPE, State> state in activeStates){
                if(state.Value.frames <= 0) {
                    isTrue = true;
                }
            }
        } else if(stateType == STATETYPE.AI && aiState.frames <= 0) {
            isTrue = true;
        } else if (activeStates[stateType].frames <= 0) {
            isTrue = true;
        }
        return isTrue;
    }
}
public enum STATE {
    NAVIGATE, NULL, WAIT,
    WALK, DASH, RUN,
    JUMP, DROP, LEAP,
    STANCE, BREAK, PHYSICAL,
    SNEAK, SLIDE, RESERVED,
    PHASE, FLOAT, SKIP, 
    ATTACK, ANY, AIR
}

public enum STATETYPE {
    MOTION, PHASE, ACTION, ALL, ANY, AI
}