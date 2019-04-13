using System;

namespace RxDivideAggregate
{
    public class FireId : IEquatable<FireId>
    {
        public FireId(long fire)
        {
            LongFireId = fire;
        }
        public long LongFireId { get; }

        public bool Equals(FireId other)
        {
            if (other == null) return false;
            return other.LongFireId == LongFireId;
        }

        public override bool Equals(object obj)
        {
            var typedObj = obj as FireId;
            if (typedObj == null)
            {
                return false;
            }
            return Equals(typedObj);
        }
        
        public override int GetHashCode()
        {
            return this.LongFireId.GetHashCode();
        }
    }
}
