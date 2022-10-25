using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class CpiUI : MonoBehaviour
{
    [SerializeField]
    private Image m_Hand;

    [SerializeField]
    private Sprite m_Click;
    [SerializeField]
    private Sprite m_Normal;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_Hand.transform.position = Input.mousePosition + Vector3.left;
        }

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Down());
        }

        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(Up());
        }
    }

    IEnumerator Down()
    {
        m_Hand.sprite = m_Normal;
        m_Hand.enabled = true;
        yield return new WaitForSeconds(.05f);
        m_Hand.sprite = m_Click;
    }

    IEnumerator Up()
    {
        m_Hand.sprite = m_Normal;

        yield return new WaitForSeconds(.3f);
        m_Hand.enabled = false;
    }
}