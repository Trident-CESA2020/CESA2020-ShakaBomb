﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;  //シーン遷移

public class StageSelectDirector : MonoBehaviour
{
    //選択するスピード
    public float speed;
    //選択するステージの名前(※build settingにシーンを登録しておく事)
    public string[] stage_names;
    //タイトル画面に戻るための無操作時間(単位：分)
    public float to_title_not_operate_minute;
    //カメラオブジェクト
    GameObject go_camera;
    //選択フレーム
    GameObject go_select_tex;
    //タイトルボタン
    GameObject go_title_button;
    //ステージの画像
    GameObject[] go_stage_tex;
    //選んでいるステージ番号
    int stage_number;
    //選んでいたステージ番号
    int last_number;
    //円運動用角度
    float angle;
    //距離
    Vector3 stage_distance;
    //覚える座標
    Vector3 last_position;
    //タイトルボタンを選んでいるか
    bool select_title;
    //ステージを選択できる状態か
    bool select_stage;
    //何も操作しなくなった時の時間
    float not_operate_time;


    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトを探す
        go_camera = GameObject.Find("Main Camera");
        go_select_tex = GameObject.Find("SelectTex");
        go_title_button = GameObject.Find("TitleButton");
        //ステージ画像を探して数える
        int num_tex = GameObject.FindGameObjectsWithTag("StageTex").Length;
        //ステージ画像を記憶する配列のサイズを設定する
        go_stage_tex = new GameObject[num_tex];
        //並び替え
        for (int i = 0; i < go_stage_tex.Length; i++)
        {
            //オブジェクトを探す
            go_stage_tex[i] = GameObject.Find("StageTex (" + i.ToString() + ")");
        }
        //座標変更
        go_select_tex.transform.position = go_stage_tex[0].transform.position;
        //拡大率変更
        go_select_tex.transform.localScale = go_stage_tex[0].transform.localScale + new Vector3(0.5f, 0.5f, 0.0f);
        //初期化
        stage_number = 0;
        last_number = 0;
        angle = 0.0f;
        stage_distance = Vector2.zero;
        select_title = false;
        select_stage = true;
        not_operate_time = -1.0f;

