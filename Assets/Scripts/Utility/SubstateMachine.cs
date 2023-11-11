using System;
using System.Collections.Generic;
using UnityEngine;

namespace Helper
{
    public struct SubstateMachine 
    {
        public delegate void MyStates(); 
        private MyStates[] stateActions;
        private Dictionary<string, int> stateMap;

        public SubstateMachine(MyStates[] stateActions)
        {
            this.stateActions = stateActions;
            stateMap = new Dictionary<string, int>();
            for(int i = 0; i < stateActions.Length; i++)
                stateMap.Add(this.stateActions[i].Method.Name, i); 
        }
 
        public MyStates GetState(MyStates state)
        {
            MyStates returnState;
            try{ returnState = stateActions[stateMap[state.Method.Name]]; }
            catch(Exception e) 
            {
                returnState = null;
                Debug.LogException(e);
            } 
            return returnState; 
        }        
    }
}
