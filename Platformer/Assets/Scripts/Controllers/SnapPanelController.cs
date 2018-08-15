using UnityEngine;

public class SnapPanelController : MonoBehaviour
{
    #region VARIABLES

    [SerializeField]
    private RectTransform[] panels;
    [SerializeField]
    private RectTransform centerToCompare;
    private RectTransform scrollPanel;

    [SerializeField]
    private float[] distances;
    [SerializeField]
    private float[] distanceReposition;
    private float cycleDistance = 4f;
    private float lerpSpeed = 6f;
    private bool isDragging = false;
    private int panelDistance;
    private int minPanelNumber;
    private int panelsLenght;
    private int startPanel = 1;

    #endregion VARIABLES

    private void Start()
    {
        scrollPanel = GetComponent<RectTransform>();
        panelsLenght = panels.Length;
        distances = new float[panelsLenght];
        distanceReposition = new float[panelsLenght];

        panelDistance = (int)Mathf.Abs(panels[1].anchoredPosition.x - panels[0].anchoredPosition.x);

        scrollPanel.anchoredPosition = new Vector2((startPanel - 1) * -scrollPanel.rect.width, 0);
    }

    private void Update()
    {
        for (int i = 0; i < panelsLenght; i++)
        {
            distanceReposition[i] = centerToCompare.position.x - panels[i].GetComponent<RectTransform>().position.x;
            distances[i] = Mathf.Abs(distanceReposition[i]);

            if(distanceReposition[i] > cycleDistance)
            {
                float currentX = panels[i].anchoredPosition.x;
                float currentY = panels[i].anchoredPosition.y;

                Vector2 newAnchoredPosition = new Vector2(currentX + (panelsLenght * panelDistance), currentY);
                panels[i].GetComponent<RectTransform>().anchoredPosition = newAnchoredPosition;
            }

            if (distanceReposition[i] < -cycleDistance)
            {
                float currentX = panels[i].anchoredPosition.x;
                float currentY = panels[i].anchoredPosition.y;

                Vector2 newAnchoredPosition = new Vector2(currentX - (panelsLenght * panelDistance), currentY);
                panels[i].anchoredPosition = newAnchoredPosition;
            }
        }

        float minDistance = Mathf.Min(distances);

        for (int a = 0; a < panels.Length; a++)
        {
            if (minDistance == distances[a])
            {
                minPanelNumber = a;
            }
        }         

        if (!isDragging)
        {
            LerpToPanel(-panels[minPanelNumber].anchoredPosition.x);
        }
    }

    private void LerpToPanel(float position)
    {
        float newX = Mathf.Lerp(scrollPanel.anchoredPosition.x, position, Time.deltaTime * lerpSpeed);
        Vector2 newPosition = new Vector2(newX, scrollPanel.anchoredPosition.y);
        scrollPanel.anchoredPosition = newPosition;
    }

    public void BegingDrag()
    {
        isDragging = true;
    }

    public void EndDrag()
    {
        isDragging = false;
        SfxLibrary.Instance.PlayUISfx("Swipe");
    }
}
