using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEditor.Build;

public static class BuildScript {
    const string KEYSTORE_URL = "/workspace/Unity/Keystore/bth.bigxu.online.keystore";
    const string KEYSTORE_PASSWORD = "111111";
    const string ALIAS_NAME = "bigxu_studio";
    const string ATLIAS_PASSWORD = "111111";


    enum CommandType {
        buildNumber,// --buildNumber ${BUILD_NUMBER}
        versionNumber,// --versionNumber ${Version_number}
        path,
        CONT
    };
    static string ToKey(this CommandType type) {
        return "--" + type.ToString();
    }

    const string DefaultValue = "FROMFILE";
    // [MenuItem("Build tool/android")]
    public static void BuildAndroid() {
        string path = "/Users/multimediajscbth/Desktop/Bigxu2019.apk";

        BuildAndroid("", -1, path);
    }


    public static void BuildAndroid(string version, int bundlecode, string path) {
        PlayerSettings.bundleVersion = version;
        PlayerSettings.Android.bundleVersionCode = bundlecode;
        PlayerSettings.Android.keystoreName = KEYSTORE_URL;
        PlayerSettings.Android.keystorePass = KEYSTORE_PASSWORD;
        PlayerSettings.Android.keyaliasName = ALIAS_NAME;
        PlayerSettings.Android.keyaliasPass = ATLIAS_PASSWORD;
        // PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        // Build player.
        path = path + ".apk";
        Debug.Log(path);

        //		string error = 
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, path, BuildTarget.Android, BuildOptions.None);

        //		if (string.IsNullOrEmpty (error)) {
        //			System.Diagnostics.Debug.WriteLine("build success");
        //			System.Console.WriteLine("build success");
        //		} else {
        //			System.Console.WriteLine ("error: " + error);
        //			System.Diagnostics.Debug.WriteLine("build success");
        //		}
    }

    public static void DoBuild() {
        DoBuild(GetCommandList());
    }

    public static void DoBuild(Dictionary<string, string> commandLineOption) {
        string version = commandLineOption[CommandType.versionNumber.ToKey()];

        int bundleCode = -1;
        int.TryParse(commandLineOption[CommandType.buildNumber.ToKey()], out bundleCode);

        string path = System.IO.Directory.GetCurrentDirectory();
        path = commandLineOption[CommandType.path.ToKey()];

        BuildAndroid(version, bundleCode, path);


    }

    static Dictionary<string, string> GetCommandList() {
        Dictionary<string, string> commandLineOption = new Dictionary<string, string>();
        for (CommandType i = CommandType.buildNumber; i < CommandType.CONT; i++) {
            string key = i.ToKey();
            commandLineOption.Add(key, GetParam(key));
        }
        return commandLineOption;
    }

    //get param from commandline
    static string GetParam(string paramName) {
        string target = "";
        string[] arguments = System.Environment.GetCommandLineArgs();
        int argNum = arguments.GetLength(0);
        for (int i = 0; i < argNum; i++) {
            if (arguments[i] == paramName)
                if (i + 1 < arguments.Length)
                    target = arguments[i + 1];
        }
        return target;
    }
}