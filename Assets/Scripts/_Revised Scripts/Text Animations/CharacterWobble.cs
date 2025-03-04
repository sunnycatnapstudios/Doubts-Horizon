using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterWobble : MonoBehaviour
{
    TMP_Text textMesh;
    Mesh mesh;
    Vector3[] vertices;
    public float modifier;

    void Start()
    {
        if (modifier == 0) modifier = 1f;
        textMesh = GetComponent<TMP_Text>();
    }

    void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = Wobble(Time.unscaledTime + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    Vector2 Wobble(float time) {
        return new Vector2(Mathf.Sin(time*3.3f*modifier), Mathf.Cos(time*2.5f*modifier));
    }

    public void NormalWobble() {modifier = 1;}
    public void ExtraWobble() {modifier = 5;}
}
