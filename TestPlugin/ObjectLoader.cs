using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public struct FaceIndices
{
    public int vi;
    public int vu;
    public int vn;
}

public class GeometryBuffer
{
    public static int MAX_VERTICES_LIMIT_FOR_A_MESH = 64999;

    private readonly List<ObjectData> objects;

    private GroupData curgr;

    private ObjectData current;
    public List<Vector3> normals;
    public int unnamedGroupIndex = 1; // naming index for unnamed group. like "Unnamed-1"
    public List<Vector2> uvs;
    public List<Vector3> vertices;

    public GeometryBuffer()
    {
        objects = new List<ObjectData>();
        var d = new ObjectData();
        d.name = "default";
        objects.Add(d);
        current = d;

        var g = new GroupData();
        g.name = "default";
        d.groups.Add(g);
        curgr = g;

        vertices = new List<Vector3>();
        uvs = new List<Vector2>();
        normals = new List<Vector3>();
    }

    public int numObjects => objects.Count;
    public bool isEmpty => vertices.Count == 0;
    public bool hasUVs => uvs.Count > 0;
    public bool hasNormals => normals.Count > 0;

    public void PushObject(string name)
    {
        Debug.Log("Adding new object " + name + ". Current is empty: " + isEmpty);
        if (isEmpty) objects.Remove(current);

        var n = new ObjectData();
        n.name = name;
        objects.Add(n);

        var g = new GroupData();
        g.name = "default";
        n.groups.Add(g);

        curgr = g;
        current = n;
    }

    public void PushGroup(string name)
    {
        if (curgr.isEmpty) current.groups.Remove(curgr);
        var g = new GroupData();
        if (name == null)
        {
            name = "Unnamed-" + unnamedGroupIndex;
            unnamedGroupIndex++;
        }
        g.name = name;
        current.groups.Add(g);
        curgr = g;
    }

    public void PushMaterialName(string name)
    {
        Debug.Log("Pushing new material " + name + " with curgr.empty=" + curgr.isEmpty);
        if (!curgr.isEmpty) PushGroup(name);
        if (curgr.name == "default") curgr.name = name;
        curgr.materialName = name;
    }

    public void PushVertex(Vector3 v)
    {
        vertices.Add(v);
    }

    public void PushUV(Vector2 v)
    {
        uvs.Add(v);
    }

    public void PushNormal(Vector3 v)
    {
        normals.Add(v);
    }

    public void PushFace(FaceIndices f)
    {
        curgr.faces.Add(f);
        current.allFaces.Add(f);
        if (f.vn >= 0)
            current.normalCount++;
    }

    public void Trace()
    {
        Debug.Log("OBJ has " + objects.Count + " object(s)");
        Debug.Log("OBJ has " + vertices.Count + " vertice(s)");
        Debug.Log("OBJ has " + uvs.Count + " uv(s)");
        Debug.Log("OBJ has " + normals.Count + " normal(s)");
        foreach (var od in objects)
        {
            Debug.Log(od.name + " has " + od.groups.Count + " group(s)");
            foreach (var gd in od.groups)
                Debug.Log(od.name + "/" + gd.name + " has " + gd.faces.Count + " faces(s)");
        }
    }

