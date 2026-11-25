
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBar : MonoBehaviour
{
    [Header("敵のステータス")]
    [SerializeField] private MonoBehaviour enemyStatusComponent;

    private IEnemyStatus enemyStatus;

    [Header("HPゲージ")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpFill;

    [Header("HPの色")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHPColor = Color.red;

    [Header("赤色に変わるHP割合")]
    [SerializeField, Range(0f, 1f)]
    private float lowHPThreshold = 0.25f;

    [Header("頭上の位置")]
    [SerializeField]
    private Vector3 offset = new Vector3(0f, 2f, 0f);

    private void Awake()
    {
        // ① Inspectorからステータスを取得
        if (enemyStatusComponent != null)
        {
            enemyStatus = enemyStatusComponent as IEnemyStatus;
        }

        // ② 取得できなければ親オブジェクトから検索
        if (enemyStatus == null)
        {
            MonoBehaviour[] components =
                GetComponentsInParent<MonoBehaviour>(true);

            foreach (MonoBehaviour component in components)
            {
                if (component is IEnemyStatus status)
                {
                    enemyStatus = status;

                    enemyStatusComponent = component;

                    break;
                }
            }
        }

        // ③ 取得結果を確認
        if (enemyStatus != null)
        {
            Debug.Log(
                "敵ステータス取得成功！ 最大HP：" +
                enemyStatus.MaxHP,
                this
            );
        }
        else
        {
            Debug.LogError(
                "敵ステータスの取得に失敗しました。GolemStatsとIEnemyStatusを確認してください。",
                this
            );
        }
    }

    private void Start()
    {
        // Canvasを敵の頭上に配置
        transform.localPosition = offset;

        // 初期HPバーを更新
        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        if (enemyStatus == null)
            return;

        if (enemyStatus.MaxHP <= 0)
        {
            if (hpSlider != null)
                hpSlider.value = 0;

            if (hpFill != null)
                hpFill.color = lowHPColor;

            return;
        }

        // HP割合を計算
        float hpRatio = Mathf.Clamp01(
            (float)enemyStatus.CurrentHP / enemyStatus.MaxHP
        );

        // HPゲージを更新
        if (hpSlider != null)
        {
            hpSlider.maxValue = enemyStatus.MaxHP;
            hpSlider.value = enemyStatus.CurrentHP;
        }

        // HPに応じて色を変更
        if (hpFill != null)
        {
            hpFill.color = hpRatio <= lowHPThreshold
                ? lowHPColor
                : normalColor;
        }
    }
}
