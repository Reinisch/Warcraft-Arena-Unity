using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicRpgEngine.Characters
{
    class RollingAverage
    {
        float[] sampleValues;
        int sampleCount;
        float valueSum;
        int currentPosition;

        public RollingAverage(int sampleCount)
        {
            sampleValues = new float[sampleCount];
        }

        public void AddValue(float newValue)
        {
            valueSum -= sampleValues[currentPosition];
            valueSum += newValue;

            sampleValues[currentPosition] = newValue;
            currentPosition++;

            if (currentPosition > sampleCount)
                sampleCount = currentPosition;

            if (currentPosition >= sampleValues.Length)
            {
                currentPosition = 0;
                valueSum = 0;

                foreach (float value in sampleValues)
                    valueSum += value;
            }
        }
        public float AverageValue
        {
            get
            {
                if (sampleCount == 0)
                    return 0;
                
                return valueSum / sampleCount;
            }
        }
    }
}