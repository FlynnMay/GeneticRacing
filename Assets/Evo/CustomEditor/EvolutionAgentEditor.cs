using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Evo.EditorStyle
{
    [CustomEditor(typeof(EvolutionAgent))]
    public class EvolutionAgentEditor : Editor
    {
        bool foldout;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");

            EvolutionAgent agent = target as EvolutionAgent;

            GUILayout.Space(5);
            if (GUILayout.Button(new GUIContent("Save DNA", "Save the DNA of an agent")))
                agent.ExportDNA();

            GUILayout.Space(5);
            DebugInfo(agent);
            GUILayout.Space(5);
            FitnessProgressBar(agent);
            serializedObject.ApplyModifiedProperties();
        }

        private void FitnessProgressBar(EvolutionAgent agent)
        {
            if (agent.DNA == null)
                return;

            Rect r = EditorGUILayout.BeginVertical();
            EditorGUI.ProgressBar(r, agent.DNA.Fitness, "Fitness");
            GUILayout.Space(18);
            EditorGUILayout.EndVertical();
        }

        private void DebugInfo(EvolutionAgent agent)
        {
            foldout = EditorGUILayout.Foldout(foldout, new GUIContent("Debug", "Agent Information"), EditorStyles.foldoutHeader);
            if (foldout)
            {
                GUI.enabled = false;
                string contents = "When agents are assigned information on startup this field will be filled.";

                if (agent.DNA != null)
                    contents = $"Fitness: {agent.DNA.Fitness}\n" +
                        $"Score: {agent.Score}\n" +
                        $"Alive: {agent.IsAlive}\n" +
                        $"Elite: {agent.IsElite}\n" +
                        $"King: {agent.IsKing}";

                EditorGUILayout.HelpBox(contents, MessageType.None);
                EditorGUILayout.ObjectField("DNA Type", agent.DNAType, typeof(DNA), false);
                GUI.enabled = true;
            }
        }
    }
}