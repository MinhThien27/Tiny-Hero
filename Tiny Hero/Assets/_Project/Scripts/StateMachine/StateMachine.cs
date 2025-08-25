using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    private StateNode current;
    public IState CurrentState => current?.State;
    public IState PreviousState { get; private set; }

    // Save with instance, not type, to allow polymorphism
    private Dictionary<IState, StateNode> nodes = new Dictionary<IState, StateNode>();
    private HashSet<ITransition> anyTransitions = new HashSet<ITransition>();

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
        {
            ChangeState(transition.To);
        }

        current?.State?.OnUpdate();
    }

    public void FixedUpdate()
    {
        current?.State?.OnFixedUpdate();
    }

    public void SetState(IState state)
    {
        current = GetOrAddNode(state);
        current.State?.OnEnter();
    }

    public void RevertToPreviousState()
    {
        if (PreviousState != null)
        {
            ChangeState(PreviousState);
        }
    }

    private void ChangeState(IState state)
    {
        if (current != null && current.State == state)
            return; // Không đổi nếu state hiện tại = state mới

        PreviousState = current?.State;

        current?.State?.OnExit();

        current = GetOrAddNode(state);
        current.State?.OnEnter();
    }

    private ITransition GetTransition()
    {
        // Ưu tiên anyTransitions
        foreach (var transition in anyTransitions)
        {
            if (transition.Condition.Evaluate())
            {
                return transition;
            }
        }

        // Kiểm tra transition riêng của state hiện tại
        if (current != null)
        {
            foreach (var transition in current.Transitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }
        }

        return null;
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        var fromNode = GetOrAddNode(from);
        var toNode = GetOrAddNode(to);

        fromNode.AddTransition(toNode.State, condition);
    }

    public void AddAnyTransition(IState to, IPredicate condition)
    {
        anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
    }

    private StateNode GetOrAddNode(IState state)
    {
        if (!nodes.TryGetValue(state, out var node))
        {
            node = new StateNode(state);
            nodes.Add(state, node);
        }

        return node;
    }

    private class StateNode
    {
        public IState State { get; }
        public HashSet<ITransition> Transitions { get; }

        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        public void AddTransition(IState to, IPredicate condition)
        {
            Transitions.Add(new Transition(to, condition));
        }
    }
}








//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using UnityEngine;

//public class StateMachine : MonoBehaviour
//{
//    StateNode current;
//    public IState PreviousState { get; private set; }

//    Dictionary<Type, StateNode> nodes = new();
//    HashSet<Transition> anyTransitions = new();

//    public void Update()
//    {
//        var transition = GetTransition();
//        if(transition != null)
//        {
//            ChangeState(transition.To);
//        }

//        current.State?.OnUpdate();
//    }

//    public void FixedUpdate()
//    {
//        current.State?.OnFixedUpdate();
//    }

//    public void SetState(IState state)
//    {
//        current = nodes[state.GetType()];
//        current.State?.OnEnter();
//    }

//    public void RevertToPreviousState()
//    {
//        if (PreviousState != null)
//        {
//            ChangeState(PreviousState);
//        }
//    }

//    void ChangeState(IState state)
//    {
//        if(current.State?.GetType() == state.GetType())
//            return; // No change needed

//        PreviousState = current.State;
//        var previousState = current.State;
//        var nextState = nodes[state.GetType()].State;

//        previousState?.OnExit();
//        nextState?.OnEnter();
//        current = nodes[state.GetType()];
//    }

//    ITransition GetTransition()
//    {
//        foreach(var transition in anyTransitions)
//        {
//            if(transition.Condition.Evaluate())
//            {
//                return transition;
//            }
//        }

//        foreach (var transition in current.Transitions)
//        {
//            if (transition.Condition.Evaluate())
//            {
//                return transition;
//            }
//        }

//        return null;
//    }

//    public void AddTransition(IState from, IState to, IPredicate condition)
//    {
//        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
//    }

//    public void AddAnyTransition(IState to, IPredicate condition)
//    {
//        anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
//    }

//    StateNode GetOrAddNode(IState state)
//    {
//        var node = nodes.GetValueOrDefault(state.GetType());

//        if(node == null)
//        {
//            node = new StateNode(state);
//            nodes.Add(state.GetType(), node);
//        }

//        return node;
//    }

//    class StateNode
//    {
//        public IState State { get; private set; }
//        public HashSet<ITransition> Transitions { get; }

//        public StateNode(IState state)
//        {
//            State = state;
//            Transitions = new HashSet<ITransition>();
//        }

//        public void AddTransition(IState to, IPredicate condition)
//        {
//            Transitions.Add(new Transition(to, condition));
//        }
//    }
//}