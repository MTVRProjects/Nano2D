////////////////////////////////////////
// author: Yu (Eric) Zhu              //
// email:  bluegenemontreal@gmail.com //
// date:   June 6, 2020               //
////////////////////////////////////////

using UnityEngine;
using System;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;
using UnityEngine.UI;
using HMLFramwork;

/// <summary>
/// 声子模型
/// </summary>
public class plot_phonon_modes : MonoBehaviour

{
    //parameters
    [Header("指定路径数据存在，则勾选")]
    public bool isPersistentData = true;
    string DataPath;
    public string DataPath_persistent = "/database04_MaterialsCloud/phonons_data/"; 

    public int supercell_M1 = 3;
    public int supercell_M2 = 3;    
    public float deltaR = 0.6f;
    public float r_bond = 0.3f;
    public float r_sphere = 1.0f;    

    Dictionary<string, float> radius_dict;
    Dictionary<string, Color> color_dict;

    int M0;
    int M1;
    int M2;
    int N_atom;
    int N_t;
    int I_t;

    string DatabaseIndex = "";
    string DatabaseIndex_ = "";
    int I_k_selected = -1;
    int I_b_selected = -1;
    float amplitude = 1f;
    float period = 2f;

    GameObject phonon_system;
    Transform phonon_system_trans;
    GameObject ButtonPanel;
    GameObject mode_panel_01;
    GameObject mode_panel_02;
    GameObject mode_panel_03;
    Canvas_database canvas00;
    Canvas_phonon canvas01;

    bool isInitialized = false;
    Vector3 Qpoint;
    Vector3[] AtomXYZ0_super;
    Vector3[] AtomXYZ1_super;
    string[] AtomSymbol_super;
    List<int[]> BondPairList = new List<int[]>();    

    JSONArray UnitcellVector;
    JSONArray AtomSymbolList;
    JSONArray AtomXYZList;
    JSONArray EigenvalueX;
    JSONArray EigenvalueY;
    JSONArray EigenvalueXtick;
    JSONArray EigenvectorAbs;
    JSONArray EigenvectorArg;
    JSONArray QpointList;

    // Awake
    void Awake()
    {
        // data path
        if (isPersistentData)
        {
            DataPath = DataPathHelper.Datas_path + DataPath_persistent;
        }

        // Find
        phonon_system = GameObject.Find("plot_phonon_modes");
        phonon_system_trans = phonon_system.transform;

        canvas00 = GameObject.Find("Canvas_database").GetComponent<Canvas_database>();
        canvas01 = GameObject.Find("Canvas_phonon").GetComponent<Canvas_phonon>();
        mode_panel_01 = GameObject.Find("Canvas_phonon").transform.Find("panel_modes").Find("show_phonon_modes").gameObject;
        mode_panel_02 = GameObject.Find("Canvas_phonon").transform.Find("panel_modes").Find("amplitude").gameObject;
        mode_panel_03 = GameObject.Find("Canvas_phonon").transform.Find("panel_modes").Find("frequency").gameObject;
        ButtonPanel = GameObject.Find("Canvas_phonon").transform.Find("ButtonPanel").gameObject;

        //set up dictionary
        get_radius();
        get_color();

        // sound
        AudioSource source = mode_panel_01.AddComponent<AudioSource>();
        AudioClip clip = Resources.Load<AudioClip>("AudioClip/sound_ui_hover");
        source.clip = clip;
    }


