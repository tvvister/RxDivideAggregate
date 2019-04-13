using System;
using System.Collections.Generic;
using System.Linq;
using RxDivideAggregate;

namespace RxDivideAggregate
{
    public class FireStates
    {
        public FireStates(IReadOnlyDictionary<FireId, FireState> fireStates)
        {
            if (fireStates == null)
            {
                throw new ArgumentNullException(nameof(fireStates));
            }
            States = fireStates;
        }
        public IReadOnlyDictionary<FireId, FireState> States { get; }


        public override string ToString()
        {
            return String.Join("| ", States.Select(fireInfo => new { FireId = fireInfo.Key.LongFireId, State = fireInfo.Value }));
        }
    }
}
