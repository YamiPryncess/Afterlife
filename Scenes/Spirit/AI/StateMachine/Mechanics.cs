using Godot;
using System.Collections.Generic;
public class Mechanics : Object {
    public Mechanics() {} //Methods like basics and alexia should not share event names. Updated: Old idea, all characters will share mechanics.
    public void motion(Dictionary<MECHEVENT, Event> events) {//First Condition can not repeat for an event.
        STATETYPE type = STATETYPE.MOTION;
        //General
        events.Add(MECHEVENT.ANIMEND, new Event(MECHEVENT.ANIMEND, type));
            events[MECHEVENT.ANIMEND].addCondition(STATE.SLIDE, STATE.WALK);
            events[MECHEVENT.ANIMEND].addCondition(STATE.DASH, STATE.WALK);

        //Movement
        events.Add(MECHEVENT.DASHPRESS, new Event(MECHEVENT.DASHPRESS, type));
            events[MECHEVENT.DASHPRESS].addCondition(STATE.WALK, STATE.DASH);
            events[MECHEVENT.DASHPRESS].addCondition(STATE.BREAK, STATE.DASH);
            events[MECHEVENT.DASHPRESS].addCondition(STATE.RUN, STATE.DASH);

            events[MECHEVENT.DASHPRESS].addCondition(STATE.SNEAK, STATE.SLIDE);
        
        events.Add(MECHEVENT.CROUCHPRESS, new Event(MECHEVENT.CROUCHPRESS, type));
            events[MECHEVENT.CROUCHPRESS].addCondition(STATE.WALK, STATE.SNEAK);
            events[MECHEVENT.CROUCHPRESS].addCondition(STATE.SNEAK, STATE.WALK);
            //s.events[MECHEVENT.CROUCHPRESS].addCondition(STATE.DASH, STATE.???); //What should happen if you press crouch while running?

        //Stance (Requires its own move controller event "STANCEDMOVE", since move must happen with stance held down.)
        events.Add(MECHEVENT.STANCEPRESS, new Event(MECHEVENT.STANCEPRESS, type));
            events[MECHEVENT.STANCEPRESS].addCondition(STATE.WALK, STATE.STANCE);
            //s.events[MECHEVENT.STANCEPRESS].addCondition(STATE.RUN, STATE.BREAK); //Not Ready and Unsure of Mech

        events.Add(MECHEVENT.STANCERELEASE, new Event(MECHEVENT.STANCERELEASE, type));
            events[MECHEVENT.STANCERELEASE].addCondition(STATE.BREAK, STATE.WALK);
            events[MECHEVENT.STANCERELEASE].addCondition(STATE.STANCE, STATE.WALK);

        //Jump
        events.Add(MECHEVENT.JUMPPRESS, new Event(MECHEVENT.JUMPPRESS, type));
            events[MECHEVENT.JUMPPRESS].addCondition(STATE.WALK, STATE.JUMP);
            //s.events[MECHEVENT.JUMPPRESS].addCondition(STATE.AIR, STATE.JUMP); //Why did I do this?

            events[MECHEVENT.JUMPPRESS].addCondition(STATE.FLOAT, STATE.SKIP);

        events.Add(MECHEVENT.AIR, new Event(MECHEVENT.AIR, type));
            //s.events[MECHEVENT.AIR].addCondition(STATE.IDLE, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.JUMP, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.WALK, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.RUN, STATE.AIR);
            //s.events[MECHEVENT.AIR].addCondition(STATE.CROUCH, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.SNEAK, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.STANCE, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.SLIDE, STATE.AIR);
            events[MECHEVENT.AIR].addCondition(STATE.DASH, STATE.AIR);

        events.Add(MECHEVENT.LANDED, new Event(MECHEVENT.LANDED, type));
            events[MECHEVENT.LANDED].addCondition(STATE.JUMP, STATE.WALK);
            events[MECHEVENT.LANDED].addCondition(STATE.AIR, STATE.WALK);

        events.Add(MECHEVENT.FAILED, new Event(MECHEVENT.FAILED, type)); //Tired Goes to floor or move
            events[MECHEVENT.FAILED].addCondition(STATE.JUMP, STATE.WALK);

        // s.events.Add(MECHEVENT.DROP, new Event(MECHEVENT.DROP, new Dictionary<STATE, STATE>())); //Air State replaced Drop
        //     s.events[MECHEVENT.DROP].addCondition(STATE.JUMP, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.IDLE, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.WALK, STATE.DROP);
        //     s.events[MECHEVENT.DROP].addCondition(STATE.RUN, STATE.DROP);
        
        //     s.events[MECHEVENT.DROP].addCondition(STATE.PHASE, STATE.FLOAT);
    }

    public void action(Dictionary<MECHEVENT, Event> events) {
        STATETYPE type = STATETYPE.ACTION;
        events.Add(MECHEVENT.ANIMEND, new Event(MECHEVENT.ANIMEND, type));
            events[MECHEVENT.ANIMEND].addCondition(STATE.ATTACK, STATE.RESERVED);

        events.Add(MECHEVENT.ATTACKPRESS, new Event(MECHEVENT.ATTACKPRESS, type));
            events[MECHEVENT.ATTACKPRESS].addCondition(STATE.RESERVED, STATE.ATTACK);
            events[MECHEVENT.ATTACKPRESS].addCondition(STATE.ATTACK, STATE.ATTACK);

    }

    public void phase(Dictionary<MECHEVENT, Event> events) {
        STATETYPE type = STATETYPE.PHASE;
        //Phase
        events.Add(MECHEVENT.PHASE, new Event(MECHEVENT.PHASE, type));
            events[MECHEVENT.PHASE].addCondition(STATE.PHYSICAL, STATE.PHASE); //Phase may run while buffered then enters OR it cancels. Not sure.
        events.Add(MECHEVENT.UNPHASE, new Event(MECHEVENT.UNPHASE, type));
            events[MECHEVENT.UNPHASE].addCondition(STATE.PHASE, STATE.PHYSICAL);
            // events[MECHEVENT.UNPHASE].addCondition(STATE.JUMP, STATE.FLOAT);
            // events[MECHEVENT.UNPHASE].addCondition(STATE.AIR, STATE.FLOAT);

        // events.Add(MECHEVENT.AIR, new Event(MECHEVENT.AIR, type));
        //     events[MECHEVENT.AIR].addCondition(STATE.PHASE, STATE.FLOAT);
    }
}
public enum MECHEVENT {
    ANIMEND, AIR,
    STANCEPRESS, STANCERELEASE, STANCEDMOVE, 
    ATTACKPRESS, STANCEDATTACK, LANDED, MOVINGLANDED,
    CROUCHPRESS, DASHPRESS, JUMPPRESS, DROP, 
    FLOAT, PHASE, UNPHASE, NONE, FAILED
}

// s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.IDLE, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.WALK, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.DASH, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.RUN, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.ATTACK, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.JUMP, STATE.PHASE);
//             s.events[MECHEVENT.PHASEPRESS].addCondition(STATE.DROP, STATE.PHASE);