    // loaddata
    void loaddata(string fname)
    {
        //load data
        //print("loading data");
        if (!File.Exists(fname))
        {
            return;
        }
        var dict = JSON.Parse(File.ReadAllText(fname));
        UnitcellVector = (JSONArray)dict["UnitcellVector"];
        AtomSymbolList = (JSONArray)dict["AtomSymbolList"];
        AtomXYZList = (JSONArray)dict["AtomXYZList"];
        EigenvalueX = (JSONArray)dict["EigenvalueX"];
        EigenvalueY = (JSONArray)dict["EigenvalueY"];
        EigenvalueXtick = (JSONArray)dict["EigenvalueXtick"];
        EigenvectorAbs = (JSONArray)dict["EigenvectorAbs"];
        EigenvectorArg = (JSONArray)dict["EigenvectorArg"];
        QpointList = (JSONArray)dict["QpointList"];

        //set up phonon system
        //print("setting up phonon system");

        //2020.7.21
        //foreach (Transform obj in phonon_system_trans)
        //{
        //    Destroy(obj.gameObject);
        //}
        BondPairList.Clear();

        AtomAndBondPool.Ins.RecycleAll();
        //BondPairList.Clear();

        //make supercell
        //print("making supercell");
        M0 = AtomSymbolList.Count;
        M1 = supercell_M1;
        M2 = supercell_M2;
        N_atom = M0 * M1 * M2;
        AtomXYZ0_super = new Vector3[N_atom];
        AtomXYZ1_super = new Vector3[N_atom];
        AtomSymbol_super = new string[N_atom];
        Vector3 v1 = new Vector3(UnitcellVector[0][0], UnitcellVector[0][1], UnitcellVector[0][2]);
        Vector3 v2 = new Vector3(UnitcellVector[1][0], UnitcellVector[1][1], UnitcellVector[1][2]);
        for (int i2 = 0; i2 < M2; i2++)
        {
            for (int i1 = 0; i1 < M1; i1++)
            {
                for (int i0 = 0; i0 < M0; i0++)
                {
                    int ind = i2 * M1 * M0 + i1 * M0 + i0;
                    Vector3 xyz = new Vector3(AtomXYZList[i0][0], AtomXYZList[i0][1], AtomXYZList[i0][2]);
                    xyz = xyz + (i1-(M1-1)/2f)* v1 + (i2-(M2-1)/2f) * v2;
                    AtomXYZ0_super[ind] = xyz;
                    AtomXYZ1_super[ind] = xyz;
                    AtomSymbol_super[ind] = AtomSymbolList[i0];
                }
            }
        }

        // make bond pairs
        //print("making bond pairs");  
        for (int i1 = 0; i1 < N_atom; i1++)
        {
            float r1 = radius_dict[AtomSymbol_super[i1]] * r_sphere;
            Vector3 xyz1 = AtomXYZ1_super[i1];
            for (int i2 = i1 + 1; i2 < N_atom; i2++)
            {
                float r2 = radius_dict[AtomSymbol_super[i2]] * r_sphere;
                Vector3 xyz2 = AtomXYZ1_super[i2];
                float d12 = (xyz1 - xyz2).magnitude;

                if (d12 < r1 + r2 + deltaR) BondPairList.Add(new int[2] { i1, i2 });
            }
        }

        // plot atoms
        //print("plotting atoms");
        for (int i1 = 0; i1 < N_atom; i1++)
        {
            Vector3 xyz1 = AtomXYZ1_super[i1];            
            float r1 = radius_dict[AtomSymbol_super[i1]] * r_sphere;
            Color c1 = color_dict[AtomSymbol_super[i1]];
            plot_atom(i1, xyz1, r1, c1);
        }

        // plot bonds
        //print("plotting bonds");
        for (int i=0; i<BondPairList.Count; i++)
        {
            int i1 = BondPairList[i][0];
            int i2 = BondPairList[i][1];
            Vector3 xyz1 = AtomXYZ1_super[i1];
            float r1 = radius_dict[AtomSymbol_super[i1]] * r_sphere;
            Color c1 = color_dict[AtomSymbol_super[i1]];
            Vector3 xyz2 = AtomXYZ1_super[i2];
            float r2 = radius_dict[AtomSymbol_super[i2]] * r_sphere;
            Color c2 = color_dict[AtomSymbol_super[i2]];
            plot_bond(i+N_atom, xyz1, r1, c1, xyz2, r2, c2, r_bond);
        }
        
    }


