using Godot;
public class Navigate : State {
    Navigation navMesh;
    Vector3 targetOrigin;
    Vector3 myOrigin;
    float navDelta = 0;
    float deltaLimit = .05f;//0.0825f;//.165f;
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
    }
    public bool pathExists() {
        if(reality.path != null && reality.path.Length > 0) {
            return true;
        }
        return false;
    }
    public bool reachedPoint() {
        if(pathExists()) {
            if(new Vector3(self.GlobalTransform.origin.x, 0, self.GlobalTransform.origin.z)
                .DistanceTo(new Vector3(reality.path[reality.pathInx].x, 0, reality.path[reality.pathInx].z)) < 2) {
                    return true;
            }
        } return false;
    }
    public Vector3 pointDir() {
        if(pathExists()) {
            return (reality.path[reality.pathInx] - self.GlobalTransform.origin).Normalized();
        }
        return Vector3.Zero;
    }
    public bool beforeFinal() {
        if(pathExists()) {
            if(reality.pathInx < reality.path.Length-1) {
                return true;
            }
        }
        return false;
    }
    public bool finalPoint() {
        if(pathExists()) {
            if(reality.pathInx >= reality.path.Length-1) {
                return true;
            }
        } 
        return false;
    }
    public bool targetMoved() {
        if(targetOrigin != null) {
            if(targetOrigin.DistanceTo(reality.target.GlobalTransform.origin) > 1) {
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
                if(((!pathExists() || targetMoved()) & sufficeTime(navDelta, deltaLimit)) || 
                    (reached & finalPoint())) {
                    refreshNav();//Refresh or start navmesh
                }
                if(beforeFinal() && reachedPoint()) {//Reached point and still not at the end of the path array.
                        reality.pathInx++;
                }
                //GD.Print(reality.path.Length, " ", reality.pathInx);
                self.move.updateDirection(pointDir() * 16);
            }    
            navDelta = count(navDelta, deltaLimit);
        }
    }
    public override void Exit() {
        reality.path = null;
        reality.pathInx = 0;
        //reality.target = null; //Not sure how I'll handle target management yet.
        base.Exit();
    }
}

// self.inputDir = (targetOrigin - myOrigin).Normalized();
                // self.velocity = self.inputDir * self.speed;