using UnityEngine;

public class CamController : MonoBehaviour {
    private const float HorizontalSpeed = 3.0f;
    private const float VerticalSpeed = 3.0f;
    private const float AlphaSpeed = 0.4f;
    private const string HorizontalAxisName = "Horizontal";
    private const string VerticalAxisName = "Vertical";
    private const string HorizontalMouseAxisName = "Mouse Y";
    private const string VerticalMouseAxisName = "Mouse X";
    private const string BatTagName = "Bat";

    private float _h;
    private float _v;
    private float _alpha = -1f;
    [SerializeField] private Texture _texture;
    
    void Start() { }

    void OnGUI() {
        if (_alpha < 0) {
            _alpha = -1f;
            return;
        }

        GUI.color = new Color(255, 0, 0, _alpha);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _texture);
        _alpha -= AlphaSpeed * Time.deltaTime;
    }

    void FixedUpdate() {
        Control();
    }

    private void Control() {
        _h -= HorizontalSpeed * Input.GetAxis(HorizontalMouseAxisName);
        _v += VerticalSpeed * Input.GetAxis(VerticalMouseAxisName);
        transform.eulerAngles = new Vector3(_h, _v, 0);

        var h = Input.GetAxis(HorizontalAxisName);
        var v = Input.GetAxis(VerticalAxisName);
        transform.Translate(Vector3.forward * v * Time.deltaTime);
        transform.Translate(Vector3.right * h * Time.deltaTime);
    }

    void OnTriggerEnter(Component collision) {
        if (collision.gameObject.tag == BatTagName)
            _alpha = 1f;
    }
}