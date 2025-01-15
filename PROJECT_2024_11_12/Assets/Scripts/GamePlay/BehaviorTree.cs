using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BehaviorTree
{

}

public abstract class BTNode
{
	public enum State { Running, Success, Failure }
	protected State _state;

	public abstract State Execute();

	public State CurrentState => _state;
}


public class Sequence : BTNode
{
	private List<BTNode> _children;
	public Sequence(List<BTNode> children)
	{
		_children = children;
	}

	public override State Execute()
	{
		foreach (var child in _children)
		{
			var childState = child.Execute();
			if (childState == State.Failure)
			{
				_state = State.Failure;
				return _state;
			}
			if (childState == State.Running)
			{
				_state = State.Running;
				return _state;
			}
		}
		_state = State.Success;
		return _state;
	}
}

public class Selector : BTNode
{
	private List<BTNode> _children;

	public Selector(List<BTNode> children)
	{
		_children = children;
	}
	public override State Execute()
	{
		foreach (var child in _children)
		{
			var childState = child.Execute();
			//if (childState == State.Success)
			//{
			//	_state = State.Success;
			//	//return _state;
			//}
			if (childState == State.Running)
			{
				_state = State.Running; 
				return _state;
			}
		}
		_state = State.Failure;
		return _state;
	}
}

public class ActionNode : BTNode
{
	private Func<State> _action;

	public ActionNode(Func<State> action)
	{
		_action = action;
	}

	public override State Execute()
	{
		_state = _action.Invoke();
		return _state;
	}
}

public class ConditionNode : BTNode
{
	private Func<bool> _condition;

	public ConditionNode(Func<bool> condition)
	{
		_condition = condition;
	}

	public override State Execute()
	{
		_state = _condition.Invoke() ? State.Success : State.Failure;
		return _state;
	}
}