    public void PopulateMeshes(GameObject[] gs, Dictionary<string, Material> mats)
    {
        if (gs.Length != numObjects) return; // Should not happen unless obj file is corrupt...
        Debug.Log("PopulateMeshes GameObjects count:" + gs.Length);
        for (var i = 0; i < gs.Length; i++)
        {
            var od = objects[i];
            var objectHasNormals = hasNormals && od.normalCount > 0;

            if (od.name != "default") gs[i].name = od.name;
            Debug.Log("PopulateMeshes object name:" + od.name);

            var tvertices = new Vector3[od.allFaces.Count];
            var tuvs = new Vector2[od.allFaces.Count];
            var tnormals = new Vector3[od.allFaces.Count];

            var k = 0;
            foreach (var fi in od.allFaces)
            {
                if (k >= MAX_VERTICES_LIMIT_FOR_A_MESH)
                {
                    Debug.LogWarning("maximum vertex number for a mesh exceeded for object:" + gs[i].name);
                    break;
                }
                tvertices[k] = vertices[fi.vi];
                if (hasUVs) tuvs[k] = uvs[fi.vu];
                if (hasNormals && fi.vn >= 0) tnormals[k] = normals[fi.vn];
                k++;
            }

            var m = (gs[i].GetComponent(typeof(MeshFilter)) as MeshFilter).mesh;
            m.vertices = tvertices;
            if (hasUVs) m.uv = tuvs;
            if (objectHasNormals) m.normals = tnormals;

            if (od.groups.Count == 1)
            {
                Debug.Log("PopulateMeshes only one group: " + od.groups[0].name);
                var gd = od.groups[0];
                var matName =
                    gd.materialName != null ? gd.materialName : "default"; // MAYBE: "default" may not enough.
                if (mats.ContainsKey(matName))
                {
                    gs[i].GetComponent<Renderer>().material = mats[matName];
                    Debug.Log("PopulateMeshes mat:" + matName + " set.");
                }
                else
                {
                    Debug.LogWarning("PopulateMeshes mat:" + matName + " not found.");
                }
                var triangles = new int[gd.faces.Count];
                for (var j = 0; j < triangles.Length; j++) triangles[j] = j;

                m.triangles = triangles;
            }
            else
            {
                var gl = od.groups.Count;
                var materials = new Material[gl];
                m.subMeshCount = gl;
                var c = 0;

                Debug.Log("PopulateMeshes group count:" + gl);
                for (var j = 0; j < gl; j++)
                {
                    var matName = od.groups[j].materialName != null
                        ? od.groups[j].materialName
                        : "default"; // MAYBE: "default" may not enough.
                    if (mats.ContainsKey(matName))
                    {
                        materials[j] = mats[matName];
                        Debug.Log("PopulateMeshes mat:" + matName + " set.");
                    }
                    else
                    {
                        Debug.LogWarning("PopulateMeshes mat:" + matName + " not found.");
                    }

                    var triangles = new int[od.groups[j].faces.Count];
                    var l = od.groups[j].faces.Count + c;
                    var s = 0;
                    for (; c < l; c++, s++) triangles[s] = c;
                    m.SetTriangles(triangles, j);
                }

                gs[i].GetComponent<Renderer>().materials = materials;
            }
            if (!objectHasNormals)
                m.RecalculateNormals();
        }
    }

    private class ObjectData
    {
        public readonly List<FaceIndices> allFaces;
        public readonly List<GroupData> groups;
        public string name;
        public int normalCount;

        public ObjectData()
        {
            groups = new List<GroupData>();
            allFaces = new List<FaceIndices>();
            normalCount = 0;
        }
    }

    private class GroupData
    {
        public readonly List<FaceIndices> faces;
        public string materialName;
        public string name;

        public GroupData()
        {
            faces = new List<FaceIndices>();
        }

        public bool isEmpty => faces.Count == 0;
    }
}

public class OBJ : MonoBehaviour
{
    /* OBJ file tags */
    private const string O = "o";

    private const string G = "g";
    private const string V = "v";
    private const string VT = "vt";
    private const string VN = "vn";
    private const string F = "f";
    private const string MTL = "mtllib";
    private const string UML = "usemtl";

    /* MTL file tags */
    private const string NML = "newmtl";

    private const string NS = "Ns"; // Shininess
    private const string KA = "Ka"; // Ambient component (not supported)
    private const string KD = "Kd"; // Diffuse component
    private const string KS = "Ks"; // Specular component
    private const string D = "d"; // Transparency (not supported)
    private const string TR = "Tr"; // Same as 'd'
    private const string ILLUM = "illum"; // Illumination model. 1 - diffuse, 2 - specular
    private const string MAP_KA = "map_Ka"; // Ambient texture
    private const string MAP_KD = "map_Kd"; // Diffuse texture
    private const string MAP_KS = "map_Ks"; // Specular texture
    private const string MAP_KE = "map_Ke"; // Emissive texture
    private const string MAP_BUMP = "map_bump"; // Bump map texture
    private const string BUMP = "bump"; // Bump map texture

    private string basepath;
    private GeometryBuffer buffer;

    /* ############## MATERIALS */
    private List<MaterialData> materialData;

    private string mtllib;


    public string objPath;

    private bool hasMaterials => mtllib != null;

    private void Start()
    {
        buffer = new GeometryBuffer();
        StartCoroutine(Load(objPath));
    }

