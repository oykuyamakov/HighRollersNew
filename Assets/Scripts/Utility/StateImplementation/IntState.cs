using System;
using UnityEngine;

namespace Utility
{
    public class IntState : State<int>
    {
        public IntState(int def = 0, Action onChange = null, SpecialCondition<int> condition = null) : base(def)
        {
            DefaultValue = def;
            CurrentValue = DefaultValue;
            OnChange = onChange;
            
            AddCondition(condition);
        }

        public void DecreaseOne()
        {
            Change(CurrentValue-1);
        }
        public void IncreaseOne()
        {
            Change(CurrentValue+1);
        }

        public override void CheckConditions()
        {
            for (int i = 0; i < SpecialConditions.Count; i++)
            {
                var cond = SpecialConditions[i];
                switch (cond.Method)
                {
                    case ComparisionMethod.Bigger:
                    {
                        if (cond.ConditionValue < CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                        }
                    }
                        break;
                    case ComparisionMethod.Equal:
                    {
                        if (cond.ConditionValue == CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                        }
                    }
                        break;
                    case ComparisionMethod.Lesser:
                    {
                        if (cond.ConditionValue > CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                        }
                    }
                        break;
                    case ComparisionMethod.BiggerOnce:
                        if (cond.ConditionValue < CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                            SpecialConditions.Remove(cond);
                        }
                        break;
                    case ComparisionMethod.LesserOnce:
                        if (cond.ConditionValue > CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                            SpecialConditions.Remove(cond);
                        }
                        break;
                    case ComparisionMethod.NonEqual:
                        if (cond.ConditionValue != CurrentValue)
                        {
                            cond.OnCondition.Invoke();
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}