        //ステージフレームに選ばれている番号を教える
        SetSelectStage(false);
        //ステージ全てをLineで通す
        AllStageLinePass();
    }

    // Update is called once per frame
    void Update()
    {
        //選択している画像が動いていなかったら
        if (angle == 0.0f)
        {
            //タイトルを選んでいなかったら
            if (select_title == false)
            {
                //拡大率を変える
                go_select_tex.transform.localScale = go_stage_tex[stage_number].transform.localScale + new Vector3(0.5f, 0.5f, 0.5f);

                //左を押したら
                if (Input.GetKeyDown(KeyCode.LeftArrow) && stage_number > 0)
                {
                    //ステージ番号を変更する準備をする(ステージ番号を1減らす)
                    PreparaChangeSelectStage(-1);
                }
                //右を押したら
                if (Input.GetKeyDown(KeyCode.RightArrow) && stage_number < go_stage_tex.Length - 1)
                {
                    //ステージ番号を変更する準備をする(ステージ番号を1増やす)
                    PreparaChangeSelectStage(1);
                }
                //下を押したら
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    //タイトルを選択している状態にする
                    select_title = true;
                    PreparaChangeTitle(select_title);
                    //ステージを選べない状態にする
                    select_stage = false;
                }
            }
            else
            {
                //上を押したら
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    //タイトルを選択していない状態にする
                    select_title = false;
                    PreparaChangeTitle(select_title);
                }
            }
            //Space(決定)を押したら
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //タイトルを選択していなかったら
                if (select_title == false)
                {
                    //登録した名前のステージのプレイシーンをロードする
                    SceneManager.LoadScene(stage_names[stage_number]);
                }
                else
                {
                    //タイトルシーンをロードする
                    SceneManager.LoadScene("TitleScene");
                }
            }
        }
        //動いている途中だったら
        else
        {
            //選択フレームを動かす
            go_select_tex.transform.position =
                last_position +
                new Vector3(
                (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.x * 0.5f),
                (Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * (stage_distance.y * 0.5f),
                0.0f);

            //ステージが選択できない状態(ステージ→タイトル　タイトル→ステージ　の場合)
            if (select_stage == false)
            {
                //タイトルを選択している時
                if (select_title)
                {
                    go_select_tex.transform.localScale = Vector3.Lerp(
                    go_stage_tex[stage_number].transform.localScale,
                    go_title_button.transform.localScale,
                    ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
                }
                else
                {
                    go_select_tex.transform.localScale = Vector3.Lerp(
                    go_title_button.transform.localScale,
                    go_stage_tex[stage_number].transform.localScale,
                    ((Mathf.Sin(Mathf.Deg2Rad * (angle - 90.0f)) + 1) * 0.5f));
                }
                go_select_tex.transform.localScale += new Vector3(0.5f, 0.5f, 0.0f);
            }

            //半周していなかったら
            if (angle < 180.0f)
            {
                angle += speed;

            }
            //半周していたら
            else
            {
                //タイトルを選択していなかったら
                if (select_title == false)
                {
                    //ステージを選択できない状態だったら
                    if (select_stage == false)
                    {
                        //選択できるようにする
                        select_stage = true;
                    }
                    //微調整する
                    go_select_tex.transform.position = go_stage_tex[stage_number].transform.position;
                }
                else
                {
                    go_select_tex.transform.position = go_title_button.transform.position;
                }
                //円運動が終わった事にする
                angle = 0.0f;
            }
        }

        //タイトルを選択していない状態で　ステージを選択できる状態
        if (select_title == false && select_stage == true)
        {
            go_camera.transform.position = new Vector3(go_select_tex.transform.position.x, 0.0f, -10.0f);
            go_title_button.transform.position = new Vector3(go_camera.transform.position.x + 6.5f, go_camera.transform.position.y + (-4.0f), 0.0f);
        }

        //操作していない時間を計る
        CountNotOperateTime();
    }

    //選択しているステージを変更する準備 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void PreparaChangeSelectStage(int number_difference)
    {
        //選択していたステージの座標を覚える
        last_position = go_stage_tex[stage_number].transform.position;
        //選択していたステージ番号を覚える
        last_number = stage_number;
        //ステージ番号を変更する
        stage_number += number_difference;
        //円運動を始める
        angle += speed;
        //選択していたステージと選択するステージの距離を求める
        stage_distance = go_stage_tex[stage_number].transform.position - go_stage_tex[last_number].transform.position;
        //ステージフレームに選ばれている番号を教える
        SetSelectStage(false);
    }

    //選択しているステージorタイトルを変更する準備 <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void PreparaChangeTitle(bool select)
    {
        //円運動を始める
        angle += speed;
        //タイトルを選択していたら
        if (select)
        {
            //選択していたステージの座標を覚える
            last_position = go_stage_tex[stage_number].transform.position;
            //タイトルと選択するステージの距離を求める
            stage_distance = go_title_button.transform.position - go_stage_tex[stage_number].transform.position;
            //ステージフレームに選ばれている番号を教える
            SetSelectStage(true);
        }
        else
        {
            //選択していたタイトルの座標を覚える
            last_position = go_title_button.transform.position;
            //選択するステージとタイトルの距離を求める
            stage_distance = go_stage_tex[stage_number].transform.position - go_title_button.transform.position;
            //ステージフレームに選ばれている番号を教える
            SetSelectStage(false);
        }
    }

    //選ばれているステージをステージフレーム側に知らせる
    void SetSelectStage(bool select_title)
    {
        //ステージフレームに選ばれている番号を教える
        for (int i = 0; i < go_stage_tex.Length; i++)
        {
            bool select = false;
            //タイトルが選ばれていなかったら
            if (select_title == false)
            {
                //選ばれている番号と同じだったら
                if (i == stage_number)
                {
                    select = true;
                }
            }
            go_stage_tex[i].GetComponent<StageFrameController>().SetNowSelect(select);
        }
    }

    //操作していない時間を計ってタイトルに戻るようにする <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void CountNotOperateTime()
    {
        //操作していなかったら
        if (!(Input.anyKey))
        {
            //操作していない時間を覚えていたら
            if (not_operate_time != -1.0f)
            {
                Debug.Log(Time.time - not_operate_time);
                //操作していない時間がタイトル画面に戻るための無操作時間以上経ったら
                if (Time.time - not_operate_time >= (to_title_not_operate_minute * 60.0f))
                {
                    SceneManager.LoadScene("TitleScene");
                }
            }
            //覚えていなかったら
            else
            {
                //操作していない時間を覚える
                not_operate_time = Time.time;
            }
        }
        //操作していたら
        else
        {
            //操作していない時間を忘れる
            not_operate_time = -1.0f;
        }
    }

    //ステージを全てLineで通す <自作関数> -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    void AllStageLinePass()
    {
        //ステージの数に対応して線を引く
        if (go_stage_tex.Length > 1)
        {
            //Lineオブジェクトを探す
            GameObject go_line = GameObject.Find("StageLine");
            //コンポーネントを設定する
            LineRenderer renderer = go_line.GetComponent<LineRenderer>();
            //Lineの座標の数を設定する
            renderer.positionCount = 1 + ((go_stage_tex.Length - 1) * 2);
            //Lineの座標の数分処理する
            for (int i = 0; i < renderer.positionCount; i++)
            {
                //偶数だったら
                if (i % 2 == 0)
                {
                    //ステージの座標に合わせる
                    renderer.SetPosition(i, go_stage_tex[i / 2].transform.position);
                }
                //奇数だったら
                else
                {
                    //xを前のステージの座標に　yを次のステージの座標にする
                    renderer.SetPosition(i, new Vector3(go_stage_tex[i / 2].transform.position.x, go_stage_tex[(i / 2) + 1].transform.position.y, 0.0f));
                }
            }
            //マテリアル設定
            renderer.materials[0].color = Color.red;
            //幅設定
            renderer.startWidth = 1.0f;
            renderer.endWidth = 1.0f;
        }
        else
        {
            Destroy(GameObject.Find("StageLine"));
        }
    }

}