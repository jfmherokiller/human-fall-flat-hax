using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace plgm_DebugConsole.EasyConsole.FrontEnd.UnityGUI
{
    public class DevConsoleSkin
    {
        public GUISkin myGUISkin = ScriptableObject.CreateInstance<GUISkin>();
        public DevConsoleSkin()
        {

              
            var texture = new Texture2D(12,12);
            texture.LoadImage(skinprefab.backgroundimage);
            myGUISkin.box.normal.background = texture;
            myGUISkin.box.normal.textColor = new Color(0.79999995f, 0.79999995f, 0.79999995f,1f);
            myGUISkin.box.border.top = 6;
            myGUISkin.box.border.bottom = 6;
            myGUISkin.box.border.left = 6;
            myGUISkin.box.border.right = 6;
            myGUISkin.textArea.normal.textColor = new Color(0.9019608f, 0.9019608f,0.9019608f,1f);
            myGUISkin.textField.normal.textColor = new Color(0.79999995f,0.79999995f,0.79999995f,1f);
        }
    }
}