    // Update
    void FixedUpdate()
    {
        // check if updated
        int index_selected = canvas00.index_selected;
        if (index_selected < 0) return;
        DatabaseIndex = index_selected.ToString("D4");
        if (DatabaseIndex != DatabaseIndex_)
        {
            //disable phonon_system
            phonon_system.SetActive(false);
            isInitialized = false;
            DatabaseIndex_ = DatabaseIndex;
            //disable mode panel if data not available
            string fname = DataPath + "phonons" + DatabaseIndex + ".json";
            bool isExist = File.Exists(fname);
            mode_panel_01.SetActive(isExist);
            mode_panel_02.SetActive(isExist);
            mode_panel_03.SetActive(isExist);
        }


        // load or reset
        bool tf0 = mode_panel_01.GetComponent<ButtonTransitioner>().isSelect;
        if (tf0)
        {
            if (!isInitialized)  // load data
            {
                phonon_system.SetActive(true);
                ButtonPanel.SetActive(true);
                I_t = 0;
                string fname = DataPath + "phonons" + DatabaseIndex + ".json";
                loaddata(fname);
                isInitialized = true;
            }
            else // reset
            {
                //do nothing
            }
        }
        else
        {
            if (!isInitialized)
            {
                //do nothing
            }
            else
            {
                I_k_selected = canvas01.I_k_selected;
                if (I_k_selected < 0) return;
                I_b_selected = canvas01.I_b_selected;
                if (I_b_selected < 0) return;

                vibrate(I_t, I_k_selected, I_b_selected);
                I_t = (I_t + 1) % N_t;
            }            
        }


        // amplitude
        float value02 = mode_panel_02.GetComponent<Slider>().value;
        amplitude = Mathf.Pow(4f, 2 * value02 - 1);


        // frequency
        float value03 = mode_panel_03.GetComponent<Slider>().value;
        float frequency = Mathf.Pow(4f, 2 * value03 - 1) / 2;
        period = 1 / frequency;
        N_t = Convert.ToInt32(period / Time.fixedDeltaTime);
    }


    // vibrate
    void vibrate(int I_t, int I_k, int I_b)
    {
        // update AtomXYZ1_super
        float tt = (2 * Mathf.PI) * I_t / N_t;
        var EigAbs = EigenvectorAbs[I_k][I_b];
        var EigArg = EigenvectorArg[I_k][I_b];
        Qpoint = new Vector3(QpointList[I_k][0], QpointList[I_k][1], QpointList[I_k][2]);
        for (int i2 = 0; i2 < M2; i2++)
        {
            for (int i1 = 0; i1 < M1; i1++)
            {
                for (int i0 = 0; i0 < M0; i0++)
                {
                    int ind = i2 * M1 * M0 + i1 * M0 + i0;
                    float phi = (2 * Mathf.PI) * (i1 * Qpoint.x + i2 * Qpoint.y);
                    float uu1 = EigAbs[i0][0] * Mathf.Cos(tt - EigArg[i0][0] - phi);
                    float uu2 = EigAbs[i0][1] * Mathf.Cos(tt - EigArg[i0][1] - phi);
                    float uu3 = EigAbs[i0][2] * Mathf.Cos(tt - EigArg[i0][2] - phi);
                    AtomXYZ1_super[ind] = AtomXYZ0_super[ind] + amplitude * new Vector3(uu1, uu2, uu3);
                }
            }
        }

        // update atoms
        for (int ii = 0; ii < N_atom; ii++)
        {
            update_atom(ii, AtomXYZ1_super[ii]);
        }

        // update bonds
        for (int ii = 0; ii < BondPairList.Count; ii++)
        {
            int i1 = BondPairList[ii][0];
            int i2 = BondPairList[ii][1];
            Vector3 xyz1 = AtomXYZ1_super[i1];
            Vector3 xyz2 = AtomXYZ1_super[i2];
            update_bond(ii, xyz1, xyz2);
        }
    }


    // update atom
    void update_atom(int i, Vector3 xyz)
    {
        var atom = phonon_system_trans.GetChild(i);
        atom.localPosition = xyz;
    }


    // update bond
    void update_bond(int i, Vector3 xyz1, Vector3 xyz2)
    {
        (float theta_y, float theta_z) = calc_direction(xyz1, xyz2);
        var bond = phonon_system_trans.GetChild(N_atom+i);
        float r12 = bond.localScale.x;
        float bond_length = (xyz2 - xyz1).magnitude;
        bond.localPosition = (xyz1 + xyz2) / 2;
        bond.localRotation = Quaternion.Euler(0, theta_y, theta_z);
        bond.localScale = new Vector3(r12, bond_length/2, r12);
    }
    

    // plot atom
    void plot_atom(int SiblingIndex, Vector3 xyz1, float r1, Color c1)
    {
        //2020.7.21
        //Transform atom1 = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;        

        Transform atom1 = AtomAndBondPool.Ins.getAtom().transform;        
        atom1.position = xyz1;
        atom1.localScale = r1 * new Vector3(1,1,1);
        atom1.GetComponent<MeshRenderer>().material = material_color(c1);
        atom1.SetParent(phonon_system_trans, false);
        atom1.SetSiblingIndex(SiblingIndex);
    }


