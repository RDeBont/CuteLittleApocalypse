using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

// Verplaatst alle tiles van de ene tilemap naar de andere (bijv. Foreground naar Ground).
// Te openen via Tools > Tilemap Verplaatser. Ctrl+Z maakt de actie ongedaan.
public class TilemapVerplaatser : EditorWindow
{
    private Tilemap bron;
    private Tilemap doel;
    private bool overschrijven = true;

    [MenuItem("Tools/Tilemap Verplaatser")]
    private static void Open()
    {
        GetWindow<TilemapVerplaatser>("Tilemap Verplaatser");
    }

    private void OnGUI()
    {
        bron = (Tilemap)EditorGUILayout.ObjectField("Van (bron)", bron, typeof(Tilemap), true);
        doel = (Tilemap)EditorGUILayout.ObjectField("Naar (doel)", doel, typeof(Tilemap), true);
        overschrijven = EditorGUILayout.Toggle("Bestaande tiles overschrijven", overschrijven);

        GUI.enabled = bron != null && doel != null && bron != doel;
        if (GUILayout.Button("Verplaats tiles"))
        {
            Verplaats();
        }
        GUI.enabled = true;
    }

    private void Verplaats()
    {
        Undo.RegisterCompleteObjectUndo(new Object[] { bron, doel }, "Tiles verplaatsen");

        int aantal = 0;
        bron.CompressBounds();

        foreach (Vector3Int positie in bron.cellBounds.allPositionsWithin)
        {
            TileBase tile = bron.GetTile(positie);
            if (tile == null)
            {
                continue;
            }

            // Wereldpositie gebruiken zodat het ook klopt als de tilemaps verschoven zijn
            Vector3Int doelPositie = doel.WorldToCell(bron.GetCellCenterWorld(positie));

            if (!overschrijven && doel.HasTile(doelPositie))
            {
                continue;
            }

            doel.SetTile(doelPositie, tile);
            doel.SetTransformMatrix(doelPositie, bron.GetTransformMatrix(positie));
            doel.SetColor(doelPositie, bron.GetColor(positie));
            bron.SetTile(positie, null);
            aantal++;
        }

        doel.CompressBounds();
        bron.CompressBounds();
        EditorUtility.SetDirty(bron);
        EditorUtility.SetDirty(doel);

        Debug.Log($"{aantal} tiles verplaatst van {bron.name} naar {doel.name}.");
    }
}
