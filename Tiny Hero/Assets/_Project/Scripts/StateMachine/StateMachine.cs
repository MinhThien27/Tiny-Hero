using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
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

    public void ReplaceTransition(Type oldType, IState newState)
    {
        var newNode = GetOrAddNode(newState);

        var updatedNodes = new Dictionary<IState, StateNode>();

        foreach (var kvp in nodes)
        {
            var node = kvp.Value;
            var updatedTransitions = new HashSet<ITransition>();

            foreach (var transition in node.Transitions)
            {
                var targetState = transition.To;

                if (transition.To.GetType() == oldType)
                {
                    updatedTransitions.Add(new Transition(newState, transition.Condition));
                }
                else
                {
                    updatedTransitions.Add(transition);
                }
            }

            node.Transitions.Clear();
            foreach (var updated in updatedTransitions)
            {
                node.Transitions.Add(updated);
            }

            updatedNodes[kvp.Key] = node;
        }

        foreach (var key in nodes.Keys)
        {
            if (key.GetType() == oldType)
            {
                nodes.Remove(key);
                break;
            }
        }

        nodes[newState] = newNode;

        var updatedAny = new HashSet<ITransition>();
        foreach (var t in anyTransitions)
        {
            if (t.To.GetType() == oldType)
            {
                updatedAny.Add(new Transition(newState, t.Condition));
            }
            else
            {
                updatedAny.Add(t);
            }
        }

        anyTransitions = updatedAny;
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