using Godot;
using System.Collections.Generic;

public class Master : Node {
    public Behavior behavior = new Behavior();
    public Mechanics mechanics = new Mechanics();
    public Dictionary<STATETYPE, Dictionary<MECHEVENT, Event>> mechEvents {set; get;}
    public Dictionary<MECHEVENT, Event> motionEvents {set; get;} = 
        new Dictionary<MECHEVENT, Event>();
    public Dictionary<MECHEVENT, Event> actionEvents {set; get;} =
        new Dictionary<MECHEVENT, Event>();
    public Dictionary<MECHEVENT, Event> phaseEvents {set; get;} =
        new Dictionary<MECHEVENT, Event>();
    public override void _Ready() {
        mechEvents = new Dictionary<STATETYPE, Dictionary<MECHEVENT, Event>>();
        mechEvents.Add(STATETYPE.MOTION, new Dictionary<MECHEVENT, Event>());
        mechanics.motion(mechEvents[STATETYPE.MOTION]);
        mechEvents.Add(STATETYPE.ACTION, new Dictionary<MECHEVENT, Event>());
        mechanics.action(mechEvents[STATETYPE.ACTION]);
        mechEvents.Add(STATETYPE.PHASE, new Dictionary<MECHEVENT, Event>());
        mechanics.phase(mechEvents[STATETYPE.PHASE]);
    }
    public override void _Process(float delta) {

    }

    public override void _ExitTree() {
        behavior.Free();
        mechanics.Free();
    }
}