    public IEnumerator Load(string path)
    {
        basepath = path.IndexOf("/", StringComparison.Ordinal) == -1
            ? ""
            : path.Substring(0, path.LastIndexOf("/") + 1);

        var loader = new WWW(path);
        yield return loader;
        SetGeometryData(loader.text);

        if (hasMaterials)
        {
            loader = new WWW(basepath + mtllib);
            Debug.Log("base path = " + basepath);
            Debug.Log("MTL path = " + basepath + mtllib);
            yield return loader;
            if (loader.error != null)
                Debug.LogError(loader.error);
            else
                SetMaterialData(loader.text);

            foreach (var m in materialData)
            {
                if (m.diffuseTexPath != null)
                {
                    var texloader = GetTextureLoader(m, m.diffuseTexPath);
                    yield return texloader;
                    if (texloader.error != null)
                        Debug.LogError(texloader.error);
                    else
                        m.diffuseTex = texloader.texture;
                }
                if (m.bumpTexPath != null)
                {
                    var texloader = GetTextureLoader(m, m.bumpTexPath);
                    yield return texloader;
                    if (texloader.error != null)
                        Debug.LogError(texloader.error);
                    else
                        m.bumpTex = texloader.texture;
                }
            }
        }

        Build();
    }

    private WWW GetTextureLoader(MaterialData m, string texpath)
    {
        char[] separators = {'/', '\\'};
        var components = texpath.Split(separators);
        var filename = components[components.Length - 1];
        var ext = Path.GetExtension(filename).ToLower();
        if (ext != ".png" && ext != ".jpg")
            Debug.LogWarning("maybe unsupported texture format:" + ext);
        var texloader = new WWW(basepath + filename);
        Debug.Log("texture path for material(" + m.name + ") = " + basepath + filename);
        return texloader;
    }

    private void GetFaceIndicesByOneFaceLine(FaceIndices[] faces, string[] p, bool isFaceIndexPlus)
    {
        if (isFaceIndexPlus)
        {
            for (var j = 1; j < p.Length; j++)
            {
                var c = p[j].Trim().Split("/".ToCharArray());
                var fi = new FaceIndices();
                // vertex
                var vi = ci(c[0]);
                fi.vi = vi - 1;
                // uv
                if (c.Length > 1 && c[1] != "")
                {
                    var vu = ci(c[1]);
                    fi.vu = vu - 1;
                }
                // normal
                if (c.Length > 2 && c[2] != "")
                {
                    var vn = ci(c[2]);
                    fi.vn = vn - 1;
                }
                else
                {
                    fi.vn = -1;
                }
                faces[j - 1] = fi;
            }
        }
        else
        {
            // for minus index
            var vertexCount = buffer.vertices.Count;
            var uvCount = buffer.uvs.Count;
            for (var j = 1; j < p.Length; j++)
            {
                var c = p[j].Trim().Split("/".ToCharArray());
                var fi = new FaceIndices();
                // vertex
                var vi = ci(c[0]);
                fi.vi = vertexCount + vi;
                // uv
                if (c.Length > 1 && c[1] != "")
                {
                    var vu = ci(c[1]);
                    fi.vu = uvCount + vu;
                }
                // normal
                if (c.Length > 2 && c[2] != "")
                {
                    var vn = ci(c[2]);
                    fi.vn = vertexCount + vn;
                }
                else
                {
                    fi.vn = -1;
                }
                faces[j - 1] = fi;
            }
        }
    }

