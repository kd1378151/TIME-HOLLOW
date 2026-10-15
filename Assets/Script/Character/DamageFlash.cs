
using UnityEngine;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [Header("点滅させるスプライト")]
    public SpriteRenderer spriteRenderer;

    [Header("白く光る時間")]
    public float flashDuration = 0.1f;

    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        // SpriteRendererを自動取得する
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // 元の色を保存する
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    // ダメージを受けたときに呼び出す
    public void Flash()
    {
        Debug.Log("Flashが呼ばれました");

        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite Rendererが設定されていません");
            return;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }



    private IEnumerator FlashRoutine()
    {
        // テスト用に赤色へ変更
        spriteRenderer.color = Color.red;

        Debug.Log("変更後の色：" + spriteRenderer.color);

        // 0.5秒間、赤色を維持
        yield return new WaitForSecondsRealtime(0.5f);

        // 元の色に戻す
        spriteRenderer.color = originalColor;

        flashCoroutine = null;
    }


}
