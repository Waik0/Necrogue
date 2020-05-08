using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toast.Masterdata.Editor
{
    public class MenuData
    {

        public enum Selected
        {
            None,
            Create,
            LoadJson,
            LoadClass,
        }

        public Selected Select { get; private set; } = Selected.None;
        public void View()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("MasterData Editor");
            if (Button("Create New"))
            {
                Select = Selected.Create;
            }

            if (Button("Load From Json"))
            {
                Select = Selected.LoadJson;
            }

            if (Button("Load From Class"))
            {
                Select = Selected.LoadClass;
            }

            GUILayout.EndVertical();
        }

        bool Button(string label)
        {
            return GUILayout.Button(label);

        }
    }
}