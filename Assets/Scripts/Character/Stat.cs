using System.Collections.Generic;

public class Stat
{
    private readonly Dictionary<StatModifierType, float> _modifiers = new Dictionary<StatModifierType, float>();

    private bool _isCurrent;
    private int _finalValue;

    public int FinalValue
    {
        get
        {
            if (_isCurrent)
                return _finalValue;
            else
            {
                _isCurrent = true;
                float result = _modifiers[StatModifierType.Base] * _modifiers[StatModifierType.BaseMultExternal];
                result *= _modifiers[StatModifierType.BaseMult];
                result += _modifiers[StatModifierType.Total];
                result *= _modifiers[StatModifierType.TotalMult];

                _finalValue = (int)result;

                if (_finalValue < 0)
                    _finalValue = 0;

                return _finalValue;
            }
        }
    }
    

    public Stat(int initialValue = 0)
    {
        StatHelper.InitializeStat(_modifiers, initialValue);
    }


    public float this[StatModifierType modType]
    {
        get
        {
            return _modifiers[modType];
        }
        set
        {
            _modifiers[modType] = value;
            _isCurrent = false;
        }
    }
}