    // plot_bond
    void plot_bond(int SiblingIndex, Vector3 xyz1, float r1, Color c1, Vector3 xyz2, float r2, Color c2, float r12)
    {
        (float theta_y, float theta_z) = calc_direction(xyz1, xyz2);
        float bond_length = (xyz2 - xyz1).magnitude;

        //2020.7.21
        //Transform bond = GameObject.CreatePrimitive(PrimitiveType.Cylinder).transform;        
        Transform bond = AtomAndBondPool.Ins.getBond().transform;

        bond.position = (xyz1 + xyz2) / 2;
        bond.rotation = Quaternion.Euler(0, theta_y, theta_z);
        bond.localScale = new Vector3(r12, bond_length / 2, r12);
        bond.GetComponent<MeshRenderer>().material = material_color(Color.white);
        bond.SetParent(phonon_system_trans, false);
        bond.SetSiblingIndex(SiblingIndex);
    }
    

    // material_color
    private Material material_color(Color color)
    {
        Material material = new Material(Shader.Find("Standard"));
        material.color = color;
        return material;
    }


    // calc_direction
    private (float theta_y, float theta_z) calc_direction(Vector3 xyz1, Vector3 xyz2)
    {
        // theta_y and theta_z are in degree
        // transform.eulerAngles = new Vector3(0, theta_y, theta_z);
        float theta_y, theta_z, pii;
        float dx, dy, dz, dd;
        dx = xyz2.x - xyz1.x;
        dy = xyz2.y - xyz1.y;
        dz = xyz2.z - xyz1.z;
        dd = (xyz1 - xyz2).magnitude;
        theta_z = (float)Mathf.Acos(dy / dd);
        theta_y = (float)Mathf.Atan2(dz, -dx);
        pii = (float)Mathf.PI;
        theta_y = theta_y * 180f / pii;
        theta_z = theta_z * 180f / pii;
        return (theta_y, theta_z);
    }


