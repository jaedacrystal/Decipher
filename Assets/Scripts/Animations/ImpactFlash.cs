using System.Collections;
using UnityEngine;

public class ImpactFlash : MonoBehaviour
{
    public enum FlashType { Damage, Heal, Stun, Buff, Debuff }

    public Color healColor = Color.green;
    public Color damageColor = Color.red;
    public Color buffColor = Color.yellow;
    public Color debuffColor = Color.blue;

    public void Flash(SpriteRenderer spriteRenderer, float duration, Color flashColor, float fadeDuration = 0.1f, FlashType flashType = FlashType.Damage)
    {
        if (spriteRenderer != null)
        {
            StartCoroutine(DoFlash(spriteRenderer, duration, flashColor, fadeDuration, flashType));
        }
    }

    private IEnumerator DoFlash(SpriteRenderer spriteRenderer, float duration, Color flashColor, float fadeDuration, FlashType flashType)
    {
        Color originalColor = spriteRenderer.color;
        Color targetColor = flashColor;

        switch (flashType)
        {
            case FlashType.Heal:
                targetColor = healColor;
                break;
            case FlashType.Damage:
                targetColor = damageColor;
                break;
            case FlashType.Buff:
                targetColor = buffColor;
                break;
            case FlashType.Debuff:
                targetColor = debuffColor;
                break;
        }

        spriteRenderer.color = targetColor;

        yield return new WaitForSeconds(duration);

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(targetColor, originalColor, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = originalColor;
    }
}