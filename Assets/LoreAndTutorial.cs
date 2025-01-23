using System.Collections;
using UnityEngine;
using TMPro; // Pastikan menggunakan TextMeshPro

public class LoreAndTutorial : MonoBehaviour
{
    public TextMeshProUGUI loreText; // Referensi ke TextMeshPro di Canvas
    public string[] fullText; // Teks lengkap lore dan tutorial
    private int currentLineIndex = 0; // Indeks baris saat ini
    public float lineDisplayDelay = 2f; // Waktu jeda antar baris
     private bool isLoreActive = false;

    public void StartLore()
    {
        if (!isLoreActive) // Cegah lore dimulai dua kali
        {
            isLoreActive = true;
            StartCoroutine(TypeText());
        }
    }

    IEnumerator TypeText()
    {
        while (currentLineIndex < fullText.Length)
        {
            loreText.text = fullText[currentLineIndex]; // Tampilkan baris saat ini
            currentLineIndex++; // Pindah ke baris berikutnya
            yield return new WaitForSeconds(lineDisplayDelay); // Tunggu sebelum menampilkan baris berikutnya
        }
    }
}
