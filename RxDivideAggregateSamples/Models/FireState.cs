using System;

namespace RxDivideAggregate
{
    public struct FireState
    {
        public FireState(DateTime dateTime, State state)
        {
            DateTime = dateTime;
            State = state;
        }

        public override string ToString()
        {
            return new { State, DateTime }.ToString();
        }

        public readonly DateTime DateTime;
        public readonly State State;
    }

    public enum State
    {
        Increasing = 1,
        Decreasing = 2,
        Stopped    = 3
    }


}
