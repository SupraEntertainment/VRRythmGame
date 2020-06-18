﻿using UnityEngine;
using Valve.VR;

public class SteamInputModule : VRInputModule
{
    
    public SteamVR_Input_Sources[] m_Sources = new SteamVR_Input_Sources[]{};
    public SteamVR_Action_Boolean m_Click = null;
    public SteamVR_Action_Vector2 m_Scroll = null;

    private int currentPointerId = 0;
    

    
    public override void Process() {
        base.Process();

        for (var i = 0; i < m_Sources.Length; i++) {
            // Press
            if (m_Click.GetStateDown(m_Sources[i])) {
                if (currentPointerId == i) {
                    Press();
                    Drag();
                } else {
                    SetPointer(i);
                    currentPointerId = i;
                }
            }

            // Release
            if (m_Click.GetStateUp(m_Sources[i])) {
                Release();
            }
            
            // Scroll
            if (m_Scroll.changed) {
                Debug.Log("Scrolled " + m_Scroll.axis);
                Data.scrollDelta = m_Scroll.axis * 20;
                Scroll();
            }
        }
    }
    
}