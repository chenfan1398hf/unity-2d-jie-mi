using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using Spine.Unity;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager :MonoSingleton<GameManager>
{
    #region 构造函数及其变量
    public GameManager()
    {
        configMag = new ConfigManager();
    }
    public static bool isDbugLog = true;
    public PlayerData playerData = null;                            //玩家数据（本地持久化）
    public ConfigManager configMag;
    private System.Random Random;                                   //随机种子
    private int TimeNumber = 0;
    private List<UnityAction> unityActionList = new List<UnityAction>();
    public bool isBattle = true;


    public static int TI_LI_MAX_NUMBER = 100;
    public static int TI_LI_CD_NUMBER = 600;

    #endregion

    private void Update()
    {
        foreach (var item in unityActionList)
        {
            item.Invoke();
        }
    }
    #region Awake()
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.targetFrameRate = 60;//设置帧率为60帧
        GetLocalPlayerData();
        Random = new System.Random(Guid.NewGuid().GetHashCode());
    }
    #endregion



    private void Start()
    {
        this.InvokeRepeating("CheckTime", 0, 0.1f);
        InitPanel();
        //BeginPingTuGame();
    }

    void CheckTime()
    {
        TimeNumber++;

        if (TimeNumber % 10 == 0)
        {
            CountDown1();
            CountDown2();
        }
        if (TimeNumber % 20 == 0)
        {

        }


    }


    #region OnApplicationPause(bool pause)切屏感知
    public void OnApplicationPause(bool pause)
    {
        if (pause == true)
        {
            if (isDbugLog)
                Debug.Log("切屏感知");
            SaveGame();
        }
    }
    #endregion

    #region OnApplicationQuit() 退出游戏感知
    public void OnApplicationQuit()
    {
        if (isDbugLog)
            Debug.Log("退出感知");
        SaveGame();

    }
    #endregion

    #region 获取本地数据
    public void GetLocalPlayerData()
    {
        playerData = PlayerData.GetLocalData();//读取本地持久化玩家数据(包括本土化设置)
        configMag.InitGameCfg();//读取配置表
        playerData.InitData();//根据配置表和本地数据初始化游戏数据
    }
    #endregion

    #region SaveGame() 保存玩家数据
    public void SaveGame()
    {
        //if(SocketManager.instance.socket!=null)
        //SocketManager.instance.socket.Disconnect();
        playerData.Save();
    }
    #endregion

    #region OnDestroy()
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
    #endregion

    /// <summary>
    /// 注册一个update在这里跑
    /// </summary>
    /// <param name="_action"></param>
    public void AddUpdateListener(UnityAction _action)
    {
        unityActionList.Add(_action);
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImage(string id, Image image)
    {
        string path = "Icon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片--装备图标
    /// </summary>
    public void SpritPropEquipIcon(string id, Image image)
    {
        string path = "EquipIcon/" + id;
        Sprite Tab3Img = ResourcesLoad.instance.Load<Sprite>(path);
        image.sprite = Tab3Img;
    }


    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, Image image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 加载图片
    /// </summary>
    public void SpritPropImageByPath(string path, SpriteRenderer image)
    {
        Sprite Tab3Img = Resources.Load(path, typeof(Sprite)) as Sprite;
        image.sprite = Tab3Img;
    }

    /// <summary>
    /// 添加预制体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fatherTransform"></param>
    /// <returns></returns>
    public GameObject AddPrefab(string name, Transform fatherTransform)
    {
        string newpath = "Prefab/" + name;
        GameObject obj = ObjPool.instance.GetObj(newpath, fatherTransform);
        obj.AddComponent<DesObj>();
        obj.GetComponent<DesObj>().InitDes(newpath);
        return obj;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(string name, GameObject gameObject)
    {
        string[] list = name.Split(new char[] { '(' });
        if (list.Length != 2)
        {
            string newpath = "Prefab/" + name;
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        else
        {
            string newpath = "Prefab/" + list[0];
            ObjPool.instance.Recycle(newpath, gameObject);
        }
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject prefabObj, GameObject gameObject, string _path = null)
    {
        ObjPool.instance.Recycle(prefabObj, gameObject, "Prefab/" + _path);
        return;
    }
    /// <summary>
    /// 销毁预制体
    /// </summary>
    /// <returns></returns>
    public void DestroyPrefab(GameObject gameObject)
    {
        string name = gameObject.GetComponent<DesObj>().name;
        ObjPool.instance.Recycle(name, gameObject);
        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(SkeletonGraphic _skeletonGraphic, bool isLoop, string _spineName, bool isRest)
    {
        if (isRest)
        {
            _skeletonGraphic.AnimationState.ClearTracks();
            _skeletonGraphic.AnimationState.Update(0);
        }
        _skeletonGraphic.AnimationState.SetAnimation(0, _spineName, isLoop);

        return;
    }
    /// <summary>
    /// 播放动画并重置动画到第0帧
    /// </summary>
    public void PlaySpine(Animator _animator, string _spineName, bool isRest)
    {
        //_animator.Play(_spineName, 0 ,0f);
        if (isRest)
        {
            //_animator.Update(0);
            _animator.Play(_spineName, 0, 0f);
        }
        else
        {
            _animator.Play(_spineName);
        }
        return;
    }
    /// <summary>
    /// 获取对象池内对象数据
    /// </summary>
    /// <returns></returns>
    public ObjPool.PoolItem GetPoolItem(string name)
    {
        string newpath = "Prefab/" + name;
        return ObjPool.instance.GetPoolItem(newpath); ;
    }
    /// <summary>
    /// 网络拉取图片
    /// </summary>
    /// <param name="_url"></param>
    /// <param name="_image"></param>
    /// <returns></returns>
    public IEnumerator GetHead(string _url, Image _image)
    {
        if (_url == string.Empty || _url == "")
        {
            _url = "https://p11.douyinpic.com/aweme/100x100/aweme-avatar/mosaic-legacy_3797_2889309425.jpeg?from=3067671334";
        }

        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(_url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(1f, 1f));
                _image.sprite = sprite;
                //Renderer renderer = plane.GetComponent<Renderer>();
                //renderer.material.mainTexture = texture;
            }
        }
    }

    /// <summary>
    /// 清空数据
    /// </summary>
    public void CleraPlayerData()
    {
        PlayerData.ClearLocalData();
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Editor/Tools/Clear")]
    static void CleraPlayerData1()
    {
        PlayerData.ClearLocalData();
    }
#endif
    private GameObject[] GetDontDestroyOnLoadGameObjects()
    {
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        //移除所有场景包含的对象
        for (var i = 0; i < SceneManager.sceneCount; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            var objs = scene.GetRootGameObjects();
            for (var j = 0; j < objs.Length; j++)
            {
                allGameObjects.Remove(objs[j]);
            }
        }
        //移除父级不为null的对象
        int k = allGameObjects.Count;
        while (--k >= 0)
        {
            if (allGameObjects[k].transform.parent != null)
            {
                allGameObjects.RemoveAt(k);
            }
        }
        return allGameObjects.ToArray();
    }
    public List<GameObject> maps = new List<GameObject>();
    public GameObject mimaPanel;
    public GameObject Panel2;
    public GameObject Panel3;
    public GameObject Panel5;
    public GameObject PanelBegin;
    public Text mimaText;
    private string mimaStr = "";
    private int mimaNumber = 0;
    public void InitPanel()
    {
        foreach (var item in maps)
        {
            item.SetActive(false);
        }
        maps[0].SetActive(true);
        mimaPanel.SetActive(false);
        Panel2.SetActive(false);
        Panel3.SetActive(false);
        ptPanelObj.SetActive(false);
        PanelBegin.SetActive(true);
        Panel5.SetActive(false);
        InitTiMu();
    }
    //打开密码界面
    public void OpenMiMaPanel()
    {
        mimaPanel.SetActive(true);
        mimaText.text = "";
    }
    //重置密码
    public void CzMiMa()
    {
        mimaText.text = "";
        mimaStr = "";
    }
    //输入密码
    public void ShuRuMiMa(int _number)
    {
        mimaNumber++;
        mimaStr += _number.ToString();
        mimaText.text = mimaStr;
        CheckMiMa();
    }
    //检查密码
    public void CheckMiMa()
    {
        if (mimaStr == "571")
        {
            //切换场景
            mimaPanel.SetActive(false);
            maps[0].SetActive(false);
            maps[1].SetActive(true);
            return;
        }
        if (mimaNumber >= 3)
        {
            mimaNumber = 0;
            CzMiMa();
            ShowMsg("The password is incorrect.");
        }
    }
    //打开章鱼对话界面
    private int duihuaNumber = 1;
    public void OpenPanel2()
    {
        Panel2.SetActive(true);
        Panel2.transform.Find("Image1").gameObject.SetActive(false);
        Panel2.transform.Find("Image2").gameObject.SetActive(false);
        Panel2.transform.Find("Image3").gameObject.SetActive(false);
        Panel2.transform.Find("Image4").gameObject.SetActive(false);
        Panel2.transform.Find("Image"+ duihuaNumber).gameObject.SetActive(true);
    }
    public void DuiHua()
    {
        duihuaNumber++;
        Panel2.transform.Find("Image1").gameObject.SetActive(false);
        Panel2.transform.Find("Image2").gameObject.SetActive(false);
        Panel2.transform.Find("Image3").gameObject.SetActive(false);
        Panel2.transform.Find("Image4").gameObject.SetActive(false);
        if (duihuaNumber > 4)
        {
            GoToTuShuGuan();
        }
        else
        {
            Panel2.transform.Find("Image" + duihuaNumber).gameObject.SetActive(true);
        }
      

    }
    //前往海洋图书馆
    public void GoToTuShuGuan()
    {
        Panel2.SetActive(false);
        maps[1].SetActive(false);
        maps[2].SetActive(true);
    }
    //打开水母对话界面
    public void OpenPanel3()
    {
        Panel3.SetActive(true);
        GetTiMu();
        countDown1 = 300;
    }
    public List<TiMuInfo> tiMuInfoList = new List<TiMuInfo>();
    public void InitTiMu()
    {
        for (int i = 0; i < 20; i++)
        {
            TiMuInfo info = new TiMuInfo();
            tiMuInfoList.Add(info);
        }
        tiMuInfoList[0].timu = "What is the largest ocean in the world?";
        tiMuInfoList[0].daan1 = "A. Atlantic Ocean";
        tiMuInfoList[0].daan2 = "B. Indian Ocean";
        tiMuInfoList[0].daan3 = "C. Pacific Ocean";
        tiMuInfoList[0].daan4 = "D. Arctic Ocean";
        tiMuInfoList[0].zhengQueDaAn = 3;

        tiMuInfoList[1].timu = "What organisms primarily make up coral reefs?";
        tiMuInfoList[1].daan1 = "A. Seaweed";
        tiMuInfoList[1].daan2 = "B. Coral polyps";
        tiMuInfoList[1].daan3 = "C. Shells";
        tiMuInfoList[1].daan4 = "D. Mollusks";
        tiMuInfoList[1].zhengQueDaAn = 2;

        tiMuInfoList[2].timu = "How do dolphins communicate?";
        tiMuInfoList[2].daan1 = "A. Low-frequency sounds";
        tiMuInfoList[2].daan2 = "B. Color changes";
        tiMuInfoList[2].daan3 = "C. Echolocation";
        tiMuInfoList[2].daan4 = "D. Vibrations";
        tiMuInfoList[2].zhengQueDaAn = 3;

        tiMuInfoList[3].timu = "What is the deepest ocean trench in the world?";
        tiMuInfoList[3].daan1 = "A. Constantine Trench";
        tiMuInfoList[3].daan2 = "B. Peru-Chile Trench";
        tiMuInfoList[3].daan3 = "C. Mariana Trench";
        tiMuInfoList[3].daan4 = "D. Tonga Trench";
        tiMuInfoList[3].zhengQueDaAn = 3;

        tiMuInfoList[4].timu = "Which whale is the largest animal in the world?";
        tiMuInfoList[4].daan1 = "A. Sperm whale";
        tiMuInfoList[4].daan2 = "B. Blue whale";
        tiMuInfoList[4].daan3 = "C. Orca";
        tiMuInfoList[4].daan4 = "D. Humpback whale";
        tiMuInfoList[4].zhengQueDaAn = 2;

        tiMuInfoList[5].timu = "What is the primary cause of tides?";
        tiMuInfoList[5].daan1 = "A. Wind force";
        tiMuInfoList[5].daan2 = "B. The gravitational pull of the moon";
        tiMuInfoList[5].daan3 = "C. Earthquakes";
        tiMuInfoList[5].daan4 = "D. Ocean currents";
        tiMuInfoList[5].zhengQueDaAn = 2;

        tiMuInfoList[6].timu = "Which sea has the highest salinity in the ocean?";
        tiMuInfoList[6].daan1 = "A. Mediterranean Sea";
        tiMuInfoList[6].daan2 = "B. Red Sea";
        tiMuInfoList[6].daan3 = "C. Baltic Sea";
        tiMuInfoList[6].daan4 = "D. Black Sea";
        tiMuInfoList[6].zhengQueDaAn = 2;

        tiMuInfoList[7].timu = "What type of fish do sharks belong to?";
        tiMuInfoList[7].daan1 = "A. Cartilaginous fish";
        tiMuInfoList[7].daan2 = "B. Bony fish";
        tiMuInfoList[7].daan3 = "C. Crustaceans";
        tiMuInfoList[7].daan4 = "D. Mollusks";
        tiMuInfoList[7].zhengQueDaAn = 1;

        tiMuInfoList[8].timu = "What is the main cause of coral bleaching?";
        tiMuInfoList[8].daan1 = "A. Rising sea temperatures";
        tiMuInfoList[8].daan2 = "B. Typhoons";
        tiMuInfoList[8].daan3 = "C. Decrease in shark population";
        tiMuInfoList[8].daan4 = "D. Deep-sea pressure";
        tiMuInfoList[8].zhengQueDaAn = 1;

        tiMuInfoList[9].timu = "What is the most abundant organism in the ocean?";
        tiMuInfoList[9].daan1 = "A. Fish";
        tiMuInfoList[9].daan2 = "B. Crustaceans";
        tiMuInfoList[9].daan3 = "C. Phytoplankton";
        tiMuInfoList[9].daan4 = "D. Octopus";
        tiMuInfoList[9].zhengQueDaAn = 3;

        tiMuInfoList[10].timu = "Where do sea turtles usually lay their eggs?";
        tiMuInfoList[10].daan1 = "A. Sandy beaches";
        tiMuInfoList[10].daan2 = "B. Seafloor";
        tiMuInfoList[10].daan3 = "C. Seaweed beds";
        tiMuInfoList[10].daan4 = "D. Coral reefs";
        tiMuInfoList[10].zhengQueDaAn = 1;

        tiMuInfoList[11].timu = "How do jellyfish catch their prey?";
        tiMuInfoList[11].daan1 = "A. Teeth";
        tiMuInfoList[11].daan2 = "B. Suction cups";
        tiMuInfoList[11].daan3 = "C. Stinging cells on their tentacles";
        tiMuInfoList[11].daan4 = "D. Mouthparts";
        tiMuInfoList[11].zhengQueDaAn = 3;

        tiMuInfoList[12].timu = "What is the largest living coral reef in the world?";
        tiMuInfoList[12].daan1 = "A. Red Sea Reef";
        tiMuInfoList[12].daan2 = "B. The Great Barrier Reef";
        tiMuInfoList[12].daan3 = "C. Maldives Coral Reef";
        tiMuInfoList[12].daan4 = "D. Galapagos Coral Reef";
        tiMuInfoList[12].zhengQueDaAn = 2;

        tiMuInfoList[13].timu = "What percentage of Earth's surface is covered by oceans?";
        tiMuInfoList[13].daan1 = "A. 50%";
        tiMuInfoList[13].daan2 = "B. 60%";
        tiMuInfoList[13].daan3 = "C. 71%";
        tiMuInfoList[13].daan4 = "D. 80%";
        tiMuInfoList[13].zhengQueDaAn = 3;

        tiMuInfoList[14].timu = "Which ocean animal can protect itself by releasing ink?";
        tiMuInfoList[14].daan1 = "A. Octopus";
        tiMuInfoList[14].daan2 = "B. Dolphin";
        tiMuInfoList[14].daan3 = "C. Shark";
        tiMuInfoList[14].daan4 = "D. Whale";
        tiMuInfoList[14].zhengQueDaAn = 1;

        tiMuInfoList[15].timu = "Which fish can fly above the water surface?";
        tiMuInfoList[15].daan1 = "A. Tuna";
        tiMuInfoList[15].daan2 = "B. Flying fish";
        tiMuInfoList[15].daan3 = "C. Flounder";
        tiMuInfoList[15].daan4 = "D. Shark";
        tiMuInfoList[15].zhengQueDaAn = 2;

        tiMuInfoList[16].timu = "Which ocean animal is known as the unicorn of the sea?";
        tiMuInfoList[16].daan1 = "A. Blue whale";
        tiMuInfoList[16].daan2 = "B. Narwhal";
        tiMuInfoList[16].daan3 = "C. Sperm whale";
        tiMuInfoList[16].daan4 = "D. Orca";
        tiMuInfoList[16].zhengQueDaAn = 2;


        tiMuInfoList[17].timu = "What is a red tide?";
        tiMuInfoList[17].daan1 = "A. A large ocean wave";
        tiMuInfoList[17].daan2 = "B. A harmful algal bloom";
        tiMuInfoList[17].daan3 = "C. A deep-sea current";
        tiMuInfoList[17].daan4 = "D. A shark migration phenomenon";
        tiMuInfoList[17].zhengQueDaAn = 2;

        tiMuInfoList[18].timu = "Which ocean animal has the strongest bite force?";
        tiMuInfoList[18].daan1 = "A. Crocodile";
        tiMuInfoList[18].daan2 = "B. Orca";
        tiMuInfoList[18].daan3 = "C. Great white shark";
        tiMuInfoList[18].daan4 = "D. Sperm whale";
        tiMuInfoList[18].zhengQueDaAn = 3;

        tiMuInfoList[19].timu = "What organ do starfish use to eat?";
        tiMuInfoList[19].daan1 = "A. Stomach";
        tiMuInfoList[19].daan2 = "B. Mouth";
        tiMuInfoList[19].daan3 = "C. Tentacles";
        tiMuInfoList[19].daan4 = "D. Gills";
        tiMuInfoList[19].zhengQueDaAn = 1;
    }
    //初始化题目
    private int index = 0;
    private int score = 0;
    public void GetTiMu()
    {
        Panel3.transform.Find("Text").GetComponent<Text>().text = tiMuInfoList[index].timu;
        Panel3.transform.Find("Button1/Text (Legacy)").GetComponent<Text>().text = tiMuInfoList[index].daan1;
        Panel3.transform.Find("Button2/Text (Legacy)").GetComponent<Text>().text = tiMuInfoList[index].daan2;
        Panel3.transform.Find("Button3/Text (Legacy)").GetComponent<Text>().text = tiMuInfoList[index].daan3;
        Panel3.transform.Find("Button4/Text (Legacy)").GetComponent<Text>().text = tiMuInfoList[index].daan4;
    }
    //答题
    public void SetDaAn(int _number)
    {
        if (_number == tiMuInfoList[index].zhengQueDaAn)
        {
            score++;
            Panel3.transform.Find("Score").GetComponent<Text>().text = score.ToString();
        }
        index++;
        if (index >= 20)
        {
            Debug.Log("答题结束");
            if (score >= 4)
            {
                //下一关
                Panel3.SetActive(false);
                maps[2].SetActive(false);
                maps[3].SetActive(true);
                BeginPingTuGame();
            }
            else
            {
                score = 0;
                index = 0;
                GetTiMu();
            }
            return;
        }
        GetTiMu();
    }

    //拼图游戏
    private Sprite[] spriteArray;
    public GameObject ptPanelObj;
    private int regNumber = 0;
    public void BeginPingTuGame()
    {
        ptPanelObj.SetActive(true);
        InitPt();
        countDown2 = 300;
    }
    //初始化拼图数据
    public void InitPt()
    {
        //初始化拼图碎片
        spriteArray = null;
        spriteArray = Resources.LoadAll<Sprite>("Tex/1234");
        for (int i = 0; i < spriteArray.Length; i++)
        {
            var image = ptPanelObj.transform.Find("list/Image" + i).GetComponent<Image>();
            image.sprite = spriteArray[i];
            var obj = AddPrefab("Card", ptPanelObj.transform);
            int randX = Util.randomInt(-800, 100);
            int randY = Util.randomInt(-300, 300);
            obj.GetComponent<card>().InitCard(i, new Vector2(randX, randY));
            obj.GetComponent<Image>().sprite = spriteArray[i];
        }

    }
    //检查位置是否OK
    public bool CheckCardVec(int _id, Vector3 _vec3)
    {
        GameObject obj = ptPanelObj.transform.Find("list/Image" + _id).gameObject;
        // 检查两个位置是否在指定范围内
        bool isWithinRange = IsWithinDistance(obj.transform.position, _vec3, 50f);
        if (isWithinRange)
        {
            obj.SetActive(true);
            regNumber++;
            if (regNumber >= spriteArray.Length)
            {
                //完成拼图
                ptPanelObj.SetActive(false);
                Panel5.SetActive(true);
            }
        }
        return isWithinRange;
    }
    // 计算两个Vector3位置之间的距离，并判断是否在指定范围内
    bool IsWithinDistance(Vector3 position1, Vector3 position2, float threshold)
    {
        // 计算两个位置之间的距离
        float distance = Vector3.Distance(position1, position2);
        Debug.Log(distance);
        // 判断距离是否小于阈值
        return distance < threshold;
    }

    public GameObject panelMsg;
    public void ShowMsg(string _msg)
    {
        StartCoroutine(ShowMsgIE(_msg));
    }
    public IEnumerator ShowMsgIE(string _msg)
    {
        panelMsg.SetActive(true);
        panelMsg.transform.Find("Text (Legacy)").GetComponent<Text>().text = _msg;
        yield return new WaitForSeconds(1f);
        panelMsg.SetActive(false);
    }
    //答题倒计时
    public Text down1;
    public Text down2;
    private int countDown1 = 300;
    private int countDown2 = 300;
    public void CountDown1()
    {
        countDown1--;
        down1.text = countDown1.ToString();
    }
    public void CountDown2()
    {
        countDown2--;
        down2.text = countDown2.ToString();
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        // 在 Unity 编辑器中停止播放
        EditorApplication.isPlaying = false;
#else
        // 在独立构建版本中退出游戏
        Application.Quit();
#endif
    }
}

