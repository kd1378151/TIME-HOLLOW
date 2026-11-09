using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [Header("体力")]
    public int maxHP = 100;
    public int currentHP = 100;

    [Header("攻撃力")]
    public int attackPower1 = 10;
    public int attackPower2 = 30;

    [Header("スキル設定")]
    [SerializeField] private float skillDuration = 10f;
    [SerializeField] private float skillTimeCost = 30f;
    [SerializeField] private float skillAttackMultiplier = 1.5f;

    private bool isSkillActive = false;
    private GameMaster gameMaster;

    [Header("探索時間")]
    public float maxExplorationTime = 100f;
    public float currentExplorationTime = 100f;

    [Header("ゲームオーバー設定")]
    public string gameOverSceneName = "GameOver";

    [Header("スキルエフェクト")]
    public ParticleSystem skillEffect;

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerMovement playerMovement;
    private PlayerAttack playerAttack;

    private bool isTakingHit = false;
    private bool isDead = false;

    private DamageFlash damageFlash;

    [Header("ゲームオーバー暗転")]
    [SerializeField] private float gameOverFadeDuration = 2f;

    public bool CanControl
    {
        get { return !isTakingHit && !isDead; }
    }

    void Start()
    {
        // 強化データを反映
        maxHP = PlayerUpgradeData.MaxHP;
        attackPower1 = PlayerUpgradeData.AttackPower1;
        attackPower2 = PlayerUpgradeData.AttackPower2;
        maxExplorationTime = PlayerUpgradeData.MaxExplorationTime;

        currentHP = maxHP;
        currentExplorationTime = maxExplorationTime;

        gameMaster = FindAnyObjectByType<GameMaster>();

        // Animator取得
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Rigidbody2D取得
        rb = GetComponent<Rigidbody2D>();

        // プレイヤー関連スクリプト取得
        playerMovement = GetComponent<PlayerMovement>();
        playerAttack = GetComponent<PlayerAttack>();

        // ダメージ演出
        damageFlash = GetComponentInChildren<DamageFlash>();

        //攻撃力アップのスキルエフェクト
        if (skillEffect != null)
        {
            skillEffect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateSkill();
        }
    }

    // 敵からダメージを受けたとき
    public void TakeDamage(int damage)
    {
        if (!CanControl) return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        // HPが0になった場合
        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            // HPが残っている場合はよろける
            isTakingHit = true;
            StopPlayer();

            if (animator != null)
            {
                animator.SetTrigger("TakeHit");
            }

            if (damageFlash != null)
            {
                damageFlash.Flash();
            }
        }
    }

    // 死亡処理

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        // ゲーム全体の時間を停止
        Time.timeScale = 0f;

        // プレイヤーの移動・攻撃を停止
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        if (playerAttack != null)
        {
            playerAttack.enabled = false;
        }

        // プレイヤーの物理演算を停止
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 死亡アニメーションだけ時間停止の影響を受けないようにする
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;

            animator.ResetTrigger("TakeHit");
            animator.Play("Death", 0, 0f);
        }
    }


    // よろけアニメーション終了時
    public void EndHitReaction()
    {
        if (isDead) return;

        isTakingHit = false;
    }

    // 死亡アニメーション終了時に呼び出す

    public void OnDeathAnimationEnd()
    {
        if (!isDead) return;

        StartCoroutine(GameOverFade());
    }

    // プレイヤーの移動を停止する
    private void StopPlayer()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private IEnumerator GameOverFade()
    {
        // FadePanelを探す
        Image fadePanel = null;

        Canvas[] canvases = FindObjectsByType<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            Transform panel = canvas.transform.Find("FadePanel");

            if (panel != null)
            {
                fadePanel = panel.GetComponent<Image>();

                if (fadePanel != null)
                    break;
            }
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);

            Color color = Color.black;
            color.a = 0f;
            fadePanel.color = color;

            float elapsed = 0f;

            while (elapsed < gameOverFadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                color.a = Mathf.Clamp01(
                    elapsed / gameOverFadeDuration
                );

                fadePanel.color = color;

                yield return null;
            }

            color.a = 1f;
            fadePanel.color = color;
        }

        // ゲームオーバー画面へ
        Time.timeScale = 1f;

        SceneManager.LoadScene(gameOverSceneName);
    }

    // スキル発動
    private void ActivateSkill()
    {
        if (!CanControl) return;

        if (isSkillActive) return;

        if (gameMaster == null) return;

        // 時間を消費できなければ発動しない
        if (!gameMaster.TryReduceSkillTime(skillTimeCost))
        {
            return;
        }

        // スキル開始
        StartCoroutine(SkillCoroutine());
    }

    private IEnumerator SkillCoroutine()
    {
        isSkillActive = true;

        int originalAttackPower1 = attackPower1;
        int originalAttackPower2 = attackPower2;

        // 攻撃力1.5倍
        attackPower1 = Mathf.FloorToInt(
            originalAttackPower1 * skillAttackMultiplier
        );

        attackPower2 = Mathf.FloorToInt(
            originalAttackPower2 * skillAttackMultiplier
        );

        // パーティクル表示
        if (skillEffect != null)
        {
            skillEffect.Play();
        }

        // スキル効果時間
        yield return new WaitForSeconds(skillDuration);

        // 攻撃力を元に戻す
        attackPower1 = originalAttackPower1;
        attackPower2 = originalAttackPower2;

        // 新しいパーティクルの発生だけを停止
        // すでに出ているパーティクルは最後まで再生する
        if (skillEffect != null)
        {
            skillEffect.Stop(
                true,
                ParticleSystemStopBehavior.StopEmitting
            );
        }

        isSkillActive = false;
    }
}