    private void SetGeometryData(string data)
    {
        var lines = data.Split("\n".ToCharArray());
        var regexWhitespaces = new Regex(@"\s+");
        var isFirstInGroup = true;
        var isFaceIndexPlus = true;
        for (var i = 0; i < lines.Length; i++)
        {
            var l = lines[i].Trim();

            if (l.IndexOf("#", StringComparison.Ordinal) != -1)
                continue;
            var p = regexWhitespaces.Split(l);
            switch (p[0])
            {
                case O:
                    buffer.PushObject(p[1].Trim());
                    isFirstInGroup = true;
                    break;
                case G:
                    string groupName = null;
                    if (p.Length >= 2)
                        groupName = p[1].Trim();
                    isFirstInGroup = true;
                    buffer.PushGroup(groupName);
                    break;
                case V:
                    buffer.PushVertex(new Vector3(cf(p[1]), cf(p[2]), cf(p[3])));
                    break;
                case VT:
                    buffer.PushUV(new Vector2(cf(p[1]), cf(p[2])));
                    break;
                case VN:
                    buffer.PushNormal(new Vector3(cf(p[1]), cf(p[2]), cf(p[3])));
                    break;
                case F:
                    var faces = new FaceIndices[p.Length - 1];
                    if (isFirstInGroup)
                    {
                        isFirstInGroup = false;
                        var c = p[1].Trim().Split("/".ToCharArray());
                        isFaceIndexPlus = ci(c[0]) >= 0;
                    }
                    GetFaceIndicesByOneFaceLine(faces, p, isFaceIndexPlus);
                    if (p.Length == 4)
                    {
                        buffer.PushFace(faces[0]);
                        buffer.PushFace(faces[1]);
                        buffer.PushFace(faces[2]);
                    }
                    else if (p.Length == 5)
                    {
                        buffer.PushFace(faces[0]);
                        buffer.PushFace(faces[1]);
                        buffer.PushFace(faces[3]);
                        buffer.PushFace(faces[3]);
                        buffer.PushFace(faces[1]);
                        buffer.PushFace(faces[2]);
                    }
                    else
                    {
                        Debug.LogWarning("face vertex count :" + (p.Length - 1) + " larger than 4:");
                    }
                    break;
                case MTL:
                    mtllib = l.Substring(p[0].Length + 1).Trim();
                    break;
                case UML:
                    buffer.PushMaterialName(p[1].Trim());
                    break;
            }
        }

        // buffer.Trace();
    }

    private static float cf(string v)
    {
        try
        {
            return float.Parse(v);
        }
        catch (Exception e)
        {
            print(e);
            return 0;
        }
    }

    private static int ci(string v)
    {
        try
        {
            return int.Parse(v);
        }
        catch (Exception e)
        {
            print(e);
            return 0;
        }
    }

    private void SetMaterialData(string data)
    {
        var lines = data.Split("\n".ToCharArray());

        materialData = new List<MaterialData>();
        var current = new MaterialData();
        var regexWhitespaces = new Regex(@"\s+");

        for (var i = 0; i < lines.Length; i++)
        {
            var l = lines[i].Trim();

            if (l.IndexOf("#", StringComparison.Ordinal) != -1)
                l = l.Substring(0, l.IndexOf("#", StringComparison.Ordinal));
            var p = regexWhitespaces.Split(l);
            if (p[0].Trim() == "") continue;

            switch (p[0])
            {
                case NML:
                    current = new MaterialData();
                    current.name = p[1].Trim();
                    materialData.Add(current);
                    break;
                case KA:
                    current.ambient = gc(p);
                    break;
                case KD:
                    current.diffuse = gc(p);
                    break;
                case KS:
                    current.specular = gc(p);
                    break;
                case NS:
                    current.shininess = cf(p[1]) / 1000;
                    break;
                case D:
                case TR:
                    current.alpha = cf(p[1]);
                    break;
                case MAP_KD:
                    current.diffuseTexPath = p[p.Length - 1].Trim();
                    break;
                case MAP_BUMP:
                case BUMP:
                    BumpParameter(current, p);
                    break;
                case ILLUM:
                    current.illumType = ci(p[1]);
                    break;
                default:
                    Debug.Log("this line was not processed :" + l);
                    break;
            }
        }
    }

    private Material GetMaterial(MaterialData md)
    {
        Material m;

        if (md.illumType == 2)
        {
            var shaderName = md.bumpTex != null ? "Bumped Specular" : "Specular";
            m = new Material(Shader.Find(shaderName));
            m.SetColor("_SpecColor", md.specular);
            m.SetFloat("_Shininess", md.shininess);
        }
        else
        {
            var shaderName = md.bumpTex != null ? "Bumped Diffuse" : "Diffuse";
            m = new Material(Shader.Find(shaderName));
        }

        if (md.diffuseTex != null)
            m.SetTexture("_MainTex", md.diffuseTex);
        else
            m.SetColor("_Color", md.diffuse);
        if (md.bumpTex != null) m.SetTexture("_BumpMap", md.bumpTex);

        m.name = md.name;

        return m;
    }

