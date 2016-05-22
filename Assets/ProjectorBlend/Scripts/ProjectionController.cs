using UnityEngine;
using System.Linq;
using sugi.cc;

public class ProjectionController : MonoBehaviour
{
    static ProjectionController Instance { get { if (_Instance == null) _Instance = FindObjectOfType<ProjectionController>(); return _Instance; } }
    static ProjectionController _Instance;

    public int camDepth = 100;
    [SerializeField]
    Setting setting;
    [SerializeField]
    Texture texture;
    public void SetTexture(Texture tex) { texture = tex; }

    Projection[] projections;
    // Use this for initialization
    void Start()
    {
        const string filePath = "Projection/Setting.json";
        SettingManager.AddSettingMenu(setting, filePath);
    }

    void BuildProjections()
    {
        var numX = setting.numX;
        var numY = setting.numY;
        var numProjections = numX * numY;

        if (projections != null && projections.Length != numProjections)
        {
            projections.ToList().ForEach(b => Destroy(b.gameObject));
            projections = null;
        }
        if (projections == null)
        {
            projections = new Projection[numProjections];
            for (var i = 0; i < numProjections; i++)
            {
                var p = projections[i] = new GameObject("projection").AddComponent<Projection>();
                p.transform.parent = transform;
            }
        }

        var totalUvX = 1f + setting.blendHorizonalRanges.Sum();
        var totalUvY = 1f + setting.blendVerticalRanges.Sum();
        var deltaUvX = totalUvX / numX;
        var deltaUvY = totalUvY / numY;

        var uvY = 0f;
        for (var y = 0; y < numY; y++)
        {
            var blendingsY = setting.blendVerticalRanges;
            var olY0 = 0f;
            var olY1 = 0f;
            if (0 < y)
            {
                uvY -= blendingsY[y - 1];
                olY0 = blendingsY[y - 1] / deltaUvY;
            }
            if (y < blendingsY.Length)
                olY1 = blendingsY[y] / deltaUvY;

            var uvX = 0f;
            for (var x = 0; x < numX; x++)
            {
                var index = numX * y + x;
                var projection = projections[index];
                var blendingsX = setting.blendHorizonalRanges;
                var olX0 = 0f;
                var olX1 = 0f;

                if (0 < x)
                {
                    uvX -= blendingsX[x - 1];
                    olX0 = blendingsX[x - 1] / deltaUvX;
                }
                if (x < blendingsX.Length)
                    olX1 = blendingsX[x] / deltaUvX;

                var projArea = new Rect((float)x / numX, (float)y / numY, 1f / numX, 1f / numY);
                var trimArea = new Rect(uvX, uvY, deltaUvX, deltaUvY);
                projection.texture = texture;
                projection.projectionArea = projArea;
                projection.trimArea = trimArea;
                projection.overlapProps = new Vector4(olX0, olX1, olY0, olY1);
                projection.camDepth = camDepth;
                projection.quadWarpProps.bottomLeft = setting.quadWarpProps[index * 4 + 0];
                projection.quadWarpProps.bottomRight = setting.quadWarpProps[index * 4 + 1];
                projection.quadWarpProps.upperLeft = setting.quadWarpProps[index * 4 + 2];
                projection.quadWarpProps.upperRight = setting.quadWarpProps[index * 4 + 3];
                projection.gamma = 1f / setting.gamma;
                projection.Setup();

                uvX += deltaUvX;
            }
            uvY += deltaUvY;
        }
    }

    void SetProjectionSetting() { }

    [System.Serializable]
    public class Setting : SettingManager.Setting
    {
        public int numX = 2;
        public int numY = 1;
        public float[] blendVerticalRanges;
        public float[] blendHorizonalRanges;
        public Vector2[] quadWarpProps;
        public float gamma = 2.2f;

        int editingQuadWarpIdx;

