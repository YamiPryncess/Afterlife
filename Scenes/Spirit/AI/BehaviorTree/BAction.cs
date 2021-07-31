using Godot;
using System.Collections.Generic;

public class BAction : BNode {
    public EVENT stage {set; get;}
    public STATE actState {set; get;} = STATE.NULL;
    public StateMachine curState {set; get;}
    public BAction() {}
    public BAction(Spirit _self, STATE _actState) : base(_self, BTYPE.ACTION) {
        actState = _actState;
    }
    public BSIGNAL aProcess() {
        curState = self.currentState;
        if(curState.name != actState) {//Prior node should return success
            curState.cancel();//on frame end. If it doesn't then we Yeet it.
            curState.setNextState(curState.enumToState(actState));
        }
        curState = curState.process(self.fDelta);//BAction only processes its own state.
        return curState != null ? curState.sSignal : BSIGNAL.FAIL;
    }//Decorator still has to be added in.
}
// public BAction(KinematicBody _self, BTYPE _bType, Tick pm) : base(_self, _bType) {
//         ProcessMethod = pm;
//     }
//     public delegate BSIGNAL Tick();
//     public Tick ProcessMethod;
//     public override BSIGNAL btProcess(float delta, State state) {   
//         if(ProcessMethod != null) {
//             return ProcessMethod();
//         } return BSIGNAL.FAIL;
//     }
//     public BSIGNAL getASignal() {
//         return aSignal;
//     }
//     public void setASignal(BSIGNAL _aSignal) {
//         aSignal =_aSignal;
//     }
//     public virtual void Enter() { stage = EVENT.UPDATE; setASignal(BSIGNAL.RUNNING); }
//     public virtual void Update() { stage = EVENT.UPDATE; }
//     public virtual void Exit() { stage = EVENT.EXIT; } 
//     public BSIGNAL processAction() {
//         if (stage == EVENT.ENTER) Enter(); 
// 	    if (stage == EVENT.UPDATE) Update();
// 	    aSignal = base.processDecorate(aSignal);
//         decideExit();

//         if (stage == EVENT.EXIT) Exit();
//         return getASignal();
//     }
//     public virtual BSIGNAL startAction(State state) {
//         return BSIGNAL.STANDBY;
//     }
//     public void decideExit() {
//         if(aSignal != BSIGNAL.RUNNING) {
//             stage = EVENT.EXIT;
//         }
//     }
// }