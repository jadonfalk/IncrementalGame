using UnityEngine;
using UnityEngine.UI;

// Require an Image instead of SpriteRenderer
[RequireComponent(typeof(Image))]

public class AnimatedImage : MonoBehaviour
{
    public Image image { get; private set; }
    public Sprite[] sprites;
    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    private void Awake()
    {
        this.image = GetComponent<Image>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(Advance), this.animationTime, this.animationTime);
    }

    private void Advance()
    {
        // Stop if image is disabled
        if (!this.image.enabled)
        {
            return;
        }

        this.animationFrame++;

        if (this.animationFrame >= this.sprites.Length && this.loop)
        {
            this.animationFrame = 0;
        }

        if (this.animationFrame >= 0 && this.animationFrame < this.sprites.Length)
        {
            this.image.sprite = this.sprites[this.animationFrame];
        }
    }

    public void Restart()
    {
        this.animationFrame = -1;
        Advance();
    }
}