    // get color
    void get_color()
    {
        color_dict = new Dictionary<string, Color>()
        {
            { "H", new Color(1.000f,1.000f,1.000f)},
            {"He", new Color(0.851f,1.000f,1.000f)},
            {"Li", new Color(0.800f,0.502f,1.000f)},
            {"Be", new Color(0.761f,1.000f,0.000f)},
            { "B", new Color(1.000f,0.710f,0.710f)},
            { "C", new Color(0.565f,0.565f,0.565f)},
            { "N", new Color(0.188f,0.314f,0.973f)},
            { "O", new Color(1.000f,0.051f,0.051f)},
            { "F", new Color(0.565f,0.878f,0.314f)},
            {"Ne", new Color(0.702f,0.890f,0.961f)},
            {"Na", new Color(0.671f,0.361f,0.949f)},
            {"Mg", new Color(0.541f,1.000f,0.000f)},
            {"Al", new Color(0.749f,0.651f,0.651f)},
            {"Si", new Color(0.941f,0.784f,0.627f)},
            { "P", new Color(1.000f,0.502f,0.000f)},
            { "S", new Color(1.000f,1.000f,0.188f)},
            {"Cl", new Color(0.122f,0.941f,0.122f)},
            {"Ar", new Color(0.502f,0.820f,0.890f)},
            { "K", new Color(0.561f,0.251f,0.831f)},
            {"Ca", new Color(0.239f,1.000f,0.000f)},
            {"Sc", new Color(0.902f,0.902f,0.902f)},
            {"Ti", new Color(0.749f,0.761f,0.780f)},
            { "V", new Color(0.651f,0.651f,0.671f)},
            {"Cr", new Color(0.541f,0.600f,0.780f)},
            {"Mn", new Color(0.612f,0.478f,0.780f)},
            {"Fe", new Color(0.878f,0.400f,0.200f)},
            {"Co", new Color(0.941f,0.565f,0.627f)},
            {"Ni", new Color(0.314f,0.816f,0.314f)},
            {"Cu", new Color(0.784f,0.502f,0.200f)},
            {"Zn", new Color(0.490f,0.502f,0.690f)},
            {"Ga", new Color(0.761f,0.561f,0.561f)},
            {"Ge", new Color(0.400f,0.561f,0.561f)},
            {"As", new Color(0.741f,0.502f,0.890f)},
            {"Se", new Color(1.000f,0.631f,0.000f)},
            {"Br", new Color(0.651f,0.161f,0.161f)},
            {"Kr", new Color(0.361f,0.722f,0.820f)},
            {"Rb", new Color(0.439f,0.180f,0.690f)},
            {"Sr", new Color(0.000f,1.000f,0.000f)},
            { "Y", new Color(0.580f,1.000f,1.000f)},
            {"Zr", new Color(0.580f,0.878f,0.878f)},
            {"Nb", new Color(0.451f,0.761f,0.788f)},
            {"Mo", new Color(0.329f,0.710f,0.710f)},
            {"Tc", new Color(0.231f,0.620f,0.620f)},
            {"Ru", new Color(0.141f,0.561f,0.561f)},
            {"Rh", new Color(0.039f,0.490f,0.549f)},
            {"Pd", new Color(0.000f,0.412f,0.522f)},
            {"Ag", new Color(0.753f,0.753f,0.753f)},
            {"Cd", new Color(1.000f,0.851f,0.561f)},
            {"In", new Color(0.651f,0.459f,0.451f)},
            {"Sn", new Color(0.400f,0.502f,0.502f)},
            {"Sb", new Color(0.620f,0.388f,0.710f)},
            {"Te", new Color(0.831f,0.478f,0.000f)},
            { "I", new Color(0.580f,0.000f,0.580f)},
            {"Xe", new Color(0.259f,0.620f,0.690f)},
            {"Cs", new Color(0.341f,0.090f,0.561f)},
            {"Ba", new Color(0.000f,0.788f,0.000f)},
            {"La", new Color(0.439f,0.831f,1.000f)},
            {"Ce", new Color(1.000f,1.000f,0.780f)},
            {"Pr", new Color(0.851f,1.000f,0.780f)},
            {"Nd", new Color(0.780f,1.000f,0.780f)},
            {"Pm", new Color(0.639f,1.000f,0.780f)},
            {"Sm", new Color(0.561f,1.000f,0.780f)},
            {"Eu", new Color(0.380f,1.000f,0.780f)},
            {"Gd", new Color(0.271f,1.000f,0.780f)},
            {"Tb", new Color(0.188f,1.000f,0.780f)},
            {"Dy", new Color(0.122f,1.000f,0.780f)},
            {"Ho", new Color(0.000f,1.000f,0.612f)},
            {"Er", new Color(0.000f,0.902f,0.459f)},
            {"Tm", new Color(0.000f,0.831f,0.322f)},
            {"Yb", new Color(0.000f,0.749f,0.220f)},
            {"Lu", new Color(0.000f,0.671f,0.141f)},
            {"Hf", new Color(0.302f,0.761f,1.000f)},
            {"Ta", new Color(0.302f,0.651f,1.000f)},
            { "W", new Color(0.129f,0.580f,0.839f)},
            {"Re", new Color(0.149f,0.490f,0.671f)},
            {"Os", new Color(0.149f,0.400f,0.588f)},
            {"Ir", new Color(0.090f,0.329f,0.529f)},
            {"Pt", new Color(0.816f,0.816f,0.878f)},
            {"Au", new Color(1.000f,0.820f,0.137f)},
            {"Hg", new Color(0.722f,0.722f,0.816f)},
            {"Tl", new Color(0.651f,0.329f,0.302f)},
            {"Pb", new Color(0.341f,0.349f,0.380f)},
            {"Bi", new Color(0.620f,0.310f,0.710f)},
            {"Po", new Color(0.671f,0.361f,0.000f)},
            {"At", new Color(0.459f,0.310f,0.271f)},
            {"Rn", new Color(0.259f,0.510f,0.588f)},
            {"Fr", new Color(0.259f,0.000f,0.400f)},
            {"Ra", new Color(0.000f,0.490f,0.000f)},
            {"Ac", new Color(0.439f,0.671f,0.980f)},
            {"Th", new Color(0.000f,0.729f,1.000f)},
            {"Pa", new Color(0.000f,0.631f,1.000f)},
            { "U", new Color(0.000f,0.561f,1.000f)},
            {"Np", new Color(0.000f,0.502f,1.000f)},
            {"Pu", new Color(0.000f,0.420f,1.000f)},
            {"Am", new Color(0.329f,0.361f,0.949f)},
            {"Cm", new Color(0.471f,0.361f,0.890f)},
            {"Bk", new Color(0.541f,0.310f,0.890f)},
            {"Cf", new Color(0.631f,0.212f,0.831f)},
            {"Es", new Color(0.702f,0.122f,0.831f)},
            {"Fm", new Color(0.702f,0.122f,0.729f)},
            {"Md", new Color(0.702f,0.051f,0.651f)},
            {"No", new Color(0.741f,0.051f,0.529f)},
            {"Lr", new Color(0.780f,0.000f,0.400f)},
            {"Rf", new Color(0.800f,0.000f,0.349f)},
            {"Db", new Color(0.820f,0.000f,0.310f)},
            {"Sg", new Color(0.851f,0.000f,0.271f)},
            {"Bh", new Color(0.878f,0.000f,0.220f)},
            {"Hs", new Color(0.902f,0.000f,0.180f)},
            {"Mt", new Color(0.922f,0.000f,0.149f)},
        };
    }


