﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidPlayerController : MonoBehaviour
{
    //リジッドボディ変数
    private Rigidbody rigid;
    //アニメーター変数
    private Animator animator;

    //動かす速度のベクトル
    private Vector3 velocity;
    //ジャンプ力
    [SerializeField] private float jumpPower = 5f;
    //入力値
    private Vector3 input;
    //走る速度
    [SerializeField] private float RunSpeed = 5f;
    //歩く速さ
    [SerializeField] private float walkSpeed = 2f;
    //ジャンプした時の高さ
    private float jumpPos;
    //ジャンプの高さ制限
    [SerializeField] private float jumpHeight = 3f;
    //Playerレイヤー以外のレイヤーマスク
    int layerMask;
    //グラウンドタグ
    private string groundTag = "GroundTag";
    //移動床タグ
    private string moveFloorTag = "MoveFloorTag";
    //接地判定
    private bool isGround = false;
    //ジャンプ判定
    private bool isJump = false;
    //GroundCheckScriptの変数
    public GroundCheckScript ground;

    void Start()
    {
        //アニメーターを取得
        animator = GetComponent<Animator>();
        //リジッドボディを取得
        rigid = GetComponent<Rigidbody>();
        layerMask = ~(1 << LayerMask.NameToLayer("Player"));
    }


    void Update()
    {

        //クリア判定時、動きを止める
        if (GManager.instance.isCrea == true)
        {
            velocity = Vector3.zero;
        }
        else
        {

            //接地判定を取得
            isGround = ground.IsGround();

            //ジャンプ開始トリガーが戻らない事があるので、毎フレームリセット
            if (animator.GetBool("JumpStart"))
            {
                animator.ResetTrigger("JumpStart");
            }

            //前後左右の入力を取得
            input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            //接地時
            if (isGround)
            {
                //接地しているので一度速度を0にする(しないとどっか飛んでく)
                velocity = Vector3.zero;
                //ジャンプアニメーションが再生されていた時
                if (animator.GetBool("JumpFall") || animator.GetBool("JumpUp"))
                {
                    //ジャンプ落下・上昇アニメーションを無効化
                    animator.SetBool("JumpFall", false);
                    animator.SetBool("JumpUp", false);
                    //着地アニメーションを再生
                    animator.SetTrigger("JumpLanding");
                }

                //入力の長さが0.1より大きい時且つ、着地アニメーションが終わっている時
                if (input.magnitude > 0.1f && !animator.GetBool("JumpLanding"))
                {
                    //入力した方向へ向かせる
                    transform.LookAt(transform.position + input.normalized);
                    //アニメーションのSpeedに入力の値を渡す
                    animator.SetBool("Run", true);
                    //移動する
                    velocity += transform.forward * RunSpeed;
                    //足音を有効化
                    GetComponent<AudioSource>().volume = 1;
                }
                else
                {
                    //走りアニメーションを無効化
                    animator.SetBool("Run", false);
                    //足音を無効化
                    GetComponent<AudioSource>().volume = 0;
                }
                //ジャンプキーを押した時
                if (Input.GetKey(KeyCode.Space) && !animator.GetBool("JumpLanding"))
                {
                    //ジャンプ開始アニメーションを再生
                    animator.SetTrigger("JumpStart");
                    //上方向へジャンプ力を代入
                    velocity.y += jumpPower;
                    //ジャンプした位置を記録
                    jumpPos = transform.position.y;
                    //ジャンプ判定を有効
                    isJump = true;
                }
                //スペースキーを離した時
                else
                {
                    //ジャンプ上昇アニメーションは無効
                    animator.SetBool("JumpUp", false);
                    //ジャンプ判定を無効
                    isJump = false;
                    //上に掛かる速度を0
                    velocity.y = 0;
                }
            }
            //着地判定でない時
            else
            {
                //足音を無効化
                GetComponent<AudioSource>().volume = 0;
                //ジャンプ判定時
                if (isJump)
                {
                    //掛かっている速度をリセット(しないとどっか飛んでく)
                    velocity = Vector3.zero;
                    //ジャンプキーを押している判定
                    bool pushJumpKey = Input.GetKey(KeyCode.Space);
                    //現在の高さがジャンプ可能な高さかの判定
                    bool canHeight = jumpPos + jumpHeight > transform.position.y;

                    //ジャンプキーを押している且つ、ジャンプ可能な高さの時
                    if (pushJumpKey && canHeight)
                    {
                        //ジャンプ上昇アニメーションを再生
                        animator.SetBool("JumpUp", true);
                        //上方向にジャンプを代入し続ける
                        velocity.y = jumpPower;
                    }
                    //ジャンプキーを押していない、またはジャンプ可能高さにいない時
                    else if (!pushJumpKey || !canHeight)
                    {
                        //ジャンプ上昇アニメーションを無効
                        animator.SetBool("JumpUp", false);
                        //ジャンプ落下アニメーションを再生
                        animator.SetBool("JumpFall", true);
                        //ジャンプ判定を無効
                        isJump = false;
                    }

                    //ジャンプ上昇中であれば移動可能（本当は落下中も移動したい）
                    //入力の長さが0.1より大きい時且つ、着地アニメーションが終わっている時
                    if (input.magnitude > 0.1f)
                    {
                        //入力した方向へ向かせる
                        transform.LookAt(transform.position + input.normalized);
                        //移動する
                        velocity += transform.forward * RunSpeed;
                    }
                    else
                    {
                        velocity += transform.forward * 0f;
                    }
                }
                //ジャンプ判定でも着地判定でもない時(走っていて落ちた時)
                else
                {
                    //足音を無効化
                    GetComponent<AudioSource>().volume = 0;
                }
            }
            //重力を常にかける
            velocity.y += Physics.gravity.y * Time.deltaTime;

        }
    }

    void FixedUpdate()
    {
        //キャラクターを移動させる処理
        rigid.MovePosition(transform.position + velocity * Time.fixedDeltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.tag);
        //敵と接触した時
        if (collision.collider.tag == "EnemyTag")
        {
            Debug.Log("敵と接触した");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //ゲームオーバーエリアに落ちた時
        if (other.gameObject.tag == "GameOverAreaTag")
        {
            //ゲームオーバー関数を呼び出す
            GameObject.Find(StageController.STR).GetComponent<StageController>().PlayerDown();
        }
        //イベントアイテムに衝突した時
        if (other.gameObject.tag == "EventItemTag")
        {
            //パーティクルを再生
            GetComponent<ParticleSystem>().Play();
        }
    }
}
