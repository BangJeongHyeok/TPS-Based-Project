using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TrapActivateStyle
{
    Activate,
    Deactivate,
    Reset,
    Toggle,
}

public class Trap : MonoBehaviour
{
    private bool isOnce = false;
    private bool OnceCount = false;

    private bool isactive = false;
    public bool isActive
    {
        get
        {
            return isactive;
        }
        private set
        {
            if (isactive != value)
            {
                if (!isOnce || (isOnce && !OnceCount))
                {
                    if (isOnce)
                        OnceCount = true;

                    isactive = value;

                    if (value)
                        ActivateTrap();
                    else
                        DeActivateTrap();
                }

            }
        }
    }


    protected UnityEvent TrapEvent_Activate = new UnityEvent();
    protected UnityEvent TrapEvent_DeActivate = new UnityEvent();



    protected void TrapSetting(bool active, bool isonce = false, UnityAction ActivateEvent = null, UnityAction DeactivateEvent = null)//우선순위 이벤트
    {
        if(ActivateEvent != null)
        TrapEvent_Activate.AddListener(()=> ActivateEvent());
        if(DeactivateEvent != null)
        TrapEvent_DeActivate.AddListener(() => DeactivateEvent());
        isActive = active;
        isOnce = isonce;
    }

    public void SetActiveState(TrapActivateStyle activeStyle)//그냥 한번 더 감싼 느낌..?
    {
        switch (activeStyle)
        {
            case TrapActivateStyle.Activate:
                isActive = true;
                break;
            case TrapActivateStyle.Deactivate:
                isActive = false;
                break;
            case TrapActivateStyle.Reset:
                isActive = false;
                isActive = true;
                break;
            case TrapActivateStyle.Toggle:
                isActive = !isActive;
                break;
        }

    }

    void ActivateTrap()
    {
        if (TrapEvent_Activate != null)
        {
            TrapEvent_Activate.Invoke();
        }
    }

    void DeActivateTrap()
    {
        if (TrapEvent_DeActivate != null)
        {
            TrapEvent_DeActivate.Invoke();
        }
    }
}
