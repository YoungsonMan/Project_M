// Unity 에디터 환경에서만 컴파일.
// 빌드 실행 환경에서는 제외되어 불필요한 컴파일 및 실행을 방지.
#if UNITY_EDITOR
using UnityEditor; // 에디터 확장 기능을 제공.
using UnityEngine;

// 스크립트의 커스텀 Inspector를 정의.
[CustomEditor(typeof(GridManager))]
public class MapEditor : Editor
{
    /// <summary>
    /// Inspector 창의 GUI를 그리기 위해 호출되는 메서드.
    /// 사용자 지정 버튼, 텍스트 필드 등을 구현이 가능하다.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // 기본 Inspector 속성들을 유지 + 사용자 정의 GUI.
        DrawDefaultInspector();

        GridManager mapData = (GridManager)target;

        // Inspector창에 "Generate Map"버튼을 생성한다.
        if (GUILayout.Button("Generate Map"))
        {
            mapData.GenerateMap();
        }

        // Inspector창에 "Clear Map"버튼을 생성한다.
        if (GUILayout.Button("Clear Map"))
        {
            mapData.ClearMap();
        }
    }
}
#endif