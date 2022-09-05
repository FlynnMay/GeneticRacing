using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Evo.Tools
{
#if UNITY_EDITOR
    public static class MenuTools
    {
        [MenuItem("Evo/Create/DNA Type")]
        public static void CreateDNATypeScript()
        {
            string copyPath = FilePathWizard.GetPath(FilePathWizard.templateKey) + "DNAObject.evotmp";
            string path = FilePathWizard.GetPath(FilePathWizard.DNATypeKey) + "DNAObject.cs";

            CopyFileToPath(copyPath, path);
        }

        [MenuItem("Evo/Create/DNA Value Generator")]
        public static void CreateDNAValueGeneratorScript()
        {
            string copyPath = FilePathWizard.GetPath(FilePathWizard.templateKey) + "DNAValueGenerator.evotmp";
            string path = FilePathWizard.GetPath(FilePathWizard.DNAValueGeneratorKey) + "DNAValueGenerator.cs";

            CopyFileToPath(copyPath, path);
        }

        [MenuItem("Evo/Create/Custom Agent Fitness")]
        public static void CreateFitnessScript()
        {
            string copyPath = FilePathWizard.GetPath(FilePathWizard.templateKey) + "AgentFitness.evotmp";
            string path = FilePathWizard.GetPath(FilePathWizard.agentFitnessKey) + "AgentFitness.cs";

            CopyFileToPath(copyPath, path);
        }

        private static void CopyFileToPath(string copyPath, string path)
        {
            StreamReader sr = new StreamReader(copyPath);
            string script = sr.ReadToEnd();
            sr.Close();

            FileStream fs = File.Create(path);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(script);
            sw.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();

            Selection.activeObject = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        }
    }
#endif

}
