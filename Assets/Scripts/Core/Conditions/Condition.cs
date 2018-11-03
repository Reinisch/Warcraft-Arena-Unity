using System;

namespace Core
{
    public class Condition
    {
        public ConditionSourceType SourceType { get; set; } // SourceTypeOrReferenceId
        public ConditionTypes ConditionType { get; set; } // ConditionTypeOrReference
        public uint SourceGroup { get; set; }
        public int SourceEntry { get; set; }
        public uint SourceId { get; set; } // So far, only used in CONDITION_SOURCE_TYPE_SMART_EVENT
        public uint ElseGroup { get; set; }
        public uint ConditionValue1 { get; set; }
        public uint ConditionValue2 { get; set; }
        public uint ConditionValue3 { get; set; }
        public uint ErrorType { get; set; }
        public uint ErrorTextId { get; set; }
        public uint ReferenceId { get; set; }
        public uint ScriptId { get; set; }
        public byte ConditionTarget { get; set; }
        public bool NegativeCondition { get; set; }


        public Condition()
        {
            SourceType = ConditionSourceType.None;
            ConditionType = ConditionTypes.None;
            SourceGroup = 0;
            SourceEntry = 0;
            SourceId = 0;
            ElseGroup = 0;
            ConditionTarget = 0;
            ConditionValue1 = 0;
            ConditionValue2 = 0;
            ConditionValue3 = 0;
            ReferenceId = 0;
            ErrorType = 0;
            ErrorTextId = 0;
            ScriptId = 0;
            NegativeCondition = false;
        }

        public bool Meets(ConditionSourceInfo sourceInfo)
        {
            throw new NotImplementedException();
        }

        public uint GetSearcherTypeMaskForCondition()
        {
            throw new NotImplementedException();
        }

        public bool IsLoaded()
        {
            return ConditionType > ConditionTypes.None || ReferenceId > 0;
        }

        public uint GetMaxAvailableConditionTargets()
        {
            throw new NotImplementedException();
        }


        public string ToString(bool ext = false)
        {
            throw new NotImplementedException();
        }
    }
}