﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMoveFloorCheck : MonoBehaviour
{
    //MoveFloorTagの変数
    private string moveFloorTag = "MoveFloorTag";
    //移動床判定
    private bool onMoveFloor = false;
    //移動床に入った、入り続けている、出た時のフラグ
    private bool onMoveFloorEnter, onMoveFloorStay, onMoveFloorExit;

    enum Status
    {
        Enter,
        Stay,
        Exit
    }

    private Status status = Status.Enter;

    //移動床判定
    public bool OnMoveFloor()
    {
        //移動床搭乗判定
        /*        if (onMoveFloorEnter || onMoveFloorStay)
                {
                    //移動床判定
                    onMoveFloor = true;
                }
                else if (onMoveFloorExit)
                {
                    onMoveFloor = false;
                }

                //各フラグは無効に戻す
                onMoveFloorEnter = false;
                onMoveFloorStay = false;
                onMoveFloorExit = false;

                //移動床判定結果を返す
                return onMoveFloor; */

        return status == Status.Enter || status == Status.Stay;
    }

    //各種トリガーフラグを用意
    private void OnTriggerEnter(Collider other)
    {
        //侵入したタグをコンソールに表示
        Debug.Log(other.gameObject.tag);
        if (other.tag == moveFloorTag)
        {
            status = Status.Enter;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == moveFloorTag)
        {
            status = Status.Stay;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == moveFloorTag)
        {
            status = Status.Exit;
        }
    }

}
