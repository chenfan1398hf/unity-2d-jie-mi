using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    // 定义移动速度
    public float moveSpeed = 5f;

    void Update()
    {
        // 获取水平和垂直输入
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // 归一化移动向量，确保对角线移动速度相同
        movement.Normalize();

        // 应用移动
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // 处理物体翻转
        if (horizontalInput > 0)
        {
            // 向右移动，确保物体不翻转
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            // 向左移动，翻转物体
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "men")
        {
            GameManager.instance.OpenMiMaPanel();
        }
        if (collision.gameObject.tag == "zhangyu")
        {
            GameManager.instance.OpenPanel2();
        }
        if (collision.gameObject.tag == "shuimu")
        {
            GameManager.instance.OpenPanel3();
        }
    }

}
