// using Godot;
// using System.Collections.Generic;

// public class Approach : BAction {
//     public Vector3 velocity {set; get;} = new Vector3();
//     public KinematicBody target {set; get;}
//     public Approach () {}
//     public Approach (KinematicBody _self,
//                     int _priorityLvl = 0) : base(_self, BTYPE.ACTION) {
//     }
//     public override BSIGNAL startAction(State state) {//Wanted to startAction() with approach but how would you get the arguments for each action...
//         target = state.target;
//         return base.processAction();
//     }
//     public override void Enter() {
//         base.Enter();
        
//     }
//     public override void Update() {
//         base.Update();
//         Vector3 targetOrigin = target.GlobalTransform.origin;
//         Vector3 selfOrigin = base.self.GlobalTransform.origin;
//         velocity = targetOrigin - selfOrigin;
//         base.self.MoveAndSlide(velocity, Vector3.Up);//Needs nav mesh
//         if(selfOrigin.DistanceTo(targetOrigin) < 1) {
//             setASignal(BSIGNAL.PASS);
//         } else if(selfOrigin.DistanceTo(targetOrigin) > 20) {
//             setASignal(BSIGNAL.FAIL);
//         } else {
//             setASignal(BSIGNAL.RUNNING);
//         }
//     }
//     public override void Exit() {
//         base.Exit();
//     }
// }