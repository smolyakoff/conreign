using System;
using MediatR;

namespace Conreign.Core.Game
{

    public class StatefulAction<TState, TAction, TResult> : IAsyncRequest<TResult>
    {
        public StatefulAction(TAction action, TState state)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            Action = action;
            State = state;
        }

        public TAction Action { get; }
        
        public TState State { get; }
    }

    public interface IStatefulActionHandler<TState, TAction, TResult> :
        IAsyncRequestHandler<StatefulAction<TState, TAction, TResult>, TResult>
    {
    }
}