﻿//==============================================================================================
/// File Name	: TimeGaugeController.cs
/// Summary		: タイムゲージ用スクリプト
//==============================================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Common;
//==============================================================================================
public class TimeGaugeController : MonoBehaviour
{
    //------------------------------------------------------------------------------------------
    // member variable
    //------------------------------------------------------------------------------------------
    [SerializeField]
    // テクスチャ
    private Image timeImage = null;
    // キャンバスグループ
    private CanvasGroup canvasGroup = null;
    // プレイヤー
    private GameObject player = null;
    
    [SerializeField]
    // アラート開始時のタイムの割合
    private float alertTimeRate = 1.0f / 6;
    // アラートが開始してからの時間
    private float alertTime = 0;

    [SerializeField]
    // プレイヤーと重なっているときのアルファ値
    private float fadeAlpha = 0.2f;
    [SerializeField]
    // 透明になり始める距離
    private float fadeStartRadius = 150.0f;
    [SerializeField]
    // 透明になり終わる距離
    private float fadeEndRadius = 110.0f;

    // SE
    [SerializeField]
    AudioClip audio;
    bool onPlay = false;

    //------------------------------------------------------------------------------------------
    // Awake
    //------------------------------------------------------------------------------------------
    private void Awake()
    {
        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

	//------------------------------------------------------------------------------------------
    // Start
	//------------------------------------------------------------------------------------------
    private void Start()
    {
        player = GameObject.Find(Player.NAME);
    }

	//------------------------------------------------------------------------------------------
    // Update
	//------------------------------------------------------------------------------------------
	private void Update()
    {
        float timeRate = Data.time / Data.timeLimit;
        if(timeRate <= alertTimeRate)
        {
            alertTime += Time.deltaTime;
            if(!onPlay)
            {
                SoundPlayer.Play(audio);
                onPlay = true;
            }
        }

        OverlapPlayer();

        Material mat = timeImage.material;
        mat.SetFloat("_TimeRate", timeRate);
        mat.SetFloat("_AlertTime", alertTime);
    }

    //------------------------------------------------------------------------------------------
    // プレイヤーとの重なり処理
    //------------------------------------------------------------------------------------------
    private void OverlapPlayer()
    {
        var playerPos = RectTransformUtility.WorldToScreenPoint(UnityEngine.Camera.main, player.transform.position);
        var timePos = (Vector2)timeImage.rectTransform.position;

        float dist = Vector2.Distance(playerPos, timePos);
        
        float t = (Mathf.Clamp(dist, fadeEndRadius, fadeStartRadius) - fadeEndRadius) / (fadeStartRadius - fadeEndRadius);
        canvasGroup.alpha = Mathf.Lerp(fadeAlpha, 1.0f, t);
    }

    //------------------------------------------------------------------------------------------
    // OnApplicationQuit
    //------------------------------------------------------------------------------------------
    private void OnApplicationQuit()
    {
        Material mat = timeImage.material;
        mat.SetFloat("_TimeRate", 1.0f);
        mat.SetFloat("_AlertTime", 0.0f);
    }
}