    private void BumpParameter(MaterialData m, string[] p)
    {
        var regexNumber = new Regex(@"^[-+]?[0-9]*\.?[0-9]+$");

        var bumpParams = new Dictionary<string, BumpParamDef>();
        bumpParams.Add("bm", new BumpParamDef("bm", "string", 1, 1));
        bumpParams.Add("clamp", new BumpParamDef("clamp", "string", 1, 1));
        bumpParams.Add("blendu", new BumpParamDef("blendu", "string", 1, 1));
        bumpParams.Add("blendv", new BumpParamDef("blendv", "string", 1, 1));
        bumpParams.Add("imfchan", new BumpParamDef("imfchan", "string", 1, 1));
        bumpParams.Add("mm", new BumpParamDef("mm", "string", 1, 1));
        bumpParams.Add("o", new BumpParamDef("o", "number", 1, 3));
        bumpParams.Add("s", new BumpParamDef("s", "number", 1, 3));
        bumpParams.Add("t", new BumpParamDef("t", "number", 1, 3));
        bumpParams.Add("texres", new BumpParamDef("texres", "string", 1, 1));
        var pos = 1;
        string filename = null;
        while (pos < p.Length)
        {
            if (!p[pos].StartsWith("-"))
            {
                filename = p[pos];
                pos++;
                continue;
            }
            // option processing
            var optionName = p[pos].Substring(1);
            pos++;
            if (!bumpParams.ContainsKey(optionName))
                continue;
            var def = bumpParams[optionName];
            var args = new ArrayList();
            var i = 0;
            var isOptionNotEnough = false;
            for (; i < def.valueNumMin; i++, pos++)
            {
                if (pos >= p.Length)
                {
                    isOptionNotEnough = true;
                    break;
                }
                if (def.valueType == "number")
                {
                    var match = regexNumber.Match(p[pos]);
                    if (!match.Success)
                    {
                        isOptionNotEnough = true;
                        break;
                    }
                }
                args.Add(p[pos]);
            }
            if (isOptionNotEnough)
            {
                Debug.Log("bump variable value not enough for option:" + optionName + " of material:" + m.name);
                continue;
            }
            for (; i < def.valueNumMax && pos < p.Length; i++, pos++)
            {
                if (def.valueType == "number")
                {
                    var match = regexNumber.Match(p[pos]);
                    if (!match.Success)
                        break;
                }
                args.Add(p[pos]);
            }
            // TODO: some processing of options
            Debug.Log("found option: " + optionName + " of material: " + m.name + " args: " +
                      string.Concat(args.ToArray()));
        }
        if (filename != null)
            m.bumpTexPath = filename;
    }

    private Color gc(string[] p)
    {
        return new Color(cf(p[1]), cf(p[2]), cf(p[3]));
    }

    private void Build()
    {
        var materials = new Dictionary<string, Material>();

        if (hasMaterials)
            foreach (var md in materialData)
            {
                if (materials.ContainsKey(md.name))
                {
                    Debug.LogWarning("duplicate material found: " + md.name + ". ignored repeated occurences");
                    continue;
                }
                materials.Add(md.name, GetMaterial(md));
            }
        else
            materials.Add("default", new Material(Shader.Find("VertexLit")));

        var ms = new GameObject[buffer.numObjects];

        if (buffer.numObjects == 1)
        {
            gameObject.AddComponent(typeof(MeshFilter));
            gameObject.AddComponent(typeof(MeshRenderer));
            ms[0] = gameObject;
        }
        else if (buffer.numObjects > 1)
        {
            for (var i = 0; i < buffer.numObjects; i++)
            {
                var go = new GameObject();
                go.transform.parent = gameObject.transform;
                go.AddComponent(typeof(MeshFilter));
                go.AddComponent(typeof(MeshRenderer));
                ms[i] = go;
            }
        }

        buffer.PopulateMeshes(ms, materials);
    }

    private class MaterialData
    {
        public float alpha;
        public Color ambient;
        public Texture2D bumpTex;
        public string bumpTexPath;
        public Color diffuse;
        public Texture2D diffuseTex;
        public string diffuseTexPath;
        public int illumType;
        public string name;
        public float shininess;
        public Color specular;
    }

    private class BumpParamDef
    {
        public readonly int valueNumMax;
        public readonly int valueNumMin;
        public readonly string valueType;
        public string optionName;

        public BumpParamDef(string name, string type, int numMin, int numMax)
        {
            optionName = name;
            valueType = type;
            valueNumMin = numMin;
            valueNumMax = numMax;
        }
    }
}