using Godot;
using System.Collections.Generic;
public class Navigate : State {
    Navigation navMesh;
    Vector3 targetOrigin;
    Vector3 myOrigin;
    float navDelta = 0;
    float deltaLimit = .05f;//0.0825f;//.165f;
    bool navigating = false;
    public Navigate(StateMachine _parent) : base(_parent) {
        name = STATE.NAVIGATE;
        navMesh = (Navigation)self.GetTree().GetNodesInGroup("Nav")[0];
    }
    public override void Enter() {
        base.Enter();
        reality = self.reality;
        targetOrigin = reality.target.GlobalTransform.origin;
        myOrigin = self.GlobalTransform.origin;
    }

    public void refreshNav() {
        targetOrigin = reality.target.GlobalTransform.origin;
        myOrigin = self.GlobalTransform.origin;
        reality.path = navMesh.GetSimplePath(myOrigin, targetOrigin);
        reality.pathInx = 0;
        navDelta = 0;
        navigating = true;
    }
    public bool reachedPoint() {
        if(reality.path != null) {
            if(new Vector3(self.GlobalTransform.origin.x, 0, self.GlobalTransform.origin.z)
                .DistanceTo(new Vector3(reality.path[reality.pathInx].x, 0, reality.path[reality.pathInx].z)) < 2) {
                    return true;
            }
        } return false;
    }
    public bool targetMoved() {
        if(targetOrigin != null) {
            if(targetOrigin.DistanceTo(reality.target.GlobalTransform.origin) > 1 & 
                sufficeTime(navDelta, deltaLimit)) {
                return true;        
            }
        }
        return false;
    }
    public override void Update() {
        base.Update();
        if(reality.target == null) {
            fail();
        } else {
            if(self.GlobalTransform.origin.DistanceTo(reality.target.GlobalTransform.origin) < 3) {//Reached target
                succeed();
            } else {
                bool reached = reachedPoint();
                //1.Compare target's previous to current position if suffice time has pasted to limit refresh
                //2.Check if self reached last path point or 3. Check if not navigating
                if((!navigating) || targetMoved() || (reached & reality.pathInx >= reality.path.Length-1)) {
                    refreshNav();//Refresh or start navmesh
                }
                if(reality.pathInx < reality.path.Length-1 && reachedPoint()) {//Reached point and still not at the end of the path array.
                        reality.pathInx++;
                }
                self.inputDir = (reality.path[reality.pathInx] - self.GlobalTransform.origin).Normalized();
            }    
            navDelta = count(navDelta, deltaLimit);
        }
    }
    public override void Exit() {
        reality.path = null;
        reality.pathInx = 0;
        self.inputDir = Vector3.Zero;
        //reality.target = null; //Not sure how I'll handle target management yet.
        base.Exit();
    }
}

// self.inputDir = (targetOrigin - myOrigin).Normalized();
                // self.velocity = self.inputDir * self.speed;