﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StageController : MonoBehaviour
{
    //プレイヤーオブジェクトを入れる
    public GameObject playerObj;
    //スタートポジションを入れる
    public GameObject startPos;
    //オブジェクト"StageController"を宣言
    public static readonly string STR = "StageController";
    //ゲームクリア画面(GameCreaCanvas)
    public GameObject gameCreaUI;
    //ゲームオーバー画面(GameOverCanvas)
    public GameObject gameOverUI;
    //タイムアップのテキスト
    private GameObject timeUpText;

    // Start is called before the first frame update
    void Start()
    {
        //ゲームクリア画面を非アクティブにする
        this.gameCreaUI.SetActive(false);
        //ゲームオーバー画面を非アクティブにする
        this.gameOverUI.SetActive(false);
        //タイムアップテキストを取得
        this.timeUpText = GameObject.Find("TimeUpText");

        StartPosition();
    }

    private void Update()
    {
        //
    }

    public void Retry()
    {
        //ステージ1のシーンをロードする
        SceneManager.LoadScene("Stage1");
    }

    public void TitleBack()
    {
        //タイトルのシーンをロードする
        SceneManager.LoadScene("Title");
    }

    public void PlayerDown()
    {
        //残機が0なら
        if (GManager.instance.GetContinueNum() == 0)
        {
            //GameOver関数を呼び出す
            GameOver();
        }
        //まだ残機があるなら
        else if (GManager.instance.GetContinueNum() > 0)
        {
            //プレイヤーの位置を初期地点に戻す
            Continue();
            //残機を1減らす
            GManager.instance.MinusContinueNum();
        }
    }

    public void StartPosition()
    {
        //プレイヤーオブジェクトの座標にスタート地点の座標を代入する
        if (playerObj != null && startPos != null)
        {
            playerObj.transform.position = startPos.transform.position;
        }
    }

    public void Continue()
    {
        //プレイヤーオブジェクトの座標にスタート地点の座標を代入する
        if (playerObj != null && startPos != null)
        {
            playerObj.transform.position = startPos.transform.position;
            //プレイヤーオブジェクトを非アクティブにする
            playerObj.SetActive(false);
            //一秒後にプレイヤーアクティブ関数を呼ぶ
            Invoke("PlayerActive", 0.5f);
        }
    }

    public void PlayerActive()
    {
        //プレイヤーをアクティブにする
        playerObj.SetActive(true);
    }

    public void TimeUp()
    {
        //タイムアップと表示するスクリプト
        this.timeUpText.GetComponent<Text>().text = "TIME UP!!";
    }

    public void GameOver()
    {
        {
            //タイムアップテキストを破棄
            Destroy(timeUpText);
            //ゲームオーバー(画面上にゲームオーバー画面を表示する)
            this.gameOverUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void GameCrea()
    {
        if (this.gameCreaUI != null)
        {
            //ゲームクリア画面を表示する
            this.gameCreaUI.SetActive(true);
        }
    }
}
