using System.Collections;
using UnityEngine;
using TMPro;

public class LoreTextManager : MonoBehaviour
{
    public TextMeshProUGUI loreText; // Referensi ke TextMeshPro di Canvas
    public string[] loreLines; // Array berisi teks yang akan ditampilkan
    public float lineDisplayDelay = 2f; // Waktu jeda antar baris
    private int currentLineIndex = 0; // Indeks baris saat ini

    private void Start()
    {
        loreText.text = ""; // Kosongkan teks saat awal
        StartCoroutine(DisplayLoreLines());
    }

    private IEnumerator DisplayLoreLines()
    {
        while (currentLineIndex < loreLines.Length)
        {
            loreText.text = loreLines[currentLineIndex]; // Tampilkan baris saat ini
            currentLineIndex++; // Pindah ke baris berikutnya
            yield return new WaitForSeconds(lineDisplayDelay); // Tunggu sebelum menampilkan baris berikutnya
        }
    }
}
