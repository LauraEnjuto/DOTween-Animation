using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class TweenAnimation : MonoBehaviour
{
    [Header("Properties")]
    
    [SerializeField] private float _rotateDuration = 3f;
    [SerializeField] private float _shakeDuration = 0.7f;
    [SerializeField] private float _colorChangeDuration = 0.2f;
    [ColorUsage(true, true)]
    [SerializeField] private Color _BaseGlowColor;
    [SerializeField] private Color _FadeOutColor;      
    
    private Vector3 _scale;   
    private Vector3 _rotation = new Vector3(0, 360, 0);

    public ParticleSystem starBoost;
    public Image achivementImage;

    void Start()
    {
        starBoost.Stop();
        _scale = transform.localScale;
        IntroTweenAnimation();
    }

    void IntroTweenAnimation()
    {
        Sequence seq = DOTween.Sequence();

        seq

            //INTRO ANIMATION
            .Append(transform.DOScale(0, 0)).AppendInterval(2f)
            .Append(transform.DOScale(0, 0)) //Spawn animation
            .Append(transform.DOScale(_scale, 2)) //Spawn animation

            //PARTICLE STAR BOOST WITH PUNCH AND ROTATION
            .AppendCallback(() => starBoost.Play())
            .Append(transform.DOPunchScale(new Vector3(0.5f, 0.5f, 0.5f), 1f, 10, 1f))
            .Join(transform.DORotate(_rotation, _rotateDuration, RotateMode.FastBeyond360).SetEase(Ease.InOutSine))

            //ROTATION ON ONE VERTEX OF THE CUBE
            .Append(transform.DORotate(new Vector3(30, 0, 45), 0.5f)).AppendInterval(0.5f)
            .Append(transform.DORotate(new Vector3(30, 1800, 45), 4, RotateMode.FastBeyond360).SetEase(Ease.InOutSine))

            //CHANGE COLOR EVERY X SECONDS 
            .InsertCallback(8f, () => InvokeRepeating("ChangeColor", 0.5f, 0.5f))
            .InsertCallback(11f, () => CancelInvoke("ChangeColor"))

            //SPIN
            .AppendCallback(() => transform.GetComponent<Renderer>().material.EnableKeyword("_EMISSION"))
            .AppendCallback(() => transform.GetComponent<Renderer>().material.SetColor("_EmissionColor", _BaseGlowColor))

            .Append(transform.DORotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.InOutExpo)).AppendInterval(1f)

            //MOVE TO LEFT
            .Append(transform.DOMove(new Vector3(-2.5f, transform.position.y, 0), 0.8f).SetEase(Ease.InOutExpo))
            .Join(transform.DORotate(new Vector3(0, 0, 25), 0.8f)).AppendInterval(0.4f)

            .Append(transform.DORotate(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.InSine)).AppendInterval(1f)

            //SCALE BEFORE JUMP 
            .Append(transform.DOScaleY(0.7f, 0.2f)).AppendInterval(0.5f)
            .Append(transform.DOScaleY(1.5f, 0.1f))

            //JUMP
            .AppendCallback(() => transform.GetComponent<TrailRenderer>().enabled = true) //Enable trail
            .Append(transform.DOJump(new Vector3(2.5f, transform.position.y, 0), 2f, 1, 1.3f))
            .Join(transform.DORotate(new Vector3(0, 0, -180), 1.3f))
            .Append(transform.DOScaleY(1f, 0.2f)).AppendInterval(1f)
            .AppendCallback(() => transform.GetComponent<TrailRenderer>().enabled = false) //Disable trail

            //MOVE TO LEFT
            .Append(transform.DOMove(new Vector3(0f, transform.position.y, 0), 1.5f).SetEase(Ease.OutExpo)).AppendInterval(0.1f)

            //POPPING THE ACHIVEMENT
            .Append(achivementImage.GetComponent<CanvasGroup>().DOFade(1, 0.4f))
            .Join(achivementImage.GetComponent<RectTransform>().DOAnchorPos(new Vector2(716, 363), 1f).SetEase(Ease.InSine))
            .AppendInterval(3f)
            .Append(achivementImage.GetComponent<CanvasGroup>().DOFade(0, 0.9f))
            .Join(achivementImage.GetComponent<RectTransform>().DOAnchorPos(new Vector2(1217, 363), 1f).SetEase(Ease.InSine))
            .AppendInterval(1f)

            //SHAKE AND DISSAPEAR
            .Append(transform.DOShakeScale(_shakeDuration, 0.5f, 20, 90f, false))
            .Join(transform.GetComponent<Renderer>().material.DOColor(_FadeOutColor, _colorChangeDuration).SetEase(Ease.InCubic)) //Change color
            .Append(transform.DOScale(0, 1).OnComplete(() => { DestroyImmediate(this); }));

    }

    void ChangeColor()
    {
        Material material = transform.GetComponent<Renderer>().material;

        material.EnableKeyword("_EMISSION");
        Color color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        material.SetColor("_EmissionColor", color);
        
    }

}