    // get radius
    void get_radius()
    {
        radius_dict = new Dictionary<string, float>()
        {
            {"H", 0.31f},
            {"He", 0.28f},
            {"Li", 1.28f},
            {"Be", 0.96f},
            {"B", 0.84f},
            {"C", 0.76f},
            {"N", 0.71f},
            {"O", 0.66f},
            {"F", 0.57f},
            {"Ne", 0.58f},
            {"Na", 1.66f},
            {"Mg", 1.41f},
            {"Al", 1.21f},
            {"Si", 1.11f},
            {"P", 1.07f},
            {"S", 1.05f},
            {"Cl", 1.02f},
            {"Ar", 1.06f},
            {"K", 2.03f},
            {"Ca", 1.76f},
            {"Sc", 1.70f},
            {"Ti", 1.60f},
            {"V", 1.53f},
            {"Cr", 1.39f},
            {"Mn", 1.61f},
            {"Fe", 1.52f},
            {"Co", 1.50f},
            {"Ni", 1.24f},
            {"Cu", 1.32f},
            {"Zn", 1.22f},
            {"Ga", 1.22f},
            {"Ge", 1.20f},
            {"As", 1.19f},
            {"Se", 1.20f},
            {"Br", 1.20f},
            {"Kr", 1.16f},
            {"Rb", 2.20f},
            {"Sr", 1.95f},
            {"Y", 1.90f},
            {"Zr", 1.75f},
            {"Nb", 1.64f},
            {"Mo", 1.54f},
            {"Tc", 1.47f},
            {"Ru", 1.46f},
            {"Rh", 1.42f},
            {"Pd", 1.39f},
            {"Ag", 1.45f},
            {"Cd", 1.44f},
            {"In", 1.42f},
            {"Sn", 1.39f},
            {"Sb", 1.39f},
            {"Te", 1.38f},
            {"I", 1.39f},
            {"Xe", 1.40f},
            {"Cs", 2.44f},
            {"Ba", 2.15f},
            {"La", 2.07f},
            {"Ce", 2.04f},
            {"Pr", 2.03f},
            {"Nd", 2.01f},
            {"Pm", 1.99f},
            {"Sm", 1.98f},
            {"Eu", 1.98f},
            {"Gd", 1.96f},
            {"Tb", 1.94f},
            {"Dy", 1.92f},
            {"Ho", 1.92f},
            {"Er", 1.89f},
            {"Tm", 1.90f},
            {"Yb", 1.87f},
            {"Lu", 1.87f},
            {"Hf", 1.75f},
            {"Ta", 1.70f},
            {"W", 1.62f},
            {"Re", 1.51f},
            {"Os", 1.44f},
            {"Ir", 1.41f},
            {"Pt", 1.36f},
            {"Au", 1.36f},
            {"Hg", 1.32f},
            {"Tl", 1.45f},
            {"Pb", 1.46f},
            {"Bi", 1.48f},
            {"Po", 1.40f},
            {"At", 1.50f},
            {"Rn", 1.50f},
            {"Fr", 2.60f},
            {"Ra", 2.21f},
            {"Ac", 2.15f},
            {"Th", 2.06f},
            {"Pa", 2.00f},
            {"U", 1.96f},
            {"Np", 1.90f},
            {"Pu", 1.87f},
            {"Am", 1.80f},
            {"Cm", 1.69f},
        };
    }
}
