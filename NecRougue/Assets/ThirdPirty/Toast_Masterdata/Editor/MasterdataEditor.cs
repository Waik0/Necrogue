using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Toast.Masterdata.Editor
{

    public class MasterdataEditor : EditorWindow
    {
        //classes


        //enum
        public enum State
        {
            Init,
            Select,
            View,
        }


        //process

        private Dictionary<State, Action<MasterdataEditor>> StateProcess =
            new Dictionary<State, Action<MasterdataEditor>>()
            {
                {
                    State.Init,
                    self =>
                    {
                        self.Table = new TableData();
                        self.Menu = new MenuData();
                        self.Table.LoadClassList();
                        self.Next(State.View);

                    }
                },
                {
                    State.Select,
                    self =>
                    {
                        self.Menu?.View();
                        switch (self.Menu.Select)
                        {
                            case MenuData.Selected.None:
                                break;
                            case MenuData.Selected.Create:
                                self.Next(State.View);
                                break;
                            case MenuData.Selected.LoadJson:
                                break;
                            case MenuData.Selected.LoadClass:
                                self.Table.LoadClassList();
                                self.Next(State.View);
                                break;
                        }
                    }
                },

                {
                    State.View,
                    self =>
                    {  GUILayout.BeginVertical(GUI.skin.box);
                        self.Table?.ViewClasses();
                        self.Table?.View();
                        GUILayout.EndVertical();
                    }
                }

            };

        State Current { get; set; } = State.Init;
        private State NextState { get; set; } = State.Init;
        private MenuData Menu { get; set; }
        TableData Table { get; set; }

        //[MenuItem("Toast/MasterdataEditor")]
        static void Open()
        {
            var window = CreateInstance<MasterdataEditor>();
            window.Show();
        }

        void UIDraw()
        {

        }

        void Next(State state)
        {
            NextState = state;
        }

        private void Update()
        {
            if (Current != NextState)
            {
                Current = NextState;
            }
        }

        private void OnGUI()
        {
            if (StateProcess.ContainsKey(Current))
            {
                StateProcess[Current]?.Invoke(this);
            }
        }

    }

}