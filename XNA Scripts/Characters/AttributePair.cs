using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicRpgEngine.Characters
{
    [Serializable]
    public class AttributePair
    {
        int currentValue;
        int maximumValue;

        public int CurrentValue
        {
            get { return currentValue; }
        }
        public int MaximumValue
        {
            get { return maximumValue; }
        }
        public static AttributePair Zero
        {
            get { return new AttributePair(); }
        }

        private AttributePair()
        {
            currentValue = 0;
            maximumValue = 0;
        }
        public AttributePair(int maxValue)
        {
            currentValue = maxValue;
            maximumValue = maxValue;
        }
        public AttributePair(int curValue, int maxValue)
        {
            currentValue = curValue;
            maximumValue = maxValue;
        }

        public void Increase(ushort value)
        {
            currentValue += value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
        }
        public void Decrease(ushort value)
        {
            currentValue -= value;
            if (currentValue < 0)
                currentValue = 0;
        }
        public void SetCurrent(ushort value)
        {
            currentValue = value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
        }
        public void SetMaximum(ushort value)
        {
            maximumValue = value;
            if (currentValue > maximumValue)
                currentValue = maximumValue;
        }
    }
}
