using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    // �����ƶ��ٶ�
    public float moveSpeed = 5f;

    void Update()
    {
        // ��ȡˮƽ�ʹ�ֱ����
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // �����ƶ�����
        Vector2 movement = new Vector2(horizontalInput, verticalInput);

        // ��һ���ƶ�������ȷ���Խ����ƶ��ٶ���ͬ
        movement.Normalize();

        // Ӧ���ƶ�
        transform.Translate(movement * moveSpeed * Time.deltaTime);

        // �������巭ת
        if (horizontalInput > 0)
        {
            // �����ƶ���ȷ�����岻��ת
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (horizontalInput < 0)
        {
            // �����ƶ�����ת����
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
