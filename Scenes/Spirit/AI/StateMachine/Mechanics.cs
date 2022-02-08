using Godot;
using System.Collections.Generic;
public class Mechanics : Object {
    public Mechanics() {} //Methods like basics and alexia should not share event names. Updated: Old idea, all characters will share mechanics.
    public void basics(Spirit s) {//First Condition can not repeat for an event.
        //General
        s.events.Add(MECHEVENT.ANIMEND, new Event(MECHEVENT.ANIMEND, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.ANIMEND].addCondition(STATE.ATTACK, STATE.WALK);
            s.events[MECHEVENT.ANIMEND].addCondition(STATE.SLIDE, STATE.WALK);
            s.events[MECHEVENT.ANIMEND].addCondition(STATE.DASH, STATE.WALK);

        //Movement
        s.events.Add(MECHEVENT.DASHPRESS, new Event(MECHEVENT.DASHPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.DASHPRESS].addCondition(STATE.WALK, STATE.DASH);
            s.events[MECHEVENT.DASHPRESS].addCondition(STATE.BREAK, STATE.DASH);
            s.events[MECHEVENT.DASHPRESS].addCondition(STATE.RUN, STATE.DASH);

            s.events[MECHEVENT.DASHPRESS].addCondition(STATE.SNEAK, STATE.SLIDE);
        
        s.events.Add(MECHEVENT.CROUCHPRESS, new Event(MECHEVENT.CROUCHPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.CROUCHPRESS].addCondition(STATE.WALK, STATE.SNEAK);
            s.events[MECHEVENT.CROUCHPRESS].addCondition(STATE.SNEAK, STATE.WALK);
            //s.events[MECHEVENT.CROUCHPRESS].addCondition(STATE.DASH, STATE.???); //What should happen if you press crouch while running?
        
        //Attack
        s.events.Add(MECHEVENT.ATTACKPRESS, new Event(MECHEVENT.ATTACKPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.ATTACKPRESS].addCondition(STATE.WALK, STATE.ATTACK);
            s.events[MECHEVENT.ATTACKPRESS].addCondition(STATE.ATTACK, STATE.ATTACK);

        //Stance (Requires its own move controller event "STANCEDMOVE", since move must happen with stance held down.)
        s.events.Add(MECHEVENT.STANCEPRESS, new Event(MECHEVENT.STANCEPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.WALK, STATE.STANCE);
            //s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.RUN, STATE.BREAK); //Not Ready and Unsure of Mech

        s.events.Add(MECHEVENT.STANCERELEASE, new Event(MECHEVENT.STANCERELEASE, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.STANCERELEASE].addCondition(STATE.BREAK, STATE.WALK);
            s.events[MECHEVENT.STANCERELEASE].addCondition(STATE.STANCE, STATE.WALK);

        //Jump
        s.events.Add(MECHEVENT.JUMPPRESS, new Event(MECHEVENT.JUMPPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.WALK, STATE.JUMP);
            //s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.AIR, STATE.JUMP); //Why did I do this?

            s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.FLOAT, STATE.SKIP);

        s.events.Add(MECHEVENT.AIR, new Event(MECHEVENT.AIR, new Dictionary<STATE, STATE>()));
            //s.events[MECHEVENT.AIR].addCondition(STATE.IDLE, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.JUMP, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.WALK, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.RUN, STATE.AIR);
            //s.events[MECHEVENT.AIR].addCondition(STATE.CROUCH, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.SNEAK, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.STANCE, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.SLIDE, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.DASH, STATE.AIR);
            s.events[MECHEVENT.AIR].addCondition(STATE.PHASE, STATE.FLOAT);

        s.events.Add(MECHEVENT.LANDED, new Event(MECHEVENT.LANDED, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.LANDED].addCondition(STATE.JUMP, STATE.WALK);
            s.events[MECHEVENT.LANDED].addCondition(STATE.AIR, STATE.WALK);

        s.events.Add(MECHEVENT.FAILED, new Event(MECHEVENT.FAILED, new Dictionary<STATE, STATE>())); //Tired Goes to floor or move
            s.events[MECHEVENT.FAILED].addCondition(STATE.JUMP, STATE.WALK);

        // s.events.Add(MECHEVENT.DROP, new Event(MECHEVENT.DROP, new Dictionary<STATE, STATE>())); //Air State replaced Drop
        //     s.events[MECHEVENT.DROP].addCondition(STATE.JUMP, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.IDLE, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.WALK, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.RUN, STATE.DROP);
        
        //     s.events[MECHEVENT.DROP].addCondition(STATE.PHASE, STATE.FLOAT);

        //Phase
        s.events.Add(MECHEVENT.PHASEPRESS, new Event(MECHEVENT.PHASEPRESS, new Dictionary<STATE, STATE>()));
            s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.ANY, STATE.PHASE); //Phase may run while buffered then enters OR it cancels. Not sure.
    }
}
public enum MECHEVENT {
    ANIMEND, AIR,
    STANCEPRESS, STANCERELEASE, STANCEDMOVE, 
    ATTACKPRESS, STANCEDATTACK, LANDED, MOVINGLANDED,
    CROUCHPRESS, DASHPRESS, JUMPPRESS, DROP, 
    FLOAT, PHASEPRESS, NONE, FAILED
}

// s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.IDLE, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.WALK, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.DASH, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.RUN, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.ATTACK, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.JUMP, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.DROP, STATE.PHASE);