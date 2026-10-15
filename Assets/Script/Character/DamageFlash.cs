
using UnityEngine;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    [Header("点滅させるスプライト")]
    public SpriteRenderer spriteRenderer;

    [Header("白く光る時間")]
    public float flashDuration = 0.1f;

    // 白色点滅用のマテリアル
    private Material flashMaterial;

    // 元のマテリアル
    private Material originalMaterial;

    // 点滅処理
    private Coroutine flashCoroutine;

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            // 元のマテリアルを保存
            originalMaterial = spriteRenderer.sharedMaterial;

            // 元のマテリアルを複製して、スプライトの設定を引き継ぐ
            flashMaterial = new Material(originalMaterial);

            // 白色点滅用のシェーダーに変更
            flashMaterial.shader = Shader.Find("Custom/WhiteFlashShader");
        }
    }

    public void Flash()
    {
        if (spriteRenderer == null || flashMaterial == null)
        {
            Debug.LogWarning("DamageFlashの設定を確認してください");
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
        // 白色点滅用マテリアルに変更
        spriteRenderer.material = flashMaterial;

        flashMaterial.SetFloat("_FlashAmount", 0.2f);

        yield return new WaitForSeconds(flashDuration);

        // 元のマテリアルに戻す
        spriteRenderer.material = originalMaterial;

        flashCoroutine = null;
    }

    void OnDestroy()
    {
        // 作成したマテリアルを解放
        if (flashMaterial != null)
        {
            Destroy(flashMaterial);
        }
    }
}