        public override void OnGUIFunc()
        {
            numX = (int)FloatField("num horizonal", numX);
            numY = (int)FloatField("num vertical", numY);
            gamma = FloatField("blend gamma", gamma);

            ValidateVals();

            FloatArrayField("blend horizonal:", blendHorizonalRanges);
            FloatArrayField("blend vertical:", blendVerticalRanges);

            QuadWarpPropField();
        }

        void ValidateVals()
        {
            numX = Mathf.Max(1, numX);
            numY = Mathf.Max(1, numY);

            if (blendHorizonalRanges.Length != numX - 1)
                blendHorizonalRanges = new float[numX - 1];
            if (blendVerticalRanges.Length != numY - 1)
                blendVerticalRanges = new float[numY - 1];
            if (quadWarpProps.Length != numX * numY * 4)
            {
                quadWarpProps = Enumerable.Range(0, numX * numY * 4)
                    .Select(i => new Vector2(
                        (i % 4 == 1 || i % 4 == 3) ? 1f : 0f,
                        1 < i % 4 ? 1f : 0)
                    ).ToArray();
            }
            Instance.BuildProjections();
        }

        protected override void OnLoad()
        {
            ValidateVals();
            Instance.BuildProjections();
        }
        protected override void OnClose()
        {
            base.OnClose();
            Instance.BuildProjections();
        }

        float FloatField(string label, float val, float minVal = 0, float maxVal = 0)
        {
            GUILayout.BeginHorizontal();
            if (label.Length > 0)
                GUILayout.Label(string.Format("{0}:{1}", label, val));
            var data = GUILayout.TextField(val.ToString());
            float f;
            if (float.TryParse(data, out f))
                val = f;
            if (maxVal - minVal != 0)
                val = GUILayout.HorizontalSlider(val, minVal, maxVal);
            GUILayout.EndHorizontal();
            return val;
        }
        void FloatArrayField(string label, float[] vals)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            for (var i = 0; i < vals.Length; i++)
            {
                var val = vals[i];
                var data = GUILayout.TextField(val.ToString());
                if (float.TryParse(data, out val))
                    vals[i] = val;
            }
            GUILayout.EndHorizontal();
        }

        void QuadWarpPropField()
        {
            CenterLabel("---Quad Warp Props---");
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("select projector");
            GUILayout.FlexibleSpace();

            var titles = new string[numX * numY];
            for (var y = 0; y < numY; y++)
                for (var x = 0; x < numX; x++)
                {
                    var i = numX * (numY - 1 - y) + x;
                    titles[i] = string.Format("QuadWarp({0}-{1})", x, y);
                }
            editingQuadWarpIdx = GUILayout.SelectionGrid(editingQuadWarpIdx, titles, numX);

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            for (var y = numY - 1; y > -1; y--)
                for (var x = 0; x < numX; x++)
                {
                    var i = numX * (numY - 1 - y) + x;
                    if (i != editingQuadWarpIdx)
                        continue;

                    CenterLabel(string.Format("QuadWarp({0}-{1})", x, y));
                    GUILayout.BeginHorizontal();
                    quadWarpProps[4 * i + 2] = Vector2Field("UpperLeft:", quadWarpProps[4 * i + 2]);
                    quadWarpProps[4 * i + 3] = Vector2Field("UpperRight:", quadWarpProps[4 * i + 3]);
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    quadWarpProps[4 * i + 0] = Vector2Field("BottomLeft:", quadWarpProps[4 * i + 0]);
                    quadWarpProps[4 * i + 1] = Vector2Field("BottomRight:", quadWarpProps[4 * i + 1]);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(16f);
                }
        }
        void CenterLabel(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        Vector2 Vector2Field(string label, Vector2 vec2)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(GUILayout.MaxWidth(300f));
            vec2.x = FloatField("", vec2.x);
            vec2.y = FloatField("", vec2.y);
            GUILayout.EndHorizontal();
            GUILayout.EndHorizontal();
            return vec2;
        }
    }
}
