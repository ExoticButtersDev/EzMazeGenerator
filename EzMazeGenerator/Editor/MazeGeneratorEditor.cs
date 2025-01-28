using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MazeGenerator))]
public class MazeGeneratorEditor : Editor
{

    private GUIStyle labelStyle;
    private GUIStyle buttonStyle;

    private bool meshEnabled = true;

    public async override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MazeGenerator generator = (MazeGenerator)target;

        labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.fontSize = 16;

        buttonStyle = new GUIStyle();
        buttonStyle.imagePosition = ImagePosition.ImageOnly;
        buttonStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Space(5);

        EditorGUILayout.LabelField($"Total Objects: {CalculateWallCount(generator.mazeScale.x, generator.mazeScale.z)}", labelStyle);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Maze"))
        {
            await generator.GenerateMaze();
        }

        if (GUILayout.Button("Clear Maze"))
        {
            generator.ClearMaze();
        }

        if (GUILayout.Button("Make Mesh Static"))
        {
            generator.MakeMeshStatic();
        }

        if (!meshEnabled)
        {
            if (GUILayout.Button("Make Mesh Visible"))
            {
                generator.MakeMeshVisible();
                meshEnabled = true;
            }

        }
        else
        {
            if (GUILayout.Button("Make Mesh Invisible"))
            {
                generator.MakeMeshInvisible();
                meshEnabled = false;
            }
        }
    }

    private int CalculateWallCount(int width, int height)
    {
        int innerWalls = width * height;
        int outerWalls = (width * 2) + (height * 2);
        return innerWalls + outerWalls;
    }
}
