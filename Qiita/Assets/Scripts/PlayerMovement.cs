using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    /// <summary> 移動操作を受け付けるタッチエリア </summary>
    [SerializeField]
    private DragHandler _moveController;

    /// <summary> 移動速度（m/秒） </summary>
    [SerializeField]
    private float _movePerSecond = 7f;

    /// <summary> 移動操作としてタッチ開始したスクリーン座標 </summary>
    private Vector2 _movePointerPosBegin;

    private Vector3 _moveVector;

    [SerializeField]
    private Camera _camera;

    /// <summary> カメラ操作を受け付けるタッチエリア </summary>
    [SerializeField]
    private DragHandler _lookController;

    /// <summary> カメラ速度（°/px） </summary>
    [SerializeField]
    private float _angularPerPixel = 0.3f;

    /// <summary> カメラ操作として前フレームにタッチしたキャンバス上の座標 </summary>
    private Vector2 _lookPointerPosPre;

    /// <summary> 起動時 </summary>
    private void Awake()
    {
        _moveController.OnBeginDragEvent += OnBeginDragMove;
        _moveController.OnDragEvent += OnDragMove;
        _moveController.OnEndDragEvent += OnEndDragMove;

        _lookController.OnBeginDragEvent += OnBeginDragLook;
        _lookController.OnDragEvent += OnDragLook;
    }

    /// <summary> 更新処理 </summary>
    private void Update()
    {
        UpdateMove(_moveVector);
        
        float horizotalValue = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        transform.position += this.transform.forward * vertical * 0.3f + this.transform.right * horizotalValue * 0.3f;

    }

    ////////////////////////////////////////////////////////////
    /// 移動操作
    ////////////////////////////////////////////////////////////
    #region Move

    /// <summary> ドラッグ操作開始（移動用） </summary>
    private void OnBeginDragMove(PointerEventData eventData)
    {
        Debug.Log("OnBeginDragMove");
        // タッチ開始位置を保持
        _movePointerPosBegin = eventData.position;
    }

    /// <summary> ドラッグ操作中（移動用） </summary>
    private void OnDragMove(PointerEventData eventData)
    {
        // タッチ開始位置からのスワイプ量を移動ベクトルにする
        var vector = eventData.position - _movePointerPosBegin;
        _moveVector = new Vector3(vector.x, 0f, vector.y);
    }

    private void UpdateMove(Vector3 vector)
    {
        // 現在向きを基準に、入力されたベクトルに向かって移動
        transform.position += transform.rotation * vector.normalized * _movePerSecond * Time.deltaTime;
    }

    /// <summary> ドラッグ操作終了（移動用） </summary>
    private void OnEndDragMove(PointerEventData eventData)
    {
        // 移動ベクトルを解消
        _moveVector = Vector3.zero;
    }
    #endregion

    ////////////////////////////////////////////////////////////
    /// カメラ操作
    ////////////////////////////////////////////////////////////
    #region Look
    /// <summary> ドラッグ操作開始（カメラ用） </summary>
    private void OnBeginDragLook(PointerEventData eventData)
    {
        _lookPointerPosPre = _lookController.GetPositionOnCanvas(eventData.position);
    }

    /// <summary> ドラッグ操作中（カメラ用） </summary>
    private void OnDragLook(PointerEventData eventData)
    {
        var pointerPosOnCanvas = _lookController.GetPositionOnCanvas(eventData.position);
        // キャンバス上で前フレームから何px操作したかを計算
        var vector = pointerPosOnCanvas - _lookPointerPosPre;
        // 操作量に応じてカメラを回転
        LookRotate(new Vector2(-vector.y, vector.x));
        _lookPointerPosPre = pointerPosOnCanvas;
    }

    private void LookRotate(Vector2 angles)
    {
        Vector2 deltaAngles = angles * _angularPerPixel;
        transform.eulerAngles += new Vector3(0f, deltaAngles.y);
        _camera.transform.localEulerAngles += new Vector3(deltaAngles.x, 0f);
    }
    